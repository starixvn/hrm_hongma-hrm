/*
--Holiday_Insert 'TEST 10/4', '2012/4/10','2012/4/10'

Holiday_Delete 5
*/

ALTER PROCEDURE Holiday_Delete
	@Id int	,
	@ModifiedBy int,
	@ModifiedDate datetime 	
AS

/*
Created: 2012/04/20@Dan
Task: delete a holiday
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @Fromdate datetime, @todate datetime

Select @fromdate = fromdate, @todate = todate
from dbo.Holiday
where Id=@id

-- update working calendar for employee tu turn off the isHoliday 
UPDATE dbo.EmployeeCalendar
Set IsHoliday = 0,
	ModifiedBy = @ModifiedBy,
	ModifiedDate = @ModifiedDate
WHERE WorkDate BETWEEN @FromDate AND @ToDate

-- update the attendance data
EXEC EmployeeAttendance_ProcessingAndInsert null, @fromdate, @todate, 1 

-- delete the holiday
DELETE Holiday
WHERE Id=@id