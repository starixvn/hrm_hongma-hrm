-- Report_EmployeeCalendarByMonth '2012/04/08'

ALTER PROCEDURE Report_EmployeeCalendarByMonth
	@reportDate datetime,
	@employeeId int = null,
	@groupid int = null
AS
/*
Created: 2012/04/26@Dan
Task: view employee calendar by month
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

set @totalDays = DAY(@ToDate)
--print @fromDate
--print @ToDate 

-- process the days string to use in pivot
declare @i int
select  @i=1, @days=''
while (@i<=@totalDays)
begin
	set @days= @days + ',[' + cast(@i as varchar(2)) + ']'
	set @i=@i+1	
end

set @days=substring(@days,2, len(@days)-1)
--print @days

set @cols='EmpId, fullname, nickname, Team,' +  @days

declare @LeaveTypeDefaultId as int 
select @LeaveTypeDefaultId = dbo.fnc_getLeaveTypeDefault()

DECLARE	@HolidayLeaveTypeCode varchar(50)
-- get leave type of holiday
select @HolidayLeaveTypeCode = code
from dbo.LeaveType (nolock)
where IsHoliday = 1

SELECT [Rank], g.Code  [Team], e.Code EmpCode, nickname, fullname, EmpId,  
		day(workdate) d, 
		case when c.isHoliday=0 and IsOffDay=0 and IsLeaveDay=0 then s.code
				when c.isHoliday=1 then @HolidayLeaveTypeCode
				when isOffday=1 then 'OFF'
				when isLeaveDay=1 then lv.code
			end as Code		
INTO #Calendar	
FROM dbo.EmployeeCalendar c 
		left join dbo.LeaveType lv on lv.id=c.LeaveTypeId
		left join Shift s on s.id= c.shiftid			
	join employee e on e.id=c.empid
	left join dbo.EmployeeGroup g on g.id=e.groupid
WHERE WorkDate between @fromDate and @toDate
	and (@employeeId is null or empid=@employeeId)
	and (@groupid  is null or e.groupid=@groupid)
order by [Rank], g.Code, e.Code
	
--SELECT * FROM #Calendar

Declare @query varchar(4000)
set @query = '
	SELECT '+ @cols + 
	' FROM (
		SELECT * FROM #Calendar		
	) ea
	PIVOT
	(
		MAX(Code) FOR d IN (' + @days + ')' + 
	') AS pvt'
	
exec (@query)

DROP TABLE #Calendar