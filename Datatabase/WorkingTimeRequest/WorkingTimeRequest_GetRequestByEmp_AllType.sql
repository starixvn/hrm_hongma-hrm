USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_GetRequestByEmp_AllType]    Script Date: 10/21/2014 12:42:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- [WorkingTimeRequest_GetRequestByEmp_AllType] null,null,'2012/05/1', '2012/06/22', null,165, 1

ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByEmp_AllType]
	@empid int = null,
	@groupids varchar(max) = '',
	@fromdate datetime,
	@todate datetime,
	@status tinyint = null,
	@excludedEmpID int= null,
	@role int
AS
/*
Created: 2012/06/20@Dan
Task: get all request list
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
CREATE TABLE #GroupsList (GroupId TINYINT)
INSERT INTO #GroupsList (GroupId)
SELECT val FROM fnc_strSplit (@groupids, ',')

set @fromdate = convert(varchar(10),@fromdate,111)
set @todate = dateadd(d, 1, convert(varchar(10),@todate,111))

SELECT 
	r.Id,
	r.empID,
	RequestType,
	r.RequestDate,		
	r.CreatedDate,
	Status,	
	e.NickName AS Requester,
	e.FullName AS FullNameRequester,
	g.Name AS Team,
	g.Code AS TeamCode,
	ap.NickName AS ProcessedBy,
	ap.FullName AS FullNameProcessedBy,
	ProcessedDate,
	(CASE 
	WHEN r.RequestType IN (1,2) THEN
		(SELECT count(*) FROM dbo.WorkingTimeRequest wt WITH (NOLOCK) WHERE wt.EmpId = r.EmpId 
		AND wt.RequestType in (1,2) AND wt.Status = 2 AND
		wt.FromDate between DATEADD(dd,-(DAY(r.FromDate)-1),r.FromDate)
		and DATEADD(dd,-(DAY(DATEADD(mm,1,r.FromDate))-1),DATEADD(mm,1,r.FromDate))) 
		--month(wt.FromDate) = month(r.FromDate) 
		--AND year(wt.FromDate) = year(r.FromDate))
	WHEN r.RequestType in (3,4) THEN
		(SELECT count(*) FROM dbo.WorkingTimeRequest wt WITH (NOLOCK) WHERE wt.EmpId = r.EmpId 
		AND wt.RequestType in (3,4) AND wt.Status = 2 AND 
		wt.FromDate between DATEADD(dd,-(DAY(r.FromDate)-1),r.FromDate)
		and DATEADD(dd,-(DAY(DATEADD(mm,1,r.FromDate))-1),DATEADD(mm,1,r.FromDate)))
		
	WHEN r.RequestType =5 THEN
		(SELECT count(*) FROM dbo.WorkingTimeRequest wt WITH (NOLOCK) WHERE wt.EmpId = r.EmpId 
		AND wt.RequestType =5 AND wt.Status = 2 AND 
		wt.FromDate between DATEADD(dd,-(DAY(r.FromDate)-1),r.FromDate)
		and DATEADD(dd,-(DAY(DATEADD(mm,1,r.FromDate))-1),DATEADD(mm,1,r.FromDate)))
	
	WHEN r.RequestType =6 THEN
		(SELECT sum(wt.TotalDays) FROM dbo.WorkingTimeRequest wt WITH (NOLOCK) WHERE wt.EmpId = r.EmpId 
		AND wt.RequestType = 6 AND wt.Status = 2 AND 
		wt.FromDate between DATEADD(dd,-(DAY(r.FromDate)-1),r.FromDate)
		and DATEADD(dd,-(DAY(DATEADD(mm,1,r.FromDate))-1),DATEADD(mm,1,r.FromDate)))
	ELSE
		(SELECT count(*) FROM dbo.WorkingTimeRequest wt WITH (NOLOCK) WHERE wt.EmpId = r.EmpId 
		AND wt.RequestType =7 AND wt.Status = 2 AND 
		wt.FromDate between DATEADD(dd,-(DAY(r.FromDate)-1),r.FromDate)
		and DATEADD(dd,-(DAY(DATEADD(mm,1,r.FromDate))-1),DATEADD(mm,1,r.FromDate)))
    END)as TotalDaysRequestInMonth,
	(CASE
	WHEN r.RequestType in (5, 7) THEN	
		s1.Code
	ELSE
		s3.Code
	END	
	) as FromShift,

	s2.Code as ToShift,
	case when r.RequestType = 3 then r.LateInMins else r.EarlyOutMins end as [Time],
	r.FromDate,
	r.ToDate,
	r.TotalDays,
	lv.Name as LeaveType,
	r.Remark,
	r.OvertimeType,
	r.IsOvertimeHours 	

FROM  dbo.WorkingTimeRequest r WITH (NOLOCK) 
	LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy	
	JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),r.FromDate,111)
	LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id= r.FromShift
	LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id= r.ToShift
	LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
	LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
	LEFT JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
	LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
WHERE 
	(@empid is null or r.empid = @empid) and 
	(e.isDeleted is null or e.isDeleted = 0) and
	(@excludedEmpID is null or r.empid != @excludedEmpID) and 
	(@status is null or r.status = @status ) and
	r.createddate >= @fromdate and r.createddate  < @todate	
	and 
	(
		((@groupids IS NULL OR @groupids = '') and @role NOT IN (1,2)) or
		((@groupids IS NULL OR @groupids = '') and @role IN (1,2)  	
					AND r.empID IN (SELECT empid
								from  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
								where eg.GroupId IN (SELECT Id
													FROM dbo.EmployeeGroup WITH (NOLOCK)
													WHERE LeaderId = @excludedEmpID OR	
														AssistantLeaderId = @excludedEmpID
													)
										AND ISNULL(HandleRequest,0) = 1			
									)) or

		(@groupids IS NOT NULL AND @groupids != '' and r.empID IN (SELECT empid
								from  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
								where eg.GroupId IN (SELECT GroupId FROM #GroupsList)
								AND ISNULL(HandleRequest,0) = 1
								))
		)
ORDER BY r.CreatedDate DESC

DROP TABLE #GroupsList
