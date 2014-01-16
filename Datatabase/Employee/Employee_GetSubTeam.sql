-- Employee_GetSubTeam 165

ALTER PROCEDURE [dbo].[Employee_GetSubTeam]
	-- Add the parameters for the stored procedure here
	@empId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select g.* 
	from dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
		JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON eg.GroupID = g.ID AND isMaingroup=0
	where EmpID=@empId 
END
