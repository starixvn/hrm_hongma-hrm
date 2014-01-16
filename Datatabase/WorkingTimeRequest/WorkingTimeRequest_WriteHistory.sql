-- [WorkingTimeRequest_WriteHistory] 316

CREATE PROCEDURE [dbo].[WorkingTimeRequest_WriteHistory]	
	@RequestID bigint
AS
/*
Created: 2012/07/09@Dan
Task: Copy request to history 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

INSERT dbo.WorkingTimeRequest_history
		(
			Id,
			EmpId,
			RequestType,
			Status,
			IsPlanned,
			Remark,
			ProcessedDate,
			ProcessedBy,
			CreatedDate,
			CreatedBy,
			ModifiedDate,
			ModifiedBy,
			FromDate,
			FromShift,
			ToDate,
			ToShift,
			LeaveTypeID,
			TotalDays,
			LateInMins,
			EarlyOutMins,
			LogDate
		)
	SELECT
			Id,
			EmpId,
			RequestType,
			Status,
			IsPlanned,
			Remark,
			ProcessedDate,
			ProcessedBy,
			CreatedDate,
			CreatedBy,
			ModifiedDate,
			ModifiedBy,	
			FromDate,
			FromShift,
			ToDate,
			ToShift,
			LeaveTypeID,
			TotalDays,
			LateInMins,
			EarlyOutMins,	
			getdate()
	FROM dbo.WorkingTimeRequest
	WHERE Id = @RequestID