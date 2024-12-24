CREATE PROCEDURE [dbo].[SetCurrentIp]
	@newIpAddress NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO [PinData].[dbo].[IpAddress]
	(
		LastIpAddress,
		CreatedOn
	)
	VALUES
	(
		@newIpAddress,
		GETDATE()
	)

	SELECT 0 AS ErrorCode, 'Success' AS ErrorMessage
END
