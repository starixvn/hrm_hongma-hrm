USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_GetByType]    Script Date: 10/21/2014 10:54:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
declare @fromtime datetime, @totime datetime, @empid int, @groupid int, 
	@type smallint 
--@type: 0-all, 1-miss checkin; 2-miss check out, 3: latein; 4-earlyout, 5- not work


set @fromtime ='2012/07/18'
set @totime ='2012/07/18'
set @type = 0
set @empid= 148
exec dbo.EmployeeAttendance_GetByType @fromtime , @totime , @empid, @groupid, @type
*/


ALTER PROCEDURE [dbo].[EmployeeAttendance_GetByType]  -- '2013-07-26', '2013-07-26', null, null, 1, 0
	@fromtime datetime, 
	@totime datetime, 
	@empid int = null, 	
	@groupids varchar(MAX) = '',
	@excludeUserDelete int = 1, 	
	@type smallint 
AS

/*
Created: 2012/03/17@Dan
Task: get attentance list of employees by type
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
	@type: 0-all, 1-missing checkin; 2-missing check out, 3: latein; 4-earlyout, 5- not work
*/


DECLARE @LeaveTypeDefaultId as int , @note varchar(200), @today datetime, @now datetime
SELECT @LeaveTypeDefaultId = dbo.fnc_getLeaveTypeDefault()

CREATE TABLE #GroupsList (GroupId TINYINT)
INSERT INTO #GroupsList (GroupId)
SELECT val FROM fnc_strSplit (@groupids, ',')

set @now = dateadd(hh,7, GETUTCDATE())
set @today = convert(varchar(10),@now,111) -- use UTC because the server use GMT-4

SELECT T.*,
		stuff(
			case when (isLateIn=1 AND LateIn > 15) then (', Late In') else '' end +  
			case when (isEarlyOut=1 AND EarlyOut > 15) then ', Early Out' else '' end   +
			case when isMissingCheckin=1 then ', Missing Check-in' else '' end   +
			case when isMissingCheckOut=1 then ', Missing Check-out' else '' end  + 		
			case when isOffday=1 then ', Off day' else '' end  + 
			case when isNotWork =1 then ', Absent' 
				else case when lv.Name is null or T.LeaveTypeId = @LeaveTypeDefaultId then ''
						else ', ' + lv.Name + case when LeaveDay=0.5 then ' (Half day)' else '' end
					 end
				end, 1,2,'') 	
		as  Notes,
		r.RequestType,
		r.FromDate,
		r.ToDate,
		r.TotalDays,
		r.Remark,
		r.LeaveType,
		r.FromShift,
		r.ToShift,
		r.[Time],
		r.OvertimeType,
		r.IsOvertimeHours,
		r.OvertimeName,
		r.OvertimeFromTime,
		r.OvertimeToTime,
		r.OvertimeTotalHours
from (
	select 
		a.Id, e.Code,		
		a.EmpId,
		FullName,
		NickName,
		g.Name [Team], g.Code [TeamCode], g.[Rank],
		AttendanceDate,
		a.ShiftId,	
		s.code as ShiftCode,
		s.BeginCheckInTime as ExpectedInTime,
		s.BeginCheckOutTime as ExpectedOutTime,
		a.InFixed,
		a.OutFixed,
		a.LateIn,
		a.EarlyOut,
		TotalWorkHours,
		Absence,
		a.LeaveTypeId,
		LeaveDay,
		--Notes,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and isnull(a.LateIn,0) > 0 then 1 else 0 end isLateIn,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and isnull(a.EarlyOut,0) > 0 then 1 else 0 end isEarlyOut,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and InFixed is null then 1 else 0 end isMissingCheckin,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and OutFixed is null and AttendanceDate < @today then 1 else 0 end isMissingCheckOut,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=1 
				and a.LeaveTypeId = @LeaveTypeDefaultId and @now> (AttendanceDate + ' ' + s.EndCheckInTime) -- over the check in time
				then 1 else 0 end isNotWork,
		case when isnull(Absence,0)=1 and a.LeaveTypeId is null then 1 else 0 end isOffday
	FROM dbo.EmployeeAttendance a WITH (NOLOCK) 
		--JOIN dbo.EmployeeCalendar c WITH (NOLOCK) 	ON c.EmpId=a.EmpId and c.Workdate = a.AttendanceDate
		JOIN Employee e WITH (NOLOCK) ON a.Empid= e.Id
			LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id=a.ShiftId			
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid=g.Id			
	WHERE attendancedate between @fromtime and  @totime
			AND e.Enabled =1
			and ((e.isDeleted is null or e.isDeleted = 0) or @excludeUserDelete = 0)
			and (@empid is null or a.empid=@empid)		
			and (@groupids IS NULL OR @groupids = '' OR g.Id IN (SELECT GroupId FROM #GroupsList))
) T
LEFT JOIN LeaveType lv WITH (NOLOCK)  ON lv.ID = T.LeaveTypeId
LEFT JOIN 
( 
	SELECT
		er.WorkDate,
		a.EmpId, 
		a.RequestType,
		a.FromDate,
		a.ToDate,
		a.TotalDays,
		a.OvertimeType,
		a.IsOvertimeHours,
		a.Remark,
		l.Name as LeaveType,
		(CASE
			WHEN a.RequestType = 5 OR a.RequestType = 7 THEN	
				s1.Code
			ELSE
				s3.Code
			END	
			) as FromShift,
		s2.Code as ToShift,
		case when a.RequestType = 3 then a.LateInMins else a.EarlyOutMins end as [Time],
		CASE WHEN a.IsOvertimeHours = 1 THEN (CAST(CAST(a.TotalDays AS INT) AS VARCHAR) + CHAR(39)) ELSE s1.Code END AS OvertimeName, 
		ot.FromTime AS OvertimeFromTime,
		ot.ToTime AS OvertimeToTime,
		ot.TotalHours AS OvertimeTotalHours

		FROM WorkingTimeRequest a WITH(NOLOCK)
		INNER JOIN EmployeeCalendar_Request er WITH(NOLOCK) ON a.id= er.RequestId
		LEFT JOIN EmployeeOvertime ot WITH(NOLOCK) ON a.Id = ot.RequestId
		LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid=a.empid and c.WorkDate = convert(VARCHAR(10),a.FromDate,111)
		LEFT JOIN dbo.LeaveType l WITH (NOLOCK) ON l.Id = a.LeaveTypeID
		LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id= a.FromShift
		LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id= a.ToShift
		LEFT JOIN dbo.Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
		
		WHERE a.Status = 2 
		AND ((CONVERT(VARCHAR(10),er.WorkDate, 101) between @fromtime and  @totime)) 
		-- OR (CONVERT(VARCHAR(10),a.ToDate,101) between @fromtime and  @totime))
) r 
ON (T.EmpId = r.EmpId 
-- AND CONVERT(VARCHAR(10),T.AttendanceDate,101) = CONVERT(VARCHAR(10),r.FromDate,101)
	--AND (
	--(r.ToDate IS NOT NULL AND CONVERT(VARCHAR(10),T.AttendanceDate,101) BETWEEN CONVERT(VARCHAR(10),r.FromDate,101) AND CONVERT(VARCHAR(10),r.ToDate,101)) OR 
	--(r.ToDate IS NULL AND CONVERT(VARCHAR(10),T.AttendanceDate,101) = CONVERT(VARCHAR(10),r.FromDate,101))
	--)
	--)
	AND CONVERT(VARCHAR(10),T.AttendanceDate,101) = CONVERT(VARCHAR(10),r.WorkDate,101))
WHERE @type=0 or 
		(@type =1 and isMissingCheckin =1) or
		(@type =2 and isMissingCheckout=1) or
		(@type =3 and isLateIn =1) or
		(@type =4 and isEarlyOut =1) or
		(@type =5 and isNotWork =1)
		
ORDER BY attendancedate DESC, [Rank] , TeamCode, T.code	

DROP TABLE #GroupsList