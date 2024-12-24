CREATE PROCEDURE [dbo].[AddTask]
	@name NVARCHAR(50),
	@description NVARCHAR(50),
	@status NVARCHAR(50)
AS
BEGIN
	INSERT INTO [dbo].[Task]
	(
		[Name],
		[Description],
		[Status]
	)
	VALUES
    (
        @name,
        @description,
        @status
	)
	SELECT 0 AS ErrorCode, 'Success' AS ErrorMessage
END
