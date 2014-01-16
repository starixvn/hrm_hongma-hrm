USE [NexHR]
GO
/****** Object:  StoredProcedure [dbo].[AnnualLeave_InsertTransaction]    Script Date: 04/20/2012 11:43:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[AnnualLeave_InsertTransaction]
	@EmpId int, 
	@RequestId int,
	@Description nvarchar(4000),
	@FromDate datetime,
	@ToDate datetime,
	@DeductDays numeric(18,1),
	@CreditDays numeric(18,1),
	@CreatedDate datetime,
	@CreatedBy int
AS

/*
Created: 2012/04/17@Dan
Task: record leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/


-- SET XACT_ABORT ON will cause the transaction to be uncommittable
-- when the constraint violation occurs. 
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    INSERT INTO dbo.LeaveTrans(EmpId, RequestId, Description, FromDate, ToDate, DeductDays,CreditDays, CreatedDate, CreatedBy)
	VALUES (@EmpId, @RequestId, @Description, @FromDate, @ToDate, @DeductDays,@CreditDays, @CreatedDate, @CreatedBy)

	UPDATE dbo.Employee 
	SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance,0) + ISNULL(@CreditDays,0) - ISNULL(@DeductDays,0)
	WHERE Id = @EmpId
	           
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

