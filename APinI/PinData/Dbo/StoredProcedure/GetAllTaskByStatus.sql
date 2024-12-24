CREATE PROCEDURE [dbo].[GetAllTaskByStatus]
	@status NVARCHAR(50)
AS
BEGIN
	SELECT [Name],[Description],[Status] FROM [dbo].[Tasks]
	where [Status] = @status
END
