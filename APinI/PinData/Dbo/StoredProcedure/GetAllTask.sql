CREATE PROCEDURE [dbo].[GetAllTask]
AS
BEGIN
	SELECT [Name],[Description],[Status] FROM [dbo].[Tasks]
END