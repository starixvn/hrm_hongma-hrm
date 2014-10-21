USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[Shift_Get]    Script Date: 10/21/2014 11:22:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Shift_Get] 
AS
/*
Created: 20141002@Quinn.Vo
Task: Get all shifts
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT Id AS ShiftId
	,Code
	,Name
	,BeginCheckInTime
	,EndCheckInTime
	,BeginCheckOutTime
	,EndCheckOutTime
	,0 IsInAfterDay
	,(CASE WHEN BeginCheckInTime < BeginCheckOutTime THEN 0 ELSE 1 END) AS IsOutAfterDay
	FROM dbo.[Shift]
END