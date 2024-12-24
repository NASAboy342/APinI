CREATE PROCEDURE [dbo].[GetPreviousIp]
AS
SET NOCOUNT ON
BEGIN
	SELECT [Id], [LastIpAddress], [CreatedOn] FROM [IpAddress] WITH(NOLOCK)
	ORDER BY [Id] DESC
END
