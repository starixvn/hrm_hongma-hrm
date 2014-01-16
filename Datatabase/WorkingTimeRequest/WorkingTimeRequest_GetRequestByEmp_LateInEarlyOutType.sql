USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_GetRequestByEmp_LateInEarlyOutType]    Script Date: 8/27/2012 11:24:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--WorkingTimeRequest_GetRequestByEmp_LateInEarlyOutType NULL,null,'2012/05/1', '2012/07/22', null,NULL, 3

ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByEmp_LateInEarlyOutType]	
	@empid int = null,
	@groupid int = null,
	@fromdate datetime,
	@todate datetime,
	@status tinyint = null,
	@excludedEmpID int= null,
	@role int
AS
/*
Created: 2012/04/27@Dan
Task: get late in/early out request list
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

set @fromdate = convert(varchar(10),@fromdate,111)
set @todate = dateadd(d, 1, convert(varchar(10),@todate,111))

SELECT 
	r.Id,
	RequestType,
	s.Code AS Shift,
	FromDate AS RequestDate,
	case when RequestType = 3 then LateInMins else EarlyOutMins end as [Time],	
	Remark,
	r.CreatedDate,
	Status,	
	e.NickName AS Requester,
	e.FullName AS FullNameRequester,
	g.Name AS Team,	
	g.Code AS TeamCode,
	ap.NickName as ProcessedBy,
	ap.FullName as FullNameProcessedBy,
	ProcessedDate,
	(SELECT count(*) FROM dbo.WorkingTimeRequest wt WITH (NOLOCK) WHERE wt.EmpId = r.EmpId 
	and wt.RequestType in (3,4) and wt.Status = 2 and month(wt.FromDate) = month(r.FromDate)) AS TotalDaysRequestInMonth	

FROM  dbo.WorkingTimeRequest r WITH (NOLOCK) 
	LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy	
	JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),r.FromDate,111)
	LEFT JOIN Shift s WITH (NOLOCK) ON s.id = c.ShiftID
	JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
	LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
WHERE RequestType in (3,4) and
	(@empid is null or r.empid = @empid) and 
	(e.isDeleted is null or e.isDeleted = 0) and
	(@excludedEmpID is null or r.empid != @excludedEmpID) and 
	(@status is null or r.status = @status ) and
	r.createddate >= @fromdate and r.createddate  < @todate	
	and (
		(@groupid is null and @role NOT IN (1,2)) or
		(@groupid is null and @role IN (1,2)  	
					AND r.empID IN (SELECT empid
								from  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
								where eg.GroupId IN (SELECT Id
													FROM dbo.EmployeeGroup WITH (NOLOCK)
													WHERE LeaderId = @excludedEmpID OR	
														AssistantLeaderId = @excludedEmpID
													)
										AND ISNULL(HandleRequest,0) = 1
									)) or

		(@groupid is not null and r.empID IN (SELECT empid
								from  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
								where eg.GroupId = @groupid AND ISNULL(HandleRequest,0) = 1
								))
		) 
ORDER BY r.CreatedDate DESC

