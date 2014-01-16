/*  
declare @empId int, @date datetime, @shiftId int  
select @empId=163,  
 @ShiftId = null,  
 @date  = '2012/04/3'  
  
exec EmployeeCalendar_Update @empId , @date , @ShiftID  
*/  
  
CREATE PROCEDURE [dbo].[EmployeeCalendar_Update]   
	@EmpId int,   
	@date datetime,  
	@ShiftID int,
	@ModifiedBy int,
	@ModifiedDate datetime
AS  
/*  
Created: 2012/05/10@Dan  
Task: Update working calendar for an employee in a day  
DB: NexHR  
Original:   
  
Revisions:      
Param's Explanation:  
 @EmpId: employee ID  
 @date: the date to update  
 @ShiftID: shift id (null if not work)  
*/  
  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
 DECLARE @fromdate datetime,  
   @todate datetime,  
   @dw int,  
   @isHoliday bit  
  
 set @date = convert(varchar(10),@date,111)  
  
 UPDATE dbo.EmployeeCalendar  
 SET @dw=datepart(dw, WorkDate),  
	@isHoliday =  case when EXISTS(select * from dbo.Holiday   
		 where WorkDate between FromDate and ToDate)  
	   THEN 1 ELSE 0 END,  
	IsHoliday = Case WHEN @ShiftID IS NULL THEN @isHoliday ELSE 0 end ,  
	ShiftId =  @ShiftID,  
	isOffDay = case when @ShiftID is null and @isHoliday=0 and LeaveTypeId is null then 1  
	  else 0 end,  
	IsLeaveDay = case when @ShiftId is null and LeaveTypeId is not null then 1  
	  else 0 end ,
	ModifiedBy = @ModifiedBy ,
	ModifiedDate = @ModifiedDate  
 WHERE empId = @EmpId AND  
  datediff(d, WorkDate, @date) = 0  
  
 -- update attendate record   
 EXEC EmployeeAttendance_ProcessingAndInsert @EmpID, @date, @date, 1  
  
  