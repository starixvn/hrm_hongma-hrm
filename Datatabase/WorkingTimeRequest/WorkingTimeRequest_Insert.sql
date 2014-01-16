USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_Insert]    Script Date: 9/12/2012 4:11:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[WorkingTimeRequest_Insert]
	@EmpId int, 
	@RequestType smallint,	
	@IsPlanned bit,
	@Remark nvarchar(2000),				
	@FromDate datetime, 
	@FromShift int, 	
	@ToDate datetime, 
	@ToShift int, 	
	@LeaveTypeID int, 
	@TotalDays numeric(5,1), 
	@LateInMins int,
	@EarlyOutMins int,
	@CreatedDate datetime,
	@CreatedBy int,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT	
AS
/*
Created: 2012/04/18@Dan
Task: insert a request form
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
IF (@RequestType IN (1,2) AND @FromDate IS NULL)
BEGIN
	SET @ErrCode =101
	SET @ErrMsg = 'Missing check in/out request has the invalid parameter'
	RETURN;
END

IF (@RequestType IN (3) AND (@FromDate IS NULL OR isnull(@LateInMins,0) <= 0))
BEGIN
	SET @ErrCode =102
	SET @ErrMsg = 'Late in request has the invalid parameter'
	RETURN;
END

IF (@RequestType IN (4) AND (@FromDate IS NULL OR isnull(@EarlyOutMins,0) <= 0))
BEGIN
	SET @ErrCode =103
	SET @ErrMsg = 'Late out request has the invalid parameter'
	RETURN;
END

IF (@RequestType IN (5) AND 
	 (@FromDate IS NULL OR @FromShift IS NULL OR @ToShift IS NULL)
	)
BEGIN
	SET @ErrCode =104
	SET @ErrMsg = 'Change-shift request has the invalid parameter'
	RETURN;
END

IF (@RequestType IN (6) AND 
	(@FromDate IS NULL OR @ToDate IS NULL OR @LeaveTypeID IS NULL 
		OR ISNULL(@TotalDays,0) <= 0 )
	)
BEGIN
	SET @ErrCode =105
	SET @ErrMsg = 'Leave request has the invalid parameter'
	RETURN;
END

--check maximum request in month
IF (@RequestType IN (3,4,5))
BEGIN
	EXEC WorkingTimeRequest_CheckNumberRequest @empid, @RequestType, @FromDate, @ErrCode OUTPUT, @ErrMsg OUTPUT	
	IF (@ErrCode <>0)
		RETURN
END

-- check if there is another request in the same date
IF (@RequestType IN (1,2,3,4,5) AND 
	exists(select * from dbo.WorkingTimeRequest
		where empid=@empid 
			and RequestType=@RequestType
			and status in (1,2)
			and datediff(d, fromdate, @FromDate ) = 0
	)
)
BEGIN
	SET @ErrCode =106
	SET @ErrMsg = 'There is another request in the same date'
	RETURN;
END

-- check if there is another request in the same range
IF (@RequestType IN (6) AND 
	exists(select * from dbo.WorkingTimeRequest
		where empid=@empid 
			and RequestType=@RequestType
			and status in (1,2)
			and (	CONVERT(VARCHAR(10),fromdate,111) between CONVERT(VARCHAR(10),@fromdate,111) and CONVERT(VARCHAR(10),@todate,111) 
				or  CONVERT(VARCHAR(10),todate,111) between CONVERT(VARCHAR(10),@fromdate,111) and CONVERT(VARCHAR(10),@todate,111) 
				or  CONVERT(VARCHAR(10),@fromdate,111) between CONVERT(VARCHAR(10),fromdate,111) and CONVERT(VARCHAR(10),todate,111) 
				or  CONVERT(VARCHAR(10),@todate,111) between CONVERT(VARCHAR(10),fromdate,111) and CONVERT(VARCHAR(10),todate,111) 
			)
		)
	)
BEGIN
	SET @ErrCode =107
	SET @ErrMsg = 'There is another request in the same range'
	RETURN;
END

-- check the late in, ealry out min <=60 
IF ( (@RequestType = 3 AND @LateInMins > 60)
	or (@RequestType = 4 AND @EarlyOutMins > 60)	
	)
BEGIN
	SET @ErrCode =108
	SET @ErrMsg = 'The maximun late in/early out minutes is 60'
	RETURN;
END

-- check the total day is valid
IF ( @RequestType = 6)
BEGIN
	IF (@TotalDays < 1 AND DATEDIFF(d,@FromDate, @ToDate)<> 0 )
	BEGIN
		SET @ErrCode =109
		SET @ErrMsg = 'The total days is not correct'
		RETURN;
	END
	ELSE
	BEGIN
		IF (@TotalDays >= 1 AND 
			@TotalDays <> (SELECT COUNT(*) FROM dbo.EmployeeCalendar
							WHERE EmpID= @EmpId AND WorkDate BETWEEN @FromDate AND @ToDate
							AND IsHoliday = 0 AND IsOffDay = 0 AND IsLeaveDay = 0)
			)
		BEGIN
			SET @ErrCode =110
			SET @ErrMsg = 'The total days is not correct'
			RETURN;
		END
	END
END

-- check the total day is not greater than annual leave balance
IF ( @RequestType = 6)
BEGIN
	IF (EXISTS(SELECT * FROM dbo.LeaveType WHERE Id = @LeaveTypeID AND IsALDeduction = 1)
		AND @TotalDays > (SELECT AnnualLeaveBalance FROM employee WHERE id = @EmpId)
	)		
	BEGIN
		SET @ErrCode =111
		SET @ErrMsg = 'You have not enough annual leave days to request'
		RETURN;
	END
	
END	

-- cut off time
IF (@RequestType IN (3,4,5))
BEGIN
	SET @fromdate = CONVERT(VARCHAR(10),@fromdate,111)
	IF (@todate IS NOT NULL)
		SET @todate = CONVERT(VARCHAR(10),@todate,111)
END

INSERT INTO dbo.WorkingTimeRequest
(	Status,
	EmpId, 
	RequestType,	
	IsPlanned,
	Remark,	
	CreatedDate, 
	CreatedBy,	
	ModifiedDate, 
	ModifiedBy,	
	FromDate,
	FromShift,
	ToDate,
	ToShift,
	LeaveTypeID,
	TotalDays,
	LateInMins,
	EarlyOutMins
)
VALUES
(
	1, -- pending status as default
	@EmpId, 
	@RequestType,	
	@IsPlanned,
	@Remark,	
	@CreatedDate, 
	@CreatedBy,	
	@CreatedDate, 
	@CreatedBy,	
	@FromDate,
	@FromShift,
	@ToDate,
	@ToShift,
	@LeaveTypeID,
	@TotalDays,
	@LateInMins,
	@EarlyOutMins
)

DECLARE @id bigint

SELECT @id = cast(SCOPE_IDENTITY() as bigint)

EXEC [WorkingTimeRequest_WriteHistory] @id 

SELECT @id 
SET @ErrCode = 0