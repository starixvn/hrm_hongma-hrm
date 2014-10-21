USE [HRMS]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_Delete]    Script Date: 10/21/2014 11:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[WorkingTimeRequest_Delete] 
	-- Add the parameters for the stored procedure here
	@id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete WorkingTimeRequest
	where Id = @id and Status != 2
END
