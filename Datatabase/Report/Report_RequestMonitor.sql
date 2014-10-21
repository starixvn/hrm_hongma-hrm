USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[Report_RequestMonitor]    Script Date: 10/21/2014 11:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
--Report_RequestMonitor '2012/5/1', '2012/5/25',2
*/

ALTER PROCEDURE [dbo].[Report_RequestMonitor]
	@fromDate datetime, 
	@toDate datetime,
	@maxRequest INT,
	@role INT,
	@employeeID INT
AS
/*
Created: 2012/05/03@Dan
Task: show dashboard info for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

SELECT 
	e.FullName, 
	e.NickName,
	g.Name as Team,
	g.Id as TeamId,
	r.*
FROM
	(

	SELECT EmpID,
		ISNULL(SUM(case when requesttype IN (1,2) then 1 else 0 end ),0) as TotalCheckInOutRequest,	
		ISNULL(SUM(case when requesttype IN (3,4) then 1 else 0 end),0) as TotalLateInEarlyOutRequest,		
		ISNULL(SUM(case when requesttype = 5 then 1 else 0 end),0) as TotalChangeShiftRequest,	
		ISNULL(SUM(case when requesttype = 6  then 1 else 0 end),0) as TotalLeaveRequest,
		ISNULL(SUM(case when requesttype = 7  then 1 else 0 end),0) as TotalOvertimeRequest		
	FROM dbo.WorkingTimeRequest r WITH(NOLOCK)
		JOIN Employee e ON e.Id = r.empid 
	WHERE r.CreatedDate >= @fromdate AND r.CreatedDate < @toDate	
		AND status = 2 
AND (
	(@role NOT IN (1,2)) 
	OR
	(@role IN (1,2)  AND r.empID IN (SELECT empid
								from  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
								where eg.GroupId IN (SELECT Id
													FROM dbo.EmployeeGroup WITH (NOLOCK)
													WHERE LeaderId = @employeeID OR	
														AssistantLeaderId = @employeeID
													)
										AND eg.EmpID <> @employeeID
									))
	)

	GROUP BY EMPID
	) r JOIN Employee e WITH (NOLOCK) ON e.ID= r.EmpID	
	LEFT JOIN dbo.EmployeeGroup G WITH (NOLOCK) on g.id = e.GroupID
WHERE (TotalCheckInOutRequest >= @maxRequest OR
	TotalLateInEarlyOutRequest >= @maxRequest OR
	TotalChangeShiftRequest >= @maxRequest OR
	TotalLeaveRequest >= @maxRequest OR
	TotalOvertimeRequest >= @maxRequest)
	AND (e.isDeleted is null or e.isDeleted = 0) 


