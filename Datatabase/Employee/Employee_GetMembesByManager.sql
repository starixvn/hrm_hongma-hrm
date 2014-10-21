USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetMembesByManager]    Script Date: 10/21/2014 10:52:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Employee_GetMembesByManager 165, 3,0,NULL, N'Quang'
ALTER PROCEDURE [dbo].[Employee_GetMembesByManager] 
	@managerID int,
	@role int,
	@excludedManager bit,
	@groupID int =NULL,
	@keyword nvarchar(100)
AS
/*
Created: 2012/05/24@Dan
Task: Get list of employees for a leader/manager
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @searchText nvarchar(100)
	
	IF (ISNULL(@keyword,'')<> '') 
		SET @searchText = '%' + @keyword + '%'
	print @searchText
    -- Insert statements for procedure here
	SELECT *
	FROM Employee e WITH (NOLOCK)
	WHERE 
		(ISNULL(@excludedManager,0)=0 or id <> @managerID) and
		((@groupid is null and @role >=3 ) or
		(@groupid is null and @role IN (1,2)  	
					AND ID IN (SELECT empid
								from  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
								where  eg.GroupId IN (SELECT Id
													FROM dbo.EmployeeGroup WITH (NOLOCK)
													WHERE LeaderId = @managerID OR	
														AssistantLeaderId = @managerID
													)
										AND ISNULL(HandleRequest,0) = 1
									)) 
		or
		(@groupid is not null and ID IN (SELECT empid
								from  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
								where eg.GroupId = @groupid AND ISNULL(HandleRequest,0) = 1
								)
		))		
		AND (@searchText is null OR FullName LIKE @searchText OR 
			NickName LIKE @searchText)		
	ORDER BY [FullName]
END

