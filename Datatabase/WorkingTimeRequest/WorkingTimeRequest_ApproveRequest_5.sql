USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_ApproveRequest_5]    Script Date: 10/21/2014 11:26:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_5]
	@RequestID bigint,	
	@EmpId int,
	@ApprovedBy int,
	@ApprovedDate datetime
AS
/*
Created: 2012/04/18@Dan
Task: approve changing working shift request form (type 5)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- get all request detail 
DECLARE @ChangeShiftID int, @WorkDate datetime, @FromShift int, @ToShift int
	
SELECT @WorkDate = FromDate, @FromShift = FromShift,  @ToShift = ToShift, 
		@ChangeShiftID = Id
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
	-- update the working calendar to record the check-in time
	UPDATE dbo.EmployeeCalendar
	SET  ChangeShiftID = @ChangeShiftID,
		ShiftId = @ToShift,
		ModifiedDate = @ApprovedDate,
		ModifiedBy = @ApprovedBy
	WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0

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
	AND DATEDIFF(d, WorkDate, @WorkDate)=0

	-- process the calendar into attendance
	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1
	
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