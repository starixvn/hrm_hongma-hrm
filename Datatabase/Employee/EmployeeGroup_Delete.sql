/*
declare @ErrCode int,@ErrMsg nvarchar(200)
		
exec EmployeeGroup_Delete 25,'2012/06/11',86,@ErrCode OUTPUT,@ErrMsg OUTPUT

select @ErrCode,@ErrMsg
*/ 
ALTER PROCEDURE EmployeeGroup_Delete
	@Id int,
    @ModifiedDate datetime,
    @ModifiedBy int,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: soft delete a group 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	IF (EXISTS(SELECT * FROM dbo.EmployeeGroup_Employee eg WITH (NOLOCK) 
							JOIN Employee e WITH (NOLOCK) ON eg.EmpID=e.ID
			WHERE eg.GroupID=@Id AND ISNULL(isDeleted,0)=0))
	BEGIN
		SELECT @ErrCode=100,
				@ErrMsg = 'The group is not empty so cannot delete it'
		RETURN
	END

	UPDATE [dbo].[EmployeeGroup]
	SET Deleted = 1
		,ModifiedDate = @ModifiedDate
		,ModifiedBy = @ModifiedBy
	WHERE Id=@Id
	SET @ErrCode=0
END