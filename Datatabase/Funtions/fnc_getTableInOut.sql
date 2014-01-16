USE [NexHRM]
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetTableInOut]    Script Date: 9/12/2012 1:39:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
--select * from dbo.fnc_GetTableInOut('2012/04/01', '2012/04/08', 60)
select 	ID, empID, RecordTime, InOutType
	from 	dbo.TimeRecord with (nolock)
	where	(InOutType is not null)
		and datediff (day, RecordTime, '2012/04/01') <= 0
		and datediff (day, RecordTime, '2012/04/08') >= 0
		and  EmpID=144
	order by EmpID, RecordTime, InOutType

select * from dbo.fnc_GetTableInOut('2012/07/18','2012/07/18', null)
*/
ALTER function [dbo].[fnc_GetTableInOut]
(
	@FromTime datetime,
	@ToTime datetime,
	@EmpID int = NULL
) 
returns @tbl_InOut table
		(
			EmpID int
			, TimeIn datetime null
			, TimeOut datetime null
		)
as
/*
Created: 2012/04/09@Dan
Task: get check in/out time for employess
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
begin		
	declare @RecordID numeric(18,0), 
			@EmpIDCur int, 
			@RecordTime datetime, 
			@InOutType bit,
			@IsProcessed bit,
			@CrossShiftTime varchar(5)
	
	set @CrossShiftTime=  '17:00' -- the time that shist startes to be in corss-date

	declare @RecordIDPrev numeric(18,0), 
			@EmpIDPrev char(15), 
			@RecordTimePrev datetime
	declare @InFlag bit, @i int
	
	declare @InOfMachine datetime, @OutOfMachine datetime	

	-- Khai bao Table dung de luu tru du lieu khi duyet, khong lien quan gi den Table chinh TimeRecord
	declare @tbl_TimeRecord table
	( 
		id INT PRIMARY KEY IDENTITY(1,1), 
		RecordID decimal(18, 0)
		, EmpID int
		, RecordTime datetime null
		, InOutType bit null
		, IsProcessed bit null
	)	

	insert @tbl_TimeRecord (RecordID, EmpID, RecordTime, InOutType)
	select 	ID, empID, RecordTime, InOutType
	from 	dbo.TimeRecord with (nolock)
	where	(InOutType is not null)
		and RecordTime> = @FromTime
		-- Quinn edit: Shift M1: check out > 0h hom sau	
		-- and RecordTime< = dateadd(d,1,@ToTime)
		and RecordTime< = convert(varchar(10),dateadd(d,1,@ToTime) ,111)  + ' 07:00' 
		-- and Quinn edit		
		and (@EmpID is null or EmpID=@EmpID)	
	UNION ALL	-- request checkin
	select 	-ID, empID, RequestedCheckinTime as RecordTime, 0  -- use negative to know it's requested time, not from Bioadmin
	from 	dbo.EmployeeCalendar with (nolock)
	where	(RequestedCheckinTime is not null)
		and WorkDate> = @FromTime
		and WorkDate< = @ToTime
		and (@EmpID is null or EmpID=@EmpID)
	UNION ALL	--  request check out time	
	select 	-ID - 10000000, empID, RequestedCheckoutTime, 1 -- use negative to know it's requested time, not from Bioadmin
	from 	dbo.EmployeeCalendar with (nolock)
	where	(RequestedCheckoutTime is not null)
		and WorkDate> = @FromTime
		and WorkDate< = @ToTime
		and (@EmpID is null or EmpID=@EmpID)
	order by EmpID, RecordTime, InOutType	
			
	select @RecordIDPrev = 0, @EmpIDPrev = 0, @RecordTimePrev = null, @i = 1

	WHILE EXISTS (SELECT 1 FROM @tbl_TimeRecord WHERE id = @i)
	BEGIN
		--reset params
		select @InOfMachine= null, @OutOfMachine = null
		
		--get data to process
		SELECT @RecordID = RecordID, @EmpIDCur= EmpID, @RecordTime = RecordTime, @InOutType = InOutType, @IsProcessed = IsProcessed 
		FROM @tbl_TimeRecord WHERE id = @i

		IF (@IsProcessed=1)
		BEGIN 
			SET @i = @i + 1
			continue;
		END 
		--------------------		

		if @EmpIDCur <> @EmpIDPrev
		begin
			-- reset  for current processing emp
			select @InFlag = 0
			select @EmpIDPrev = @EmpIDCur

		end
		if @InOutType = 0  --//In Record
		begin
			
			if @InFlag=0 -- lan check in dau tien
			begin
				select @RecordIDPrev = @RecordID, @RecordTimePrev = @RecordTime
				select @InFlag = 1

				-- neu thoi gian checkin nam trong khoang thoi gian lam viec tren ca 2 ngay (cross date) 
				if (@RecordTimePrev >= cast(CONVERT(VARCHAR(10),@RecordTimePrev,111) + ' ' + @CrossShiftTime as datetime))
				begin									
					--tim thoi gian check out trong ngay ke tiep
					select top 1 @OutOfMachine = RecordTime, @RecordID= ID
					from dbo.TimeRecord WITH (NOLOCK)
					where EmpID = @EmpIDPrev and 
						RecordTime > @RecordTimePrev
						-- Quinn change:
						-- and RecordTime< dateadd(d,1,@RecordTimePrev)
						and RecordTime< dateadd(hh,-8,dateadd(d,1,@RecordTimePrev))
						-- end Quinn change
						and InOutType = 1
					order by RecordTime
					
					-- da du thong tin checkin-checkout
					if (@OutOfMachine is not null)					
					begin						
						insert @tbl_InOut (EmpID, TimeIn, TimeOut)
						values (@EmpIDPrev, @RecordTimePrev, @OutOfMachine)
						
						-- xoa record da xu ly
						delete @tbl_TimeRecord where RecordID = @RecordIDPrev 
						-- marke the records has been process to ignore process later
						update @tbl_TimeRecord 
						set isprocessed=1
						where RecordID = @RecordID

						select @InFlag = 0
					end
				end
			end
			else
			begin
				
				-- checkin nhieu lan trong vong 12h (ma chua checkout), xoa di, chi giu lai 1 lan checkin dau tien
				if (datediff(hh,@RecordTimePrev, @RecordTime) >=0
					and datediff(hh,@RecordTimePrev, @RecordTime)<=12 )
					delete @tbl_TimeRecord where id = @i
				else -- neu checkin nhieu lan nhung ko phai lien tiep (co the la hom sau) thi xu ly binh thuong
					select @RecordIDPrev = @RecordID, @RecordTimePrev = @RecordTime
			end
		end
		else --//Out Record
		begin
			if @InFlag = 1 --//Match one pair
			begin
				select @InOfMachine = @RecordTimePrev, @OutOfMachine = @RecordTime
				-- Can kiem tra so phut keo dai cua ca lam viec
				if (@InOfMachine < @OutOfMachine 
						and datediff(hh,@InOfMachine,@OutOfMachine)<20)
				begin
					insert @tbl_InOut (EmpID, TimeIn, TimeOut)
					values (@EmpIDCur, @InOfMachine, @OutOfMachine)
	
					delete @tbl_TimeRecord where RecordID = @RecordIDPrev OR RecordID = @RecordID
				end	
				select @InFlag = 0
			end		
		end		

	SET @i = @i + 1

	END
	
	-- delete processed records
	DELETE @tbl_TimeRecord
	WHERE IsProcessed=1

	-- Non full TimeIn/TimeOut
	declare cs cursor for 
	select RecordID, EmpID, RecordTime, InOutType 
	from @tbl_TimeRecord 
	order by EmpID,RecordTime
	open cs
	fetch next from cs into @RecordID, @EmpIDCur, @RecordTime, @InOutType
	while @@fetch_status = 0
	begin
		if @InOutType = 0 	
			insert @tbl_InOut(EmpID, TimeIn) values (@EmpIDCur, @RecordTime)
		else if (@InOutType = 1)
 			insert @tbl_InOut(EmpID, TimeOut) values (@EmpIDCur, @RecordTime)		
		delete @tbl_TimeRecord where RecordID = @RecordID
		fetch next from cs into @RecordID, @EmpIDCur, @RecordTime, @InOutType
	end
	close cs
	deallocate cs

	return
end