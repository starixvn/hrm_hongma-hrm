-- PRINT  DBO.[fnc_LeaveHalfDayID]()

CREATE function [dbo].[fnc_LeaveHalfDayID]  ()
returns int
as
BEGIN
	DECLARE	@LeaveHalfDayID int 
	select @LeaveHalfDayID = id
	from dbo.LeaveType 
	where Code = 'HAL'
--	print @LeaveHalfDayID -- leave half day need processing specially
	RETURN @LeaveHalfDayID
END