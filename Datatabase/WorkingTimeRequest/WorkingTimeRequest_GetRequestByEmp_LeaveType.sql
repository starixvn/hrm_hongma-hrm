USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_GetRequestByEmp_LeaveType]    Script Date: 8/27/2012 11:24:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- [WorkingTimeRequest_GetRequestByEmp_LeaveType] 106,1,'2012/06/1', '2012/06/30', 2,86, 3

ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByEmp_LeaveType]
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
Task: get leave request list
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



