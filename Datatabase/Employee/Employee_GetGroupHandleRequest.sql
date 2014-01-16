-- Employee_GetGroupHandleRequest 165

CREATE PROCEDURE [dbo].[Employee_GetGroupHandleRequest]	
	@id int
AS
/*
Created: 2012/05/22@Dan
Task: get group that handle request of an employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	

	SELECT g.*
	FROM dbo.EmployeeGroup_Employee eg 
			LEFT JOIN dbo.EmployeeGroup g ON eg.Groupid=g.ID 					
	WHERE eg.EmpID=@id AND ISNULL(HandleRequest,0) = 1		
END

