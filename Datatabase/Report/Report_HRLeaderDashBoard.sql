/*
--Report_HRLeaderDashBoard 150, '2012/5/15'
*/

ALTER PROCEDURE Report_HRLeaderDashBoard
	@employeeid int = null,
	@date datetime
AS
/*
Created: 2012/05/03@Dan
Task: show dashboard info for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @fromDate datetime, @toDate datetime, @role int, @roles varchar(20)

SET @date = CONVERT(VARCHAR(10),@date,111)
SET @fromDate = dateadd(d,-7,@date)
SET @toDate = dateadd(d,1,@date)

SELECT @roles=roles
FROM Employee e WITH (NOLOCK)
WHERE ID = @employeeid

SELECT @role = MAX(cast(val as int))
FROM dbo.fnc_strSplit(@roles, ',')

if (ISNULL(@role,0)=0)
	return;

SELECT 
	-- check in/out request	
	ISNULL(SUM(case when requesttype in (1,2) and status=1 then 1 else 0 end),0) as TotalPendingCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (1,2) and status=2 then 1 else 0 end),0) as TotalApprovedCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (1,2) and status=3 then 1 else 0 end),0) as TotalRejectCheckInOutRequest,

	-- late in/early out request	
	ISNULL(SUM(case when requesttype in (3,4) and status=1 then 1 else 0 end),0) as TotalPendingLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and status=2 then 1 else 0 end),0) as TotalApprovedLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and status=3 then 1 else 0 end),0) as TotalRejectLateInEarlyOutRequest,

	-- Change Shift Request	
	ISNULL(SUM(case when requesttype = 5 and status=1 then 1 else 0 end),0) as TotalPendingChangeShiftRequest,
	ISNULL(SUM(case when requesttype = 5 and status=1 then 2 else 0 end),0) as TotalApprovedChangeShiftRequest,
	ISNULL(SUM(case when requesttype = 5 and status=1 then 3 else 0 end),0) as TotalRejectChangeShiftRequest,
	
	--Leave Request	
	ISNULL(SUM(case when requesttype = 6 and status=1 then 1 else 0 end),0) as TotalPendingLeaveRequest,
	ISNULL(SUM(case when requesttype = 6 and status=2 then 1 else 0 end),0) as TotalApprovedLeaveRequest,
	ISNULL(SUM(case when requesttype = 6 and status=3 then 1 else 0 end),0) as TotalRejectLeaveRequest

FROM dbo.WorkingTimeRequest r
	JOIN Employee e ON e.Id = r.empid 
WHERE r.CreatedDate >= @fromdate and r.CreatedDate < @toDate	
	AND (--@role>=3 or 
				( r.empid <> @employeeid -- not count the leader itself 
						AND r.empID IN (SELECT empid
								from  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
								where eg.GroupId IN (SELECT Id
													FROM dbo.EmployeeGroup WITH (NOLOCK)
													WHERE LeaderId = @employeeid OR	
															AssistantLeaderId = @employeeid
												)
									AND ISNULL(HandleRequest,0) = 1			
									)
				)
		)