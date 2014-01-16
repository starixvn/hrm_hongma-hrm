/*
--set @empid= 165--165--143
Report_EmployeeDashBoard 143, '2012/04/01'
*/

ALTER PROCEDURE Report_EmployeeDashBoard
	@empid int,
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

DECLARE  @month int,  @year int, 
		@groupid int, @AnnualLeaveBalance numeric (18,1),
		@TotalWorkHours decimal (18,2),  
		@previousMonth datetime, @fromDate datetime, @toDate datetime

SET @date = CONVERT(VARCHAR(10),@date,111)
SET @fromDate = cast(YEAR(@date) as varchar(4)) + '/' + cast(MONTH(@date) as varchar(2)) + '/01'
SET @toDate = dateadd(d,-1, dateadd(m,1,@fromDate))
SET @previousMonth = DATEADD(m,-1,@date )

SELECT @AnnualLeaveBalance = AnnualLeaveBalance, 
		@groupid=groupid
FROM employee WITH (NOLOCK) 
WHERE id=@empid


SELECT @TotalWorkHours = SUM(TotalWorkHours)
FROM dbo.EmployeeAttendance
WHERE empid=@empid	
	and AttendanceDate between @fromDate and @todate
SELECT 
	ISNULL(@AnnualLeaveBalance,0) AnnualLeaveBalance,
	ISNULL(@TotalWorkHours,0) TotalWorkHours,
	COUNT(*) TotalRequest,
	ISNULL(SUM(case when requesttype in (1,2) and status in (1,2) then 1 else 0 end),0) as TotalCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (1,2) and status=1 then 1 else 0 end),0) as TotalPendingCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and status in (1,2)  then 1 else 0 end),0) as TotalLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and status=1 then 1 else 0 end),0) as TotalPendingLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype = 5 and status in (1,2) then 1 else 0 end),0) as TotalChangedShiftRequest,
	ISNULL(SUM(case when requesttype = 5 and status=1 then 1 else 0 end),0) as TotalPendingChangedShiftRequest,
	ISNULL(SUM(case when requesttype = 6 and status in (1,2) then 1 else 0 end),0) as TotalLeaveRequest,
	ISNULL(SUM(case when requesttype = 6 and status=1 then 1 else 0 end),0) as TotalPendingLeaveRequest
FROM dbo.WorkingTimeRequest
WHERE CreatedDate between @fromDate and @todate
	AND empid=@empid