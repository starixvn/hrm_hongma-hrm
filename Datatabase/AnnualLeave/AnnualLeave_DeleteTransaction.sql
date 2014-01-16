/*
AnnualLeave_DeleteTransaction 2,'2012/04/20',86
*/

alter PROCEDURE AnnualLeave_DeleteTransaction	
	@Id int	,
	@ModifiedDate datetime,
	@ModifiedBy int
AS

/*
Created: 2012/04/20@Dan
Task: soft delete leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
SET NOCOUNT ON

-- SET XACT_ABORT ON will cause the transaction to be uncommittable
-- when the constraint violation occurs. 
SET XACT_ABORT ON;
DECLARE @OldDeductDays numeric(18,1), @OldCreditDays numeric(18,1) , 
		@EmpID int, @isDeleted bit

SELECT @EmpID = EmpID, @OldDeductDays =  ISNULL(DeductDays,0), @OldCreditDays = ISNULL(CreditDays,0), @isDeleted = Deleted
FROM dbo.LeaveTrans
WHERE Id=@Id

IF (ISNULL(@isDeleted,0)=1)
	RETURN;

BEGIN TRY	
    BEGIN TRANSACTION;	

	UPDATE dbo.LeaveTrans WITH (ROWLOCK, UPDLOCK)
	SET Deleted=1,
		ModifiedDate = @ModifiedDate,
		ModifiedBy = @ModifiedBy
	WHERE Id=@Id
	    
	UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
	SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance,0) + @OldDeductDays - @OldCreditDays
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

