USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_GetRequestByDate_LeaveType]    Script Date: 10/21/2014 12:41:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByDate_LeaveType]
	@empid int = null,
	@groupids varchar(max) = '',
	@fromdate datetime,
	@todate datetime,
	@status tinyint = null,
	@excludedEmpID int= null,
	@role int
AS

CREATE TABLE #GroupsList (GroupId TINYINT)
INSERT INTO #GroupsList (GroupId)
SELECT val FROM fnc_strSplit (@groupids, ',')

SET @fromdate = CONVERT(varchar(10),@fromdate,111)
SET @todate = DATEADD(d, 1, CONVERT(varchar(10),@todate,111))
SET @todate = DATEADD(ms,-2,@todate) -- end of date (23:59:59.999)

SELECT 
	r.Id,
	RequestType,			
	FromDate,
	ToDate,
	TotalDays, 
	lv.Name as LeaveType,
	Remark,
	r.CreatedDate,
	Status,	
	e.NickName AS Requester,
	e.FullName AS FullNameRequester,
	g.Name AS Team,
	g.Code AS TeamCode,
	ap.NickName AS ProcessedBy,
	ap.FullName AS FullNameProcessedBy,
	ProcessedDate,
	(SELECT sum(wt.TotalDays) FROM dbo.WorkingTimeRequest wt WITH (NOLOCK) WHERE wt.EmpId = r.EmpId 
	and wt.RequestType = 6 and wt.Status = 2 and month(wt.FromDate) = month(r.FromDate)) AS TotalDaysRequestInMonth	

FROM  dbo.WorkingTimeRequest r WITH (NOLOCK) 
	LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
	JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
	JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
	LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
WHERE RequestType = 6 and
	(@empid is null or r.empid = @empid) and 
	(e.isDeleted is null or e.isDeleted = 0) and
	(@excludedEmpID is null or r.empid != @excludedEmpID) and 
	(@status is null or r.status = @status )
	and (
		((@groupids IS NULL OR @groupids = '')  and @role NOT IN (1,2)) or
		((@groupids IS NULL OR @groupids = '')  and @role IN (1,2)  	
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
	AND dbo.fnc_WorkingTimeRequest_GetRequestByDate_IsOutOfRange(r.FromDate,r.ToDate,@fromdate,@todate)=0
ORDER BY r.CreatedDate DESC

DROP TABLE #GroupsList
