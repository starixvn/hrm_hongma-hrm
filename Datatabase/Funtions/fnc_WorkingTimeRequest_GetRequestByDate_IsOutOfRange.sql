USE [NexHRM]
GO

/****** Object:  UserDefinedFunction [dbo].[fnc_WorkingTimeRequest_GetRequestByDate_IsOutOfRange]    Script Date: 1/23/2013 2:54:44 PM ******/
DROP FUNCTION [dbo].[fnc_WorkingTimeRequest_GetRequestByDate_IsOutOfRange]
GO

/****** Object:  UserDefinedFunction [dbo].[fnc_WorkingTimeRequest_GetRequestByDate_IsOutOfRange]    Script Date: 1/23/2013 2:54:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[fnc_WorkingTimeRequest_GetRequestByDate_IsOutOfRange](@fromDate1 datetime, @toDate1 datetime, @fromDate2 datetime, @toDate2 datetime)
RETURNS bit
AS
BEGIN
	IF @toDate1 IS NULL
		SET @toDate1 = @fromDate1
	
	DECLARE @ret bit;
	SET @ret  = 0;
	if ( (@fromDate1 > @toDate2) or (@toDate1 < @fromDate2) )
		set @ret = 1;

	RETURN @ret;
END;
GO


