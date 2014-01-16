USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_GetRequestByEmp_ChangeShiftType]    Script Date: 8/27/2012 2:56:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- [WorkingTimeRequest_GetRequestByEmp_ChangeShiftType] null,null,'2012/05/1', '2012/05/22', null,165, 1

ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByEmp_ChangeShiftType]
	@empid int = null,
	@groupid int = null,
	@fromdate datetime,
	@todate datetime,
	@status tinyint = null,
	@excludedEmpID int= null,
	@role int
AS
/*
Created: 2012/05/23@Dan
Task: get change shift request list
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
	FromDate AS RequestDate,	
	s1.Code as FromShift,
	s2.Code as ToShift,
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
	(SELECT count(*) FROM dbo.WorkingTimeRequest wt WITH(NOLOCK) WHERE wt.EmpId = r.EmpId 
	and wt.RequestType = 5 and wt.Status = 2 
	and month(wt.FromDate) = month(r.FromDate)
	and year(wt.FromDate) = year(r.FromDate)
	) AS TotalDaysRequestInMonth
	
FROM  dbo.WorkingTimeRequest r WITH (NOLOCK) 
	LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy	
	LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id= FromShift
	LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id= ToShift
	LEFT JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
	LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
WHERE RequestType = 5 and
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

