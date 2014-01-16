USE [HRMS-HongMa]
GO
/****** Object:  StoredProcedure [dbo].[Report_WorkingHoursSummary]    Script Date: 10/18/2013 8:45:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
declare @fromdate datetime,
		@todate datetime,
		@empid int,
		@groupid int
set @fromdate = '2012/07/01'
set @todate = '2012/07/30'
set @empid=128
--set @groupid =3
EXEC Report_WorkingHoursSummary @fromdate , @todate ,	@empid , @groupid 
*/
ALTER PROCEDURE [dbo].[Report_WorkingHoursSummary]
	@fromdate datetime,
	@todate datetime,
	@empid int,
	@groupids varchar(max) = ''
AS
/*
Created: 2012/04/23@Dan
Task: Working Hours Summary REPORT
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/

	DECLARE @UnpaidLeaveType INT
	SELECT @UnpaidLeaveType = Id
	FROM dbo.LeaveType WITH (NOLOCK)
	WHERE Code='UPL'	

	CREATE TABLE #GroupsList (GroupId TINYINT)
	INSERT INTO #GroupsList (GroupId)
	SELECT val FROM fnc_strSplit (@groupids, ',')

	SELECT c.empid, e.Code, FullName, NickName, g.Code [Team],
		22 as [WorkingDays],
		sum(case when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=0 then 1 
				when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=1 and c.LeaveDay < 1 then 0.5
				else 0 end) [ActualWorkingDays],
		sum(case when isOffday=0 and isHoliday=0 and IsLeaveDay=1 and c.LeaveTypeId <> @UnpaidLeaveType then c.LeaveDay else 0 end) [LeaveTaken],
		sum(case when isOffday=0 and isHoliday=0 and IsLeaveDay=1 and c.LeaveTypeId = @UnpaidLeaveType  then c.LeaveDay else 0 end) [UnpaidLeave],
		SUM(a.LateIn) LateIn,
		SUM(a.EarlyOut) EarlyOut,
		SUM(a.TotalWorkHours) TotalWorkHours,
		SUM(a.TotalWorkHours)/sum(case when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=0 then 1 
				when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=1 and c.LeaveDay < 1 then 0.5
				else 0 end) as AverageHours		
	FROM	
		dbo.EmployeeAttendance a with (nolock)
		right join dbo.EmployeeCalendar c with (nolock)  on c.empid = a.empid and c.workdate = a.AttendanceDate
		join employee e with (nolock) on e.id = c.empid	
		left join dbo.EmployeeGroup g with (nolock) on g.Id = e.GroupId		
	WHERE (@empid is null or c.empid= @empid)
		and (e.isDeleted is null or e.isDeleted = 0)
		and WorkDate between @fromdate and @todate
		AND (@groupids IS NULL OR @groupids = '' OR e.GroupId IN (SELECT GroupId FROM #GroupsList))
	GROUP BY c.empid, e.Code, FullName, NickName, g.Code, [Rank]
	ORDER BY [Rank] , g.Code , e.code

DROP TABLE #GroupsList