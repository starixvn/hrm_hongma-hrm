--AnnualLeave_GetTransByEmp 144, 2012,0
CREATE PROCEDURE [dbo].[AnnualLeave_GetTransByEmp]
	-- Add the parameters for the stored procedure here	
	@EmpID int,
	@Year int,
	@excludedRequestTrans bit
AS
/*
Created: 2012/05/29@Dan
Task: show annual leave tranasaction by employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @fromdate datetime, @todate datetime
	set @fromdate = cast(@year as varchar(4)) + '/01/01'
	set @todate=dateadd(yy,1,@fromdate) 
		
    
	SELECT 
		t.ID,
		RequestID,
		FromDate AS AffectedDate,
		ISNULL(CreditDays,0) - ISNULL(DeductDays,0) AS [ToTalDays],
		Description,
		t.CreatedDate		
	FROM dbo.LeaveTrans t JOIN dbo.Employee e WITH (NOLOCK) ON e.ID = t.EmpID	
	WHERE EmpId=@EmpID AND
		fromdate>=@fromdate and fromdate<@todate AND
		ISNULL(Deleted,0)=0 AND
		(@excludedRequestTrans=0 OR RequestId IS NULL) 		
		
END
