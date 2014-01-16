USE [HRMS-HongMa]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_ApproveRequest_6]    Script Date: 10/18/2013 8:53:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_6]
	@RequestID bigint,	
	@EmpId int,
	@ApprovedBy int,
	@ApprovedDate datetime
AS
/*
Created: 2012/04/19@Dan
Task: approve leave request form (type 6)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @LeaveTypeID int, @TotalDays numeric(5,1),
		@FromDate datetime, @ToDate datetime

-- get request detail
select @FromDate = FromDate, @ToDate = ToDate, 
	@LeaveTypeID = LeaveTypeID, @TotalDays = TotalDays	
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID	

-- SET XACT_ABORT ON will cause the transaction to be uncommittable
-- when the constraint violation occurs. 
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
	-- update the working calendar to record the check-in time
	IF(@TotalDays <> 0)
	BEGIN
		UPDATE dbo.EmployeeCalendar
		SET IsLeaveDay = 1,
			LeaveTypeID = @LeaveTypeID,
			LeaveDay = 	case when (LeaveDay IS NULL OR LeaveDay = 0) then 
							case when @TotalDays = 0.5 then 0.5
							else 1 end 
						else 1 end,
			ModifiedDate = @ApprovedDate,
			ModifiedBy = @ApprovedBy 
		WHERE EmpID = @EmpId and	
			WorkDate between convert(varchar(10),@FromDate,111) and convert(varchar(10),@ToDate,111) -- cut off time before compare
	
		-- process the calendar into attendance
		EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @FromDate, @ToDate, 1
	
		-- insert leave record		
		IF (EXISTS(SELECT * FROM dbo.LeaveType WHERE Id = @LeaveTypeID and IsALDeduction = 1))		
			EXEC AnnualLeave_InsertTransaction @EmpId, @RequestID, 'Leave Request', @FromDate, @ToDate, @TotalDays, NULL, @ApprovedDate, @ApprovedBy
	END
	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 2 ,-- approved
		ProcessedBy= @ApprovedBy,
		ProcessedDate= @ApprovedDate,
		ModifiedDate = @ApprovedDate,
		ModifiedBy = @ApprovedBy 
	WHERE Id = @RequestID
	
	INSERT INTO dbo.EmployeeCalendar_Request(EmpId, WorkDate, RequestId, LeaveTypeId)
	SELECT EmpId, WorkDate, @RequestID, @LeaveTypeID FROM dbo.EmployeeCalendar WITH(NOLOCK) 
	WHERE EmpId = @EmpId 
	AND WorkDate BETWEEN CONVERT(VARCHAR(10),@FromDate,111) AND CONVERT(VARCHAR(10),@ToDate,111)

    COMMIT TRANSACTION; 
END TRY
BEGIN CATCH
	
	DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

	ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;
