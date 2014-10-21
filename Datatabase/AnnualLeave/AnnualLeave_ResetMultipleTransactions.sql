USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[AnnualLeave_ResetMultipleTransactions]    Script Date: 10/21/2014 10:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- AnnualLeave_InsertMultipleTransactions '86,87', 'test', '2012/05/28', 0,1, '2012/05/28', 86 
-- select * from employee where id in ('86','87')

ALTER PROCEDURE [dbo].[AnnualLeave_ResetMultipleTransactions]	
	@EmpValue varchar(4000) = null, 	
	@Description nvarchar(4000),
	@FromDate datetime,
	@ResetDays numeric(18,1),
	@CreatedDate datetime,
	@CreatedBy int
AS
/*
Created: 2012/12/24@Edward
Task: reset anual balance
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
	@EmpValue:
		<ROOT><Employee Id="1" LeaveBalance="2"/></ROOT>
*/

-- SET XACT_ABORT ON will cause the transaction to be uncommittable
-- when the constraint violation occurs. 
SET XACT_ABORT ON;

DECLARE @xml XML
DECLARE @Emp TABLE(ID int,LeaveBalance decimal)

SET @xml = CONVERT(XML, @EmpValue)
INSERT INTO @Emp(ID,LeaveBalance)
SELECT	T.C.value('./@Id', 'int')
		,T.C.value('./@LeaveBalance', 'numeric(18,1)')		
FROM @xml.nodes('/ROOT/Employee') as [T](C) --T: table, C: column

BEGIN TRY
    BEGIN TRANSACTION;
    INSERT INTO dbo.LeaveTrans(EmpId, Description, FromDate, DeductDays,CreditDays, CreatedDate, CreatedBy)	
	SELECT ID,  @Description, @FromDate,LeaveBalance-@ResetDays,0, @CreatedDate, @CreatedBy
	FROM @Emp 
 
	UPDATE e
	SET e.AnnualLeaveBalance = e.AnnualLeaveBalance - (d.LeaveBalance-@ResetDays)
	FROM dbo.Employee AS e
	INNER JOIN @Emp AS d ON e.Id = d.ID
	           
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