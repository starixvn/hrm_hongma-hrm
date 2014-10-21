USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_ApproveRequest_3]    Script Date: 10/21/2014 11:26:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_3]
	@RequestID bigint,	
	@EmpId int,
	@ApprovedBy int,
	@ApprovedDate datetime
AS
/*
Created: 2012/04/18@Dan
Task: approve late in request form (type 3)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- get request latein dates
DECLARE @FromDate datetime, @ToDate datetime, @LateIn int

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
	SELECT @FromDate = FromDate, @ToDate = ToDate, @LateIn = LateInMins 
	FROM dbo.WorkingTimeRequest
	WHERE Id = @RequestID

	-- update the working calendar to record the check-in time
	UPDATE dbo.EmployeeCalendar
	SET LateIn = @LateIn,
		ModifiedDate = @ApprovedDate,
		ModifiedBy = @ApprovedBy
	WHERE EmpID = @EmpId 
	and WorkDate between convert(varchar(10),@FromDate,111) and convert(varchar(10),@ToDate,111)

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 2 ,-- approved
		ProcessedBy= @ApprovedBy,
		ProcessedDate= @ApprovedDate,
		ModifiedDate = @ApprovedDate,
		ModifiedBy = @ApprovedBy
	WHERE Id = @RequestID
	
	INSERT INTO dbo.EmployeeCalendar_Request(EmpId, WorkDate, RequestId)
	SELECT EmpId, WorkDate, @RequestID FROM dbo.EmployeeCalendar WITH(NOLOCK) 
	WHERE EmpId = @EmpId
	AND IsOffDay <> 1 AND IsHoliday <> 1 
	AND WorkDate BETWEEN CONVERT(VARCHAR(10),@FromDate,111) AND CONVERT(VARCHAR(10),@ToDate,111)
	
	-- process the calendar into attendance
	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @FromDate, @ToDate, 1

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