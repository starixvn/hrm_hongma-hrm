CREATE PROCEDURE [dbo].[WorkingTimeRequest_Delete] 
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

GO


