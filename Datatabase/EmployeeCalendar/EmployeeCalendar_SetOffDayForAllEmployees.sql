-- EmployeeCalendar_SetOffDayForAllEmployees '2012/07/02',0, 128 , '2012/07/19 11:11'

ALTER PROCEDURE [dbo].[EmployeeCalendar_SetOffDayForAllEmployees]
	@date datetime,
	@isOffDay bit,
	@ModifiedBy int,
	@ModifiedDate datetime ,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT		
AS	

/*
Created: 2012/07/19@Dan
Task: Turn on/off the off day on working calendar for all employees. 
	It's used to set replacement off day for public holiday when public holiday is on the off day
DB: NexHRM
Original: 

Revisions:    
Param's Explanation:

*/

SET NOCOUNT ON;  
IF (@isOffDay IS NULL)
	SET @isOffDay=0
	
-- if not exist calendar

IF (NOT EXISTS(SELECT *
				FROM dbo.EmployeeCalendar WITH (NOLOCK)
				WHERE workdate = @date					
					)
	)
BEGIN
	SET @ErrCode = 100
	SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
	RETURN
END

-- cut off time

 set @date = convert(varchar(10),@date,111)  
  
 UPDATE dbo.EmployeeCalendar  
 SET isOffDay = case when @isOffDay = 0 and ShiftID is null and isHoliday=0 and isLeaveday=0 then 1
					else @isOffDay end,
	ModifiedBy = @ModifiedBy,
	ModifiedDate = @ModifiedDate					
 WHERE WorkDate = @date
    
 -- update attendate record   
 EXEC EmployeeAttendance_ProcessingAndInsert null, @date, @date, 1  
 
 -- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=getdate()
--Log general info
Select @LogSPData = '<EmployeeCalendar_SetOffDayForAllEmployees @date="'+ coalesce(convert(varchar(10),@date, 111 ),space(1)) + '" ' +
'isOffDay="'+ coalesce(cast(@isOffDay as varchar(100)),space(1)) + '" ' +
'@ModifiedBy="'+ coalesce(cast(@ModifiedBy as varchar(100)),space(1)) + '" ' +
'@ModifiedDate="'+ coalesce(convert(varchar(10),@ModifiedDate, 111 ),space(1)) + 
'@ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'@ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'	

EXEC dbo.Log_WriteLog
	@LogActionID = 4,
	@LogDate = @now,
	@Description = 'Set off day for all employees',
	@TrackingValue = @LogSPData,
	@UserID = @ModifiedBy 

set @ErrCode=0
 