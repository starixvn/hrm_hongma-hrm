-- AnnualLeave_InsertMultipleTransactions '86,87', 'test', '2012/05/28', 0,1, '2012/05/28', 86 
-- select * from employee where id in ('86','87')

ALTER PROCEDURE [dbo].[AnnualLeave_InsertMultipleTransactions]
	@EmpIds varchar(4000) = null, 	
	@Description nvarchar(4000),
	@FromDate datetime,
	@DeductDays numeric(18,1),
	@CreditDays numeric(18,1),
	@CreatedDate datetime,
	@CreatedBy int
AS
/*
Created: 2012/04/17@Dan
Task: record multiple annual leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- SET XACT_ABORT ON will cause the transaction to be uncommittable
-- when the constraint violation occurs. 
SET XACT_ABORT ON;

DECLARE	@Emp TABLE(ID int) 

INSERT INTO @Emp (ID)
SELECT val 
FROM dbo.[fnc_strSplit](@EmpIDs,',')

BEGIN TRY
    BEGIN TRANSACTION;
    INSERT INTO dbo.LeaveTrans(EmpId, Description, FromDate, DeductDays,CreditDays, CreatedDate, CreatedBy)	
	SELECT ID,  @Description, @FromDate, @DeductDays,@CreditDays, @CreatedDate, @CreatedBy
	from Employee 
	WHERE ((@EmpIds IS NULL AND ISNULL(isdeleted,0)=0) OR  
			Id in (select id from @Emp))
 
	UPDATE dbo.Employee 
	SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance,0) + ISNULL(@CreditDays,0) - ISNULL(@DeductDays,0)
	WHERE Id in (select id from @Emp)
	           
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

