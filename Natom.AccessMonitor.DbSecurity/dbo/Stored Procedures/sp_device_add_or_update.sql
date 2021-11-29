CREATE PROCEDURE [dbo].[sp_device_add_or_update]
(
	@InstanceId CHAR(32),
	@DeviceId INT,
	@DeviceName NVARCHAR(50),
	@LastConfigurationAt DATETIME = NULL,
	@SerialNumber NVARCHAR(50),
	@Model NVARCHAR(30),
	@Brand NVARCHAR(30),
	@DateTimeFormat NVARCHAR(20),
	@FirmwareVersion NVARCHAR(30)
)
AS
BEGIN

	DECLARE @currentDeviceName NVARCHAR(50);
	DECLARE @currentLastConfigurationAt DATETIME;

	SELECT
		@currentDeviceName = DeviceName,
		@currentLastConfigurationAt = LastConfigurationAt
	FROM
		[dbo].[Device]
	WHERE
		InstanceId = @InstanceId
		AND DeviceId = @DeviceId;

	--SI NO EXISTE EL DEVICE LO INSERTAMOS
	IF @currentDeviceName IS NULL
		INSERT INTO [dbo].[Device] (InstanceId, DeviceId, DeviceName, GoalId, AddedAt,
									LastConfigurationAt, SerialNumber, Model, Brand,
									DateTimeFormat, FirmwareVersion)
				VALUES (@InstanceId, @DeviceId, @DeviceName, NULL, GETDATE(),
						@LastConfigurationAt, @SerialNumber, @Model, @Brand,
						@DateTimeFormat, @FirmwareVersion);

	--SI YA EXISTE EL DEVICE Y CAMBIO LA CONFIGURACIÓN, LO ACTUALIZAMOS
	ELSE IF @currentDeviceName IS NOT NULL AND (@currentLastConfigurationAt IS NULL OR @currentLastConfigurationAt != @LastConfigurationAt)
		UPDATE [dbo].[Device]
			SET DeviceName = @DeviceName,
				LastConfigurationAt = @LastConfigurationAt,
				SerialNumber = @SerialNumber,
				Model = @Model,
				Brand = @Brand,
				DateTimeFormat = @DateTimeFormat,
				FirmwareVersion = @FirmwareVersion
			WHERE InstanceId = @InstanceId
					AND DeviceId = @DeviceId;

END