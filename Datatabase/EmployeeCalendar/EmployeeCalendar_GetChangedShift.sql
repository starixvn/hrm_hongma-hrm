-- EmployeeCalendar_GetChangedShift '2012/06/6'
ALTER PROCEDURE EmployeeCalendar_GetChangedShift
	@reportDate datetime,
	@employeeId int = null,
	@groupid int = null
AS
/*
Created: 2012/06/20@Dan
Task: get changed shift in a month (it's used for coloring the day in report)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON

DECLARE @Days varchar(1000), @cols varchar(2000), @totalDays int, 
		@fromDate datetime, @toDate datetime

-- first day of month
set @fromDate= cast(YEAR(@reportDate) as varchar(4)) + '/' + cast(MONTH(@reportDate) as varchar(2)) + '/01'

-- last day of month
set @ToDate = dateadd(d,-1, dateadd(m,1,@fromdate))

SELECT e.ID, e.NickName, e.FullName, DAY(workdate) as Day, s.Code 
FROM dbo.EmployeeCalendar c with (nolock)
	join employee e with (nolock) on e.id=c.empid
	left join Shift s on s.id= c.shiftid			
WHERE WorkDate between @fromDate and @toDate
	and (@employeeId is null or empid=@employeeId)
	and (@groupid  is null or e.groupid=@groupid)
	and ChangeShiftId is not null
	and IsHoliday=0 and IsOffDay=0 and IsLeaveDay=0