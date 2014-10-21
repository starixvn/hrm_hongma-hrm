USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetGroupHandleRequest]    Script Date: 10/21/2014 10:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Employee_GetLeaders 165

ALTER PROCEDURE [dbo].[Employee_GetGroupHandleRequest]	
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

