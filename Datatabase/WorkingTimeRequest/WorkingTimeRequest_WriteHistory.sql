USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_WriteHistory]    Script Date: 10/21/2014 12:43:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- [WorkingTimeRequest_WriteHistory] 316

ALTER PROCEDURE [dbo].[WorkingTimeRequest_WriteHistory]	
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
			LogDate,
			OvertimeType,
			IsOvertimeHours,
			RequestDate
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
			getdate(),
			OvertimeType,
			IsOvertimeHours,
			RequestDate
	FROM dbo.WorkingTimeRequest
	WHERE Id = @RequestID