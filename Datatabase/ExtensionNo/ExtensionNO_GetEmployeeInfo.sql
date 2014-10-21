USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[ExtensionNO_GetEmployeeInfo]    Script Date: 10/21/2014 11:10:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

ALTER PROCEDURE [dbo].[ExtensionNO_GetEmployeeInfo]
	@isInternal BIT = NULL,
	@EmpExtID INT = NULL
AS
/*
Created: Tom.Vo@2014-01-04
Task: Get list Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON
IF @isInternal IS NULL
	SELECT  exg.[ExtID]
		  ,COALESCE(g.Id,exg.[ExtID]) TeamID		
		  ,ISNULL(g.Code,'') Team
		  ,COALESCE(g.Name,exg.[FullName]) TeamName
		  ,COALESCE(ee.Id,exm_1.EmpID,NULL) EmpID
		  ,COALESCE(ee.FullName,exm_1.FullName,exm_2.FullName) FullName
		  ,COALESCE(ee.NickName,exm_1.NickName,exm_2.NickName) NickName
		  ,COALESCE(exm_1.ExtensionNo,exm_2.ExtensionNo) [ExtensionNo]
		  ,COALESCE(ee.Id,exm_1.ExtID,exm_2.ExtID) EmpExtID
		  ,exg.IsInternal
		  ,exg.[OrderNo] [OrderNoGroup]
		  ,ROW_NUMBER() OVER(ORDER BY exg.[ExtID]) AS [OrderNoMember]
	  FROM [dbo].[ExtensionNumbers] exg WITH(NOLOCK)
		LEFT JOIN dbo.EmployeeGroup g WITH(NOLOCK)
			ON exg.GroupID = g.Id AND exg.IsInternal = 1 AND exg.IsGroup =1
		LEFT JOIN [dbo].[ExtensionNumbers] exm_1 WITH(NOLOCK)
			ON exg.GroupID = exm_1.GroupID AND exg.IsInternal = 1 AND exm_1.IsGroup =0 AND exg.IsGroup =1
		LEFT JOIN [dbo].Employee ee WITH(NOLOCK)
			ON exm_1.EmpID = ee.Id 
		LEFT JOIN [dbo].[ExtensionNumbers] exm_2 WITH(NOLOCK)
			ON exg.ExtID = exm_2.GroupID AND exg.IsInternal = 0 AND exm_2.IsGroup =0 AND exg.IsGroup =1
	 WHERE COALESCE(ee.Id,exm_1.EmpID,exm_2.EmpID,-1)> 0 OR (exg.IsInternal = 0 AND  exg.IsGroup =1)
	 ORDER BY [OrderNoGroup],[OrderNoMember]
			 
 ELSE IF @EmpExtID IS NULL AND @isInternal = 0 
	SELECT  exg.[ExtID]
		  ,exg.[ExtID] TeamID		
		  ,'' Team
		  ,exg.[FullName] TeamName
		  ,NULL EmpID
		  ,exm_2.FullName FullName
		  ,exm_2.NickName NickName
		  ,exm_2.ExtensionNo [ExtensionNo]
		  ,exm_2.ExtID EmpExtID
		  ,exg.IsInternal
		  ,exg.[OrderNo] [OrderNoGroup]
		  ,ROW_NUMBER() OVER(ORDER BY exg.[ExtID]) AS [OrderNoMember]
	  FROM [dbo].[ExtensionNumbers] exg WITH(NOLOCK)
		LEFT JOIN [dbo].[ExtensionNumbers] exm_2 WITH(NOLOCK)
			ON exg.ExtID = exm_2.GroupID AND exg.IsInternal = 0 AND exm_2.IsGroup =0 AND exg.IsGroup =1
	 WHERE (exg.IsInternal = 0 AND  exg.IsGroup =1)
	 ORDER BY [OrderNoGroup],[OrderNoMember]
			 
 ELSE IF @EmpExtID IS NULL AND @isInternal = 1 
	SELECT  exg.[ExtID]
		  ,COALESCE(g.Id,exg.[ExtID]) TeamID		
		  ,ISNULL(g.Code,'') Team
		  ,COALESCE(g.Name,exg.[FullName]) TeamName
		  ,COALESCE(ee.Id,exm_1.EmpID) EmpID
		  ,COALESCE(ee.FullName,exm_1.FullName) FullName
		  ,COALESCE(ee.NickName,exm_1.NickName) NickName
		  ,exm_1.ExtensionNo [ExtensionNo]
		  ,exm_1.ExtID EmpExtID
		  ,exg.IsInternal
		  ,exg.[OrderNo] [OrderNoGroup]
		  ,CAST (ROW_NUMBER() OVER(ORDER BY exg.[ExtID]) AS INT) AS [OrderNoMember]
	  FROM [dbo].[ExtensionNumbers] exg WITH(NOLOCK)
		LEFT JOIN dbo.EmployeeGroup g WITH(NOLOCK)
			ON exg.GroupID = g.Id AND exg.IsInternal = 1 AND exg.IsGroup =1
		LEFT JOIN [dbo].[ExtensionNumbers] exm_1 WITH(NOLOCK)
			ON exg.GroupID = exm_1.GroupID AND exg.IsInternal = 1 AND exm_1.IsGroup =0 AND exg.IsGroup =1
		LEFT JOIN [dbo].Employee ee WITH(NOLOCK)
			ON exm_1.EmpID = ee.Id 
	 WHERE COALESCE(ee.Id,exm_1.EmpID,-1)> 0
	 ORDER BY [OrderNoGroup],[OrderNoMember]
	 			 
			 
 ELSE IF @EmpExtID IS NOT NULL AND @isInternal = 0 
	SELECT  exg.[ExtID]
		  ,exg.[ExtID] TeamID		
		  ,'' Team
		  ,exg.[FullName] TeamName
		  ,NULL EmpID
		  ,exm_2.FullName FullName
		  ,exm_2.NickName NickName
		  ,exm_2.ExtensionNo [ExtensionNo]
		  ,exm_2.ExtID EmpExtID
		  ,exg.IsInternal
		  ,exg.[OrderNo] [OrderNoGroup]
		  ,CAST (ROW_NUMBER() OVER(ORDER BY exg.[ExtID]) AS INT) AS [OrderNoMember]
	  FROM [dbo].[ExtensionNumbers] exg WITH(NOLOCK)
		LEFT JOIN [dbo].[ExtensionNumbers] exm_2 WITH(NOLOCK)
			ON exg.ExtID = exm_2.GroupID AND exg.IsInternal = 0 AND exm_2.IsGroup =0 AND exg.IsGroup =1
	 WHERE (exg.IsInternal = 0 AND  exg.IsGroup =1) AND exm_2.[ExtID] = @EmpExtID
	 ORDER BY [OrderNoGroup],[OrderNoMember]
			 
 ELSE IF @EmpExtID IS NOT NULL AND @isInternal = 1 
	SELECT  exg.[ExtID]
		  ,COALESCE(g.Id,exg.[ExtID]) TeamID		
		  ,ISNULL(g.Code,'') Team
		  ,COALESCE(g.Name,exg.[FullName]) TeamName
		  ,COALESCE(ee.Id,exm_1.EmpID) EmpID
		  ,COALESCE(ee.FullName,exm_1.FullName) FullName
		  ,COALESCE(ee.NickName,exm_1.NickName) NickName
		  ,exm_1.ExtensionNo [ExtensionNo]
		  ,exm_1.ExtID EmpExtID
		  ,exg.IsInternal
		  ,exg.[OrderNo] [OrderNoGroup]
		  ,CAST (ROW_NUMBER() OVER(ORDER BY exg.[ExtID]) AS INT) AS [OrderNoMember]
	  FROM [dbo].[ExtensionNumbers] exg WITH(NOLOCK)
		LEFT JOIN dbo.EmployeeGroup g WITH(NOLOCK)
			ON exg.GroupID = g.Id AND exg.IsInternal = 1 AND exg.IsGroup =1
		LEFT JOIN [dbo].[ExtensionNumbers] exm_1 WITH(NOLOCK)
			ON exg.GroupID = exm_1.GroupID AND exg.IsInternal = 1 AND exm_1.IsGroup =0 AND exg.IsGroup =1
		LEFT JOIN [dbo].Employee ee WITH(NOLOCK)
			ON exm_1.EmpID = ee.Id 
	 WHERE COALESCE(ee.Id,exm_1.EmpID,-1)> 0 AND exm_1.[ExtID]= @EmpExtID
	 ORDER BY [OrderNoGroup],[OrderNoMember]
	 			 