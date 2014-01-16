-- [WorkingTimeRequest_GetRequestHistory] 310,0 

ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestHistory]
	@requestid bigint,
	@type int	
AS
/*
Created: 2012/07/09@Dan
Task: view request history
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
	@Type: 0 -- view all; 1: chckin/out type; 2: latein/early out; 3: change shift; 4: leave
*/

-- view all
IF (@type =0) 
BEGIN 
	SELECT 
		r.Id,
		RequestType,		
		r.CreatedDate,
		Status,	
		e.NickName AS Requester,
		e.FullName AS FullNameRequester,
		g.Name AS Team,
		g.Code AS TeamCode,
		ap.NickName AS ProcessedBy,
		ap.FullName AS FullNameProcessedBy,
		ProcessedDate	
	FROM  dbo.WorkingTimeRequest_history r WITH (NOLOCK) 
		LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy	
		LEFT JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
		LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE r.Id = @requestid
	ORDER BY keyid
END
ELSE IF (@type =1) -- view CheckinOutType
BEGIN
	SELECT 
		r.Id,
		RequestType,
		s.Code AS Shift,
		FromDate AS RequestTime,	
		r.CreatedDate,
		Status,	
		e.NickName AS Requester,
		e.FullName AS FullNameRequester,
		g.Name AS Team,
		g.Code AS TeamCode,
		ap.NickName AS ProcessedBy,
		ap.FullName AS FullNameProcessedBy,
		ProcessedDate
	FROM  dbo.WorkingTimeRequest_history r WITH (NOLOCK)
		LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy	
		JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),r.FromDate,111)
		JOIN Shift s WITH (NOLOCK) ON s.id = c.ShiftID
		JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
		LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE r.Id = @requestid
	ORDER BY keyid
END
ELSE IF (@type =2) -- view LateInEarlyOutType
BEGIN
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
		ProcessedDate
	FROM  dbo.WorkingTimeRequest_history r WITH (NOLOCK) 
		LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy	
		JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),r.FromDate,111)
		LEFT JOIN Shift s WITH (NOLOCK) ON s.id = c.ShiftID
		JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
		LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE r.Id = @requestid
	ORDER BY keyid
END
ELSE IF (@type = 3) -- view ChangeShiftType
BEGIN
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
		ProcessedDate	
	FROM  dbo.WorkingTimeRequest_history r WITH (NOLOCK) 
		LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy	
		LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id= FromShift
		LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id= ToShift
		LEFT JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
		LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE r.Id = @requestid
	ORDER BY keyid
END
ELSE IF (@type = 4) -- view LeaveType
	BEGIN
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
		ProcessedDate	
	FROM  dbo.WorkingTimeRequest_history r WITH (NOLOCK) 
		LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
		JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
		JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID	
		LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE r.Id = @requestid
	ORDER BY keyid
END