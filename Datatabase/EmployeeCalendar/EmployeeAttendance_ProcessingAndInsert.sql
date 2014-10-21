USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_ProcessingAndInsert]    Script Date: 10/21/2014 10:55:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
declare	@empid int, 
		@fromtime datetime, 
		@totime datetime

set @empid= null --86 (Tuyen), 128 (Dan), 88 (Thien), 144 (Peter-truong hop cross date), 166 (William-truong hop holiday), 163 (Ethan-annual leave)
set @fromtime = '2012/04/02'
set @totime ='2012/04/18'
exec EmployeeAttendance_ProcessingAndInsert @empid, @fromtime , @totime 

select * from dbo.EmployeeAttendance
where empid= @empid and 
		AttendanceDate between @fromtime and @totime
*/
ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert] -- null, '2014-05-25', '2014-05-30'
		@empid int, 
		@fromtime datetime, 
		@totime datetime,
		@onlyUpdate bit = 0
AS

/*
Created: 2012/14/13@Dan
Task: Process the working calendar and insert the attendance data for EmployeeAttendance
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
	@empid: the employee id to process (null if process all)
	@fromtime: the date to start process
	@totime: the date to end process
*/
-- cut off time
set @fromtime = convert(varchar(10),@fromtime,111)
set @totime = convert(varchar(10),@totime,111)

DECLARE	@ShiftIdSpecial int 
-- get shift that need to process check in/out time specially
select @ShiftIdSpecial = id
from dbo.Shift (nolock)
where code= 'E3'

DECLARE	@HolidayLeaveType int 
DECLARE	@CalculateWokingLeaveType int 
-- get leave type of holiday
select @HolidayLeaveType = id
from dbo.LeaveType (nolock)
where IsHoliday = 1

-- get leave type of working
select @CalculateWokingLeaveType = id
from dbo.LeaveType (nolock)
where IsCalculateWoking = 1

--DECLARE	@LeaveHalfDayID int 
-- leave half day need processing specially
--SET @LeaveHalfDayID= DBO.[fnc_LeaveHalfDayID]() 

DECLARE @TimeRecord TABLE (	
	id INT PRIMARY KEY IDENTITY(1,1), 
	Empid int,
	TimeIn datetime,
	[TimeOut] datetime
)

CREATE TABLE #WorkingTimeRequest (
	id INT PRIMARY KEY IDENTITY(1,1), 
	FromTime datetime,
	ToTime datetime,
	LeaveTypeId INT
)

DECLARE @adjustExpectedInTime DATETIME, @adjustExpectedOutTime DATETIME, @adjustTotalMunites INT, @adjustLeaveTypeId INT,
@requestBusinessTimeInDay INT 
--get data checkin/out for each employee
DECLARE @yesterday datetime
-- set @yesterday = convert(varchar(10),dateadd(d,-1,@FromTime) ,111)  + ' 17:00' 
--set @yesterday = @ToTime
-- set @yesterday = convert(varchar(10),@FromTime ,111)
set @ToTime = DATEADD(d, 1, @ToTime)
INSERT INTO @TimeRecord (EmpID,TimeIn, TimeOut)
select EmpID, TimeIn, [TimeOut]
from dbo.fnc_getTableInOut(@FromTime, @ToTime, @empid)

-- view the @TimeRecord
--select * from @TimeRecord

-- working calendar of employees
declare @EmpCalendar table (
		id int primary key identity(1,1), 
		EmpId int,
		WorkDate datetime,
		ShiftId int,
		IsHoliday bit,
		isOffDay bit,
		IsLeaveDay bit,
		LeaveTypeId int,
		LateIn int,
		EarlyOut int,
		RequestedCheckinTime datetime,
		RequestedCheckoutTime datetime,
		BeginCheckInTime datetime,
		EndCheckInTime datetime,
		BeginCheckOutTime datetime,
		EndCheckOutTime datetime,
		BreakTimeMins int,
		LeaveDay decimal(18,1)		
		)
-- get working calendar of employees to calculate
insert into @EmpCalendar (
		EmpId ,
		WorkDate ,
		ShiftId ,
		IsHoliday ,
		isOffDay ,
		IsLeaveDay ,
		LeaveTypeId ,
		LateIn ,
		EarlyOut ,		
		RequestedCheckinTime,
		RequestedCheckoutTime,
		BeginCheckInTime ,
		EndCheckInTime ,
		BeginCheckOutTime ,
		EndCheckOutTime ,
		BreakTimeMins,
		LeaveDay
)
select 	
		EmpId ,
		WorkDate ,
		ShiftId ,
		IsHoliday ,
		isOffDay ,
		IsLeaveDay ,
		LeaveTypeId ,
		LateIn ,
		EarlyOut ,		
		RequestedCheckinTime,
		RequestedCheckoutTime,
		cast(WorkDate + ' ' + s.BeginCheckInTime AS datetime) [BeginCheckInTime],
		case when BeginCheckInTime>EndCheckInTime -- if the range is cross-date
			then dateadd(d,1, WorkDate + ' ' + s.EndCheckInTime)
			else cast(WorkDate + ' ' + s.EndCheckInTime AS datetime)
			end as [EndCheckInTime],

		case when BeginCheckInTime>=BeginCheckOutTime  -- if the range is cross-date
			then dateadd(d,1, WorkDate + ' ' + s.BeginCheckOutTime )
			else cast(WorkDate + ' ' + s.BeginCheckOutTime  AS datetime)
			end as [BeginCheckOutTime],

		case when BeginCheckInTime>=EndCheckOutTime  -- if the range is cross-date
			then dateadd(d,1, WorkDate + ' ' + s.EndCheckOutTime )
			else cast(WorkDate + ' ' + s.EndCheckOutTime  AS datetime)
			end as [EndCheckOutTime],
		BreakTimeMins,
		LeaveDay		
from dbo.EmployeeCalendar ec LEFT JOIN dbo.Shift s ON s.Id=ec.ShiftId
where (@EmpId is null or EmpId=@EmpId)
and WorkDate between @FromTime and @ToTime
order by EmpId, WorkDate

-- view the working calendar
-- select * from @EmpCalendar

declare	@ID int, 
		@EmpIdCur int,
		@WorkDate datetime,
		@ShiftId int,
		@IsHoliday bit,
		@isOffDay bit,
		@IsLeaveDay bit,
		@LeaveTypeId int,
		@LateIn int,
		@EarlyOut int,
		@RequestedCheckinTime datetime,
		@RequestedCheckoutTime datetime,
		@BeginCheckInTime datetime,
		@EndCheckInTime datetime,
		@BeginCheckOutTime datetime,
		@EndCheckOutTime datetime,
		@InOfMachine datetime,
		@OutOfMachine datetime,
		@ExpectedInTime datetime,
		@ExpectedOutTime datetime,
		@Absence bit,
		@IsRightShift bit,
		@ActualLateIn int,
		@ActualEarlyOut int,
		@TotalWorkHours decimal(18,2),
		@BreakTimeMins int,
		@LeaveDay decimal(18,1),
		@Notes nvarchar(2000)

declare @LeaveTypeDefaultId as int 
select @LeaveTypeDefaultId = dbo.fnc_getLeaveTypeDefault()

declare @i int
declare @j int
set @i=1
while exists (select 1 from @empcalendar where id = @i)
begin 
	-- reset parameter
	select @Absence=null,  @InOfMachine = null, @OutOfMachine = null, 
			@ID = null, @ExpectedOutTime  = null, @IsRightShift=null,
			@ExpectedInTime = null,	@ExpectedOutTime = null, 
			@ActualLateIn = 0, @ActualEarlyOut = 0,
			@RequestedCheckinTime = null, @RequestedCheckoutTime = null,
			@TotalWorkHours = null, @BreakTimeMins = 0,
			@adjustExpectedInTime = null, @adjustExpectedOutTime = null, @adjustTotalMunites = 0
	
	-- get one employee calendar to process
	select 		
		@EmpIdCur = EmpId,
		@WorkDate = WorkDate,
		@ShiftId = ShiftId,
		@IsHoliday = IsHoliday,
		@isOffDay = isOffDay,
		@IsLeaveDay = IsLeaveDay,
		@LeaveTypeId = LeaveTypeId,
		@LateIn = ISNULL(LateIn,0),
		@EarlyOut = ISNULL(EarlyOut,0),
		@RequestedCheckinTime = RequestedCheckinTime,
		@RequestedCheckoutTime = RequestedCheckoutTime,
		@BeginCheckInTime = BeginCheckInTime,
		@EndCheckInTime = EndCheckInTime,
		@BeginCheckOutTime = BeginCheckOutTime,
		@EndCheckOutTime = EndCheckOutTime,
		@BreakTimeMins  = BreakTimeMins,
		@LeaveDay = LeaveDay
	from @EmpCalendar
	where id=@i
	-- get the checkin/out data in the same day of working calendar
	-- case shift has working hours >= 24h (S8, S10)
	
	--if datediff(d,'2014-05-30', @WorkDate)=0 
	--	begin
	--		--select @EmpIdCur,@InOfMachine, @OutOfMachine
	--		select @BeginCheckInTime, @BeginCheckOutTime
	--		select * from @TimeRecord
	--		where EmpID = @EmpIdCur
	--	end
	if(datediff (hh, @BeginCheckInTime , @BeginCheckOutTime)>=24)
	begin
		select top 1 @ID = ID, @InOfMachine = TimeIn from @TimeRecord
		where EmpID = @EmpIdCur
				and ( datediff (d, TimeIn , @WorkDate) = 0
					or (TimeIn is null and datediff (d, @WorkDate, TimeOut) in (0,1))) -- in case missing timein
					
		--if datediff(d,'2014-05-30', @WorkDate)=0 
		--begin
		--	--select @EmpIdCur,@InOfMachine, @OutOfMachine
		--	select * from @TimeRecord
		--	where EmpID = @EmpIdCur
		--end
		
		select top 1 @OutOfMachine = TimeOut 
		from @TimeRecord
		where EmpID = @EmpIdCur
				and TimeOut is not null
				and ( datediff (d, TimeIn , @WorkDate) = 0 
					or (TimeIn is null and datediff (d, @WorkDate, TimeOut) in (0,1))) -- in case missing timein

	end
	else -- case normal shift 
	begin
		select top 1 @ID = ID, @InOfMachine = TimeIn, @OutOfMachine = TimeOut 
		from @TimeRecord
		where EmpID = @EmpIdCur
				and ( datediff (d, TimeIn , @WorkDate) = 0
					or (TimeIn is null and datediff (d, @WorkDate, TimeOut) in (0,1))) -- in case missing timein
	end
	
	-- if there is check-in request time, use it 
	if (@RequestedCheckinTime is not null)
		SET @InOfMachine = @RequestedCheckinTime

	-- if there is check-out request time, use it 
	if (@RequestedCheckoutTime is not null)
		SET @OutOfMachine = @RequestedCheckoutTime	

	if @InOfMachine is not null
	begin
		-- remove second from checkin/out time
		select @InOfMachine = dateadd (second, - datepart (second, @InOfMachine), @InOfMachine)

	end
	
	if @OutOfMachine is not null
	begin
		-- remove second from checkin/out time
		select @OutOfMachine = dateadd (second, - datepart (second, @OutOfMachine), @OutOfMachine)
		-- if checkout befort the shift start, then ignore
		if (@OutOfMachine<@EndCheckInTime)
			set @OutOfMachine = null
	end
	
	SELECT @requestBusinessTimeInDay = count(1) FROM WorkingTimeRequest WITH (NOLOCK) WHERE EmpId = @EmpIdCur 
				AND RequestType = 6 
				AND LeaveTypeID = @CalculateWokingLeaveType
				AND FromDate between @WorkDate and dateadd(ss,-1,dateadd(Day,1,@WorkDate))


	-- if this is off day, or leave day, or holiday
	if (@isOffDay = 1 or @IsHoliday = 1 or
			(@IsLeaveDay = 1 and @LeaveDay = 1 AND @LeaveTypeId != @CalculateWokingLeaveType AND @requestBusinessTimeInDay < 2)
		)
	begin
		if (@InOfMachine is not null and @OutOfMachine is not null )
			SET @TotalWorkHours =  (datediff(mi,@InOfMachine, @OutOfMachine))/ 60.0 
			
		set @Absence=1
			
		if (@IsHoliday=1)
			set @LeaveTypeId = @HolidayLeaveType
	end
	else if (@InOfMachine is null and @OutOfMachine is null ) -- if has no data check in/out --> the employee is absent 
	begin		
		set @Absence=1
		set @LeaveTypeId = @LeaveTypeDefaultId -- mark default leave type 
	end
	else -- the employee has checked in/out
	begin
		set @Absence = 0		
		--the employee not absent--> @LeaveTypeId = null (except the half leave day)
		if (@LeaveTypeId is not null and @LeaveTypeId = 1)
			set @LeaveTypeId = null

		-- calculate the expected in/out time (depent on check-in time of employee)
		declare @RequestLeaveTimeBegin datetime
		declare @RequestLeaveTimeEnd datetime
		declare @adjustCheckinTime datetime
		if (@InOfMachine is not null)
		begin
			if(@IsLeaveDay = 1 and @LeaveDay = 0.5)
			begin
				--select '11'
				select @RequestLeaveTimeBegin = FromDate, @RequestLeaveTimeEnd = ToDate 
					from WorkingTimeRequest 
					where EmpId = @EmpIdCur and 
					FromDate between @WorkDate and dateadd(ss,-1,dateadd(Day,1,@WorkDate))
					--day(FromDate) = day(@WorkDate) and
					--month(FromDate) = month(@WorkDate) and
					--year(FromDate) = year(@WorkDate)
				
				if (@ShiftID = @ShiftIdSpecial)
					if(@RequestLeaveTimeBegin< @WorkDate + ' 12:00')
						begin
							-- nghi sang
							select @RequestLeaveTimeBegin
							set @ExpectedInTime = @BeginCheckInTime
							set @ExpectedOutTime = @BeginCheckOutTime
						end
					else
						begin
							-- nghi chieu
							set @adjustCheckinTime = dateadd(mi, -@LateIn, @InOfMachine)
							if (@adjustCheckinTime<= @BeginCheckInTime)
								begin
									set @ExpectedInTime = @BeginCheckInTime
									set @ExpectedOutTime = @BeginCheckOutTime
								end
								else if (@ShiftID = @ShiftIdSpecial and @adjustCheckinTime> @BeginCheckInTime and @adjustCheckinTime <= @EndCheckInTime)
									begin
										set @ExpectedInTime = @adjustCheckinTime
										set @ExpectedOutTime = dateadd(mi, (datediff(mi, @BeginCheckInTime, @BeginCheckOutTime)),@adjustCheckinTime)
									end			
									else
									begin
										set @ExpectedInTime = @EndCheckInTime
										set @ExpectedOutTime = @EndCheckOutTime
									end
						end
				else
					begin
						-- khong phai ca E3
						set @ExpectedInTime = @EndCheckInTime
						set @ExpectedOutTime = @EndCheckOutTime
					end
			end
			else
			begin
				
				set @adjustCheckinTime = dateadd(mi, -@LateIn, @InOfMachine)
				
				if (@ShiftID = @ShiftIdSpecial and @adjustCheckinTime<= @BeginCheckInTime)
				begin
					set @ExpectedInTime = @BeginCheckInTime
					set @ExpectedOutTime = @BeginCheckOutTime
				end
				else if (@ShiftID = @ShiftIdSpecial and @adjustCheckinTime> @BeginCheckInTime and @adjustCheckinTime<= @EndCheckInTime)
				begin
					set @ExpectedInTime = @adjustCheckinTime
					set @ExpectedOutTime = dateadd(mi, datediff(mi, @BeginCheckInTime, @BeginCheckOutTime), @adjustCheckinTime)
				end			
				else
				begin
					set @ExpectedInTime = @EndCheckInTime
					set @ExpectedOutTime = @EndCheckOutTime
				end

				-- select  @ExpectedInTime, @ExpectedOutTime
			end
		end
			
			DELETE FROM #WorkingTimeRequest
			INSERT INTO #WorkingTimeRequest (FromTime, ToTime, LeaveTypeId)
			SELECT FromDate, ToDate, LeaveTypeID FROM WorkingTimeRequest WITH (NOLOCK)
			WHERE EmpId = @EmpIdCur 
				AND RequestType = 6
				AND Status = 2 
				AND FromDate between @WorkDate and dateadd(ss,-1,dateadd(Day,1,@WorkDate))
			SELECT @j = MIN(Id) FROM #WorkingTimeRequest
			SELECT @adjustExpectedInTime = null, @adjustExpectedOutTime = null, @adjustTotalMunites = 0
			WHILE exists (select 1 from #WorkingTimeRequest where id = @j)
			BEGIN

				SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime, @adjustLeaveTypeId = LeaveTypeId FROM #WorkingTimeRequest where id = @j

				IF(datediff(mi, @ExpectedInTime, @RequestLeaveTimeBegin) < 5 AND datediff(mi, @ExpectedInTime, @RequestLeaveTimeBegin) > -5)
				BEGIN
					SET @ExpectedInTime = @RequestLeaveTimeEnd
				END
				ELSE IF (datediff(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) < 5 AND datediff(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) > -5)
				BEGIN
					SET @ExpectedOutTime = @RequestLeaveTimeBegin
				END


				IF(@adjustLeaveTypeId = @CalculateWokingLeaveType)
				BEGIN
					SET @adjustTotalMunites = @adjustTotalMunites + datediff(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
				END

				SET @j = @j + 1
			END

			
		-- Quinn
		-- neu nghi 1/2 ngay xu ly rieng, nguoc lai giu nguyen cach xu ly cua a Dan
		-- calculate the Early Out minutes (after subtracting the minutes has been allowed to check out early)

		if ( dateadd(mi, @EarlyOut, @OutOfMachine) < @ExpectedOutTime  )
			set @ActualEarlyOut = datediff (minute, dateadd(mi, @EarlyOut, @OutOfMachine), @ExpectedOutTime)	

		-- calculate the Late In minutes (after subtracting the minutes has been allowed to check in late)
		if ( dateadd(mi, -@LateIn, @InOfMachine) >  @ExpectedInTime ) 
			set @ActualLateIn = datediff (minute, @ExpectedInTime, dateadd(mi,-@LateIn , @InOfMachine)) 
	--if(@EmpIdCur = 18)
	--	begin
	--		select @WorkDate
	--		select @ExpectedInTime
	--		select @LateIn
	--		select @InOfMachine
	--		select @ActualLateIn
	--	end
		--select @adjustTotalMunites
		if(@IsLeaveDay = 1 and @LeaveDay = 0.5)
			begin 
				-- if leave half day
				-- calculate the actual work hour
				if (@InOfMachine is not null and @OutOfMachine is not null )
					begin
						set @TotalWorkHours =  (datediff(mi,@InOfMachine, @OutOfMachine))/ 60.0 
						IF (@adjustTotalMunites > 0)
						BEGIN
							SET @TotalWorkHours =  (@TotalWorkHours * 60 + @adjustTotalMunites ) / 60.0
						END
						IF(@TotalWorkHours > 240)
						BEGIN
							SET @TotalWorkHours = @TotalWorkHours - ISNULL(@BreakTimeMins,0)
						END
						-- if(@LeaveTypeId = @CalculateWokingLeaveType)
						-- set @TotalWorkHours  = @TotalWorkHours + (datediff(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd) - ISNULL(@BreakTimeMins,0))/ 60.0 
					end
				if(
					 datediff(mi, (dateadd(mi,(datediff(mi,@ExpectedInTime, @ExpectedOutTime)/2 - 90), @ExpectedInTime)), @RequestLeaveTimeBegin) <0
				)
					begin
						--nghi buoi sang
						-- calculate the Late In minutes (after subtracting the minutes has been allowed to check in late)				
						 set @ActualLateIn = @ActualLateIn - ((datediff(mi, @BeginCheckInTime, @BeginCheckOutTime) - ISNULL(@BreakTimeMins, 0))/2 + ISNULL(@BreakTimeMins,0))
						 if(@ActualLateIn < 0)
							set @ActualLateIn = 0
					end
				else
					begin
						-- nghi buoi chieu
						-- calculate the Early Out minutes (after subtracting the minutes has been allowed to check out early)
						set @ActualEarlyOut = @ActualEarlyOut - ((datediff(mi, @BeginCheckInTime, @BeginCheckOutTime) - ISNULL(@BreakTimeMins, 0))/2 + ISNULL(@BreakTimeMins, 0))
						if(@ActualEarlyOut < 0)
							set @ActualEarlyOut = 0
					end
					
			end
		-- end xu ly nghi 1/2 ngay (Quinn)
		else
			begin
				-- calculate the actual work hour
				if (@InOfMachine is not null and @OutOfMachine is not null )

						set @TotalWorkHours =  (datediff(mi,@InOfMachine, @OutOfMachine) - ISNULL(@BreakTimeMins,0))/ 60.0

						SET @TotalWorkHours =  (@TotalWorkHours * 60 + @adjustTotalMunites) / 60.0
						
						if(@TotalWorkHours < 4)
							set @TotalWorkHours = (@TotalWorkHours * 60 + ISNULL(@BreakTimeMins,0)) / 60.0
			end
		
		-- check in/out in right shift
		if 
		(
			(	/*Gio Vao thuoc khoang chenh lech Gio Vao quy dinh cua Ca*/
				@InOfMachine is not null			
				and datediff (minute, dateadd(mi,-@LateIn , @InOfMachine), @EndCheckInTime) >= 0
			)
			or
			(   /*Gio Ra thuoc khoang chenh lech Gio Ra quy dinh cua Ca*/
				@OutOfMachine is not null
				and datediff (minute, dateadd(mi, @EarlyOut, @OutOfMachine), @BeginCheckOutTime) <= 0 
			)
		)
		begin 			
			set @IsRightShift = 1
		end
		else -- not right shift
		begin 			
			set @IsRightShift = 0
		end 
	end

	-- insert or update into EmployeeAttendance
	if (@onlyUpdate=1 OR
		exists(select * from dbo.EmployeeAttendance with (nolock)
				where EmpId=@EmpIdCur and datediff(d,AttendanceDate, @WorkDate)=0))
	begin
		--update
		update dbo.EmployeeAttendance
		set 
			ShiftId = @ShiftId, 
			InFixed = @InOfMachine,
			OutFixed = @OutOfMachine,
			ExpectedInTime = @ExpectedInTime,
			ExpectedOutTime = @ExpectedOutTime,
			LateIn  = @ActualLateIn,
			EarlyOut = @ActualEarlyOut,
			LeaveTypeId = @LeaveTypeId,
			Absence = @Absence,
			TotalWorkHours = @TotalWorkHours,
			IsRightShift = @IsRightShift,
			LeaveDay = @LeaveDay,
			InsertDate = getdate()
		where EmpId=@EmpIdCur and datediff(d,AttendanceDate, @WorkDate)=0
	end
	else
	begin
		--insert
		--select 0
		insert into dbo.EmployeeAttendance
		(
			EmpId,
			AttendanceDate,
			ShiftId,
			InFixed,
			OutFixed,
			ExpectedInTime,
			ExpectedOutTime,
			LateIn,
			EarlyOut,
			LeaveTypeId,
			Absence,
			TotalWorkHours,
			IsRightShift,
			LeaveDay,
			InsertDate
		)
		values
		(
			@EmpIdCur, 
			@WorkDate,
			@ShiftId,
			@InOfMachine,
			@OutOfMachine,
			@ExpectedInTime,
			@ExpectedOutTime,
			@ActualLateIn,
			@ActualEarlyOut,
			@LeaveTypeId,
			@Absence,
			@TotalWorkHours,
			@IsRightShift,
			@LeaveDay,
			getdate()
		)			
	end
	/*
		-- for testing purpose
		print 'EMPID : ' + cast(@EmpIdCur as varchar(100))
		print '@WorkDate ' + cast(@WorkDate as varchar(100))		
		print '@InOfMachine ' + cast(@InOfMachine as varchar(100))
		print '@OutOfMachine ' + cast(@OutOfMachine as varchar(100))
		print '@isOffDay ' + cast(@isOffDay as varchar(100))
		print '@IsLeaveDay ' + cast(@IsLeaveDay as varchar(100))
		print '@IsHoliday ' + cast(@IsHoliday as varchar(100))
		print '@Absence: ' + cast(@Absence as varchar(100))
		print 'ShiftId' + cast(@ShiftId as varchar(100))
		print 'ExpectedInTime ' + cast(@ExpectedInTime as varchar(100)) 
		print 'ExpectedOutTime ' + cast(@ExpectedOutTime as varchar(100)) 
		print 'LateIn ' + cast(@ActualLateIn as varchar(100))
		print 'EarlyOut' +  cast(@ActualEarlyOut as varchar(100))
		print 'LeaveTypeId ' + cast(@LeaveTypeId as varchar(100))
		print 'TotalWorkHours ' + cast(@TotalWorkHours as varchar(100))
		print 'IsRightShift' + cast( @IsRightShift as varchar(100))
	*/	
	set @i=@i+1
	SET @j = 1
end 
DROP TABLE #WorkingTimeRequest