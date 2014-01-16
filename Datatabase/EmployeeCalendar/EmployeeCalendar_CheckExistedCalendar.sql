/*declare @empid int, @fromdate datetime, @todate datetime,
		@maxDate datetime, @minDate datetime
set @empid=86
set @fromdate='2012/04/06'
set @todate='2012/06/06'
exec EmployeeCalendar_CheckExistedCalendar 86, 
*/
alter PROCEDURE dbo.EmployeeCalendar_CheckExistedCalendar
	@empid int, 
	@fromdate datetime, 
	@todate datetime
AS	
/*
Created: 2012/04/20@Dan
Task: approve request form (main SP)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

declare	@maxDate datetime, @minDate datetime, @result bit
set @result=0

select @maxDate=max(workdate),@minDate = min(workdate)
from dbo.EmployeeCalendar
where empid=@empid
	and workdate between @fromdate and @todate	

if (@fromdate = @minDate and @todate = @maxDate)
	set @result = 1

select @result