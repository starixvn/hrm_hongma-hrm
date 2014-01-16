USE [HRMS-HongMa]
GO
/****** Object:  StoredProcedure [dbo].[Report_LeaveSummary]    Script Date: 10/18/2013 8:44:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
declare @empid int, @groupid int,
		@todate datetime, @year datetime
set @todate = '2012/06/1'
exec Report_LeaveSummary @empid , @groupid, 2012
*/

ALTER PROCEDURE [dbo].[Report_LeaveSummary] 
	@empid int, 
	@groupids varchar(max) = '',
	@year int
AS
/*
Created: 2012/04/24@Dan
Task: leave summary report
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/

CREATE TABLE #GroupsList (GroupId TINYINT)
INSERT INTO #GroupsList (GroupId)
SELECT val FROM fnc_strSplit (@groupids, ',')

DECLARE @fromdate datetime, @todate datetime 
-- get the first date of year
SET @fromdate = cast(@year as varchar(10)) + '/01/01'
SET @todate = DATEADD(yy, 1, @fromdate)

-- get the openning balace for annual leave to first date of year
SELECT  e.Id as Empid, SUM( ISNULL(CreditDays,0) - ISNULL(DeductDays,0)) as OpenBalance 	
 INTO #OpenBalance
FROM  Employee e WITH (NOLOCK) 
LEFT JOIN (SELECT * FROM dbo.LeaveTrans WITH (NOLOCK) WHERE ISNULL(Deleted,0)=0 AND FromDate < @fromdate) t 
	 ON e.Id=t.Empid
	--LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID=e.GroupId
WHERE -- (FromDate < @fromdate OR FromDate IS NULL) 
	(@EmpId IS NULL OR e.id=@empid) 
	-- AND ISNULL(t.Deleted,0)=0
	AND (@groupids IS NULL OR @groupids = '' OR e.GroupId IN (SELECT GroupId FROM #GroupsList))
	AND ( ISNULL(e.isDeleted,0)=0 OR YEAR(ResignedDate) >= year(@todate))

GROUP BY e.Id

-- select * from #OpenBalance order by Empid

-- calculate the balance form the first date of year to @todate
SELECT  e.Id as EmpId,
		SUM( ISNULL(CreditDays,0)) CreditDays,
		SUM( ISNULL(DeductDays,0)) DeductDays,
		SUM(CASE WHEN MONTH(FromDate)=1 THEN ISNULL(DeductDays,0) ELSE 0 END) Jan,
		SUM(CASE WHEN MONTH(FromDate)=2 THEN ISNULL(DeductDays,0) ELSE 0 END) Feb,
		SUM(CASE WHEN MONTH(FromDate)=3 THEN ISNULL(DeductDays,0) ELSE 0 END) Mar,
		SUM(CASE WHEN MONTH(FromDate)=4 THEN ISNULL(DeductDays,0) ELSE 0 END) Apr,
		SUM(CASE WHEN MONTH(FromDate)=5 THEN ISNULL(DeductDays,0) ELSE 0 END) May,
		SUM(CASE WHEN MONTH(FromDate)=6 THEN ISNULL(DeductDays,0) ELSE 0 END) Jun,
		SUM(CASE WHEN MONTH(FromDate)=7 THEN ISNULL(DeductDays,0) ELSE 0 END) Jul,
		SUM(CASE WHEN MONTH(FromDate)=8 THEN ISNULL(DeductDays,0) ELSE 0 END) Aug,
		SUM(CASE WHEN MONTH(FromDate)=9 THEN ISNULL(DeductDays,0) ELSE 0 END) Sep,
		SUM(CASE WHEN MONTH(FromDate)=10 THEN ISNULL(DeductDays,0) ELSE 0 END) Oct,
		SUM(CASE WHEN MONTH(FromDate)=11 THEN ISNULL(DeductDays,0) ELSE 0 END) Nov,
		SUM(CASE WHEN MONTH(FromDate)=12 THEN ISNULL(DeductDays,0) ELSE 0 END) Dec
INTO #CurrenBalace				
FROM  Employee e WITH (NOLOCK) 
LEFT JOIN (SELECT * FROM dbo.LeaveTrans WITH (NOLOCK) WHERE ISNULL(Deleted,0)=0 AND FromDate >= @fromdate and FromDate< @todate) t 
-- FROM dbo.LeaveTrans t WITH (NOLOCK)	
--	JOIN Employee e WITH (NOLOCK) 
ON e.Id=t.Empid	
WHERE (@EmpId IS NULL OR e.id=@empid)
	-- AND ISNULL(t.Deleted,0)=0
	AND (@groupids IS NULL OR @groupids = '' OR e.GroupId IN (SELECT GroupId FROM #GroupsList))
	AND ( ISNULL(e.isDeleted,0)=0 OR YEAR(ResignedDate) >= year(@todate))
GROUP BY e.ID

--  select * from #CurrenBalace order by Empid

-- show the report
SELECT 
		cb.EmpId, e.Code,
		FullName, NickName, g.Name [Team], g.[Rank],
		JoinedDate,	ConfirmedDate, 
		ISNULL(OpenBalance,0) [AnnualLeaveLastYear],
		ISNULL(CreditDays,0) [AnnualLeaveCurrentYear],
		Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec,
--		ISNULL(DeductDays,0) LeaveTaken,
		ISNULL(OpenBalance,0) + ISNULL(CreditDays,0) - ISNULL(DeductDays,0) AS LeaveBalance,
		'' as Remark
FROM #CurrenBalace cb LEFT JOIN #OpenBalance op ON OP.empid = cb.empid
	JOIN Employee e WITH (NOLOCK) on e.Id=cb.Empid
	LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID=e.GroupId

 UNION ALL

SELECT 
		op.EmpId, e.Code,
		FullName, NickName, g.Name [Team], g.[Rank],
		JoinedDate,	ConfirmedDate, 
		ISNULL(OpenBalance,0) [AnnualLeaveLastYear],
		0 as [AnnualLeaveCurrentYear],
		0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dec,
--		ISNULL(DeductDays,0) LeaveTaken,
		ISNULL(OpenBalance,0) AS LeaveBalance,
		'' as Remark
FROM #OpenBalance op 
	JOIN Employee e WITH (NOLOCK) ON e.Id=op.Empid
	LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID=e.GroupId
WHERE Empid NOT IN (SELECT EmpID FROM #CurrenBalace cb)		
ORDER BY [Rank] , [Team] , code				


DROP TABLE #OpenBalance
DROP TABLE #CurrenBalace
DROP TABLE #GroupsList
