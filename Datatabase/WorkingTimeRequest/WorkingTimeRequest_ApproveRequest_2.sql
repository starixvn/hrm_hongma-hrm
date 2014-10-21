USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_ApproveRequest_2]    Script Date: 10/21/2014 11:25:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_2]	
	@RequestID bigint,	
	@EmpId int,
	@ApprovedBy int,
	@ApprovedDate datetime
AS
/*
Created: 2012/04/18@Dan
Task: approve check-out request form (type 2)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- get request check-in time
DECLARE @CheckoutTime datetime
DECLARE @RequestDate datetime

SELECT @CheckoutTime = FromDate,
	@RequestDate = RequestDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
	-- update the working calendar to record the check-in time
	UPDATE dbo.EmployeeCalendar
	SET RequestedCheckoutTime = @CheckoutTime,
		ModifiedDate = @ApprovedDate,
		ModifiedBy = @ApprovedBy 
	WHERE EmpID = @EmpId and datediff(d, WorkDate, @RequestDate)=0
	
	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 2, -- approved
		ProcessedBy= @ApprovedBy,
		ProcessedDate= @ApprovedDate,
		ModifiedDate = @ApprovedDate,
		ModifiedBy = @ApprovedBy 
	WHERE Id = @RequestID
	
	INSERT INTO dbo.EmployeeCalendar_Request(EmpId, WorkDate, RequestId)
	SELECT EmpId, WorkDate, @RequestID FROM dbo.EmployeeCalendar WITH(NOLOCK) 
	WHERE EmpId = @EmpId 
	AND DATEDIFF(d, WorkDate, @RequestDate)=0

	-- process the calendar into attendance
	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @RequestDate, @RequestDate, 1

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
 
