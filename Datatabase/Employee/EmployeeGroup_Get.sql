USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeGroup_Get]    Script Date: 10/21/2014 11:04:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EmployeeGroup_Get] 
	@FromDate DATETIME = NULL
AS
/*
Created: 20140320@Quinn.Vo
Task: Get all groups from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
	g.Id AS TeamId,
	g.DepartmentId,
	g.Code,
	g.Name,
	ISNULL(g.Deleted, 0) AS IsDeleted,
	CASE WHEN g.ModifiedDate IS NULL THEN g.CreatedDate ELSE g.ModifiedDate END AS ModifiedDate
	FROM dbo.EmployeeGroup AS g WITH (NOLOCK)
	WHERE @FromDate IS NULL
	OR g.ModifiedDate > @FromDate
END
