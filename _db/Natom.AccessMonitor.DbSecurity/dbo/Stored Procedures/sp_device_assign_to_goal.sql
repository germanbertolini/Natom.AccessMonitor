CREATE PROCEDURE [dbo].[sp_device_assign_to_goal]
	@Id INT,
	@GoalId INT
AS
BEGIN

	UPDATE [dbo].[Device]
		SET GoalId = @GoalId
		WHERE Id = @Id;

END