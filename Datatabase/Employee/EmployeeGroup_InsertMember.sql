USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeGroup_InsertMember]    Script Date: 10/21/2014 11:06:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[EmployeeGroup_InsertMember]
	@EmpId INT
   ,@GroupId INT
   ,@isMainGroup bit
AS
/*
Created: 2012/06/11@Dan
Task: insert member into group
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	INSERT INTO dbo.EmployeeGroup_Employee(GroupId, EmpID, isMainGroup)
	VALUES (@GroupId, @EmpId, @isMainGroup)

	IF (@isMainGroup=1)
	BEGIN
		UPDATE dbo.EmployeeGroup_Employee
		SET isMainGroup=0
		WHERE EmpID=@EmpID AND GroupId<>@GroupId
	END
END