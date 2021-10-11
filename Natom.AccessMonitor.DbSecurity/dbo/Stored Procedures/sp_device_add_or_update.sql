CREATE PROCEDURE [dbo].[sp_device_add_or_update]
(
	@InstanceId CHAR(32),
	@DeviceId INT,
	@DeviceName NVARCHAR(50)
)
AS
BEGIN

	DECLARE @currentDeviceName NVARCHAR(50);
	SELECT
		@currentDeviceName = DeviceName
	FROM
		[dbo].[Device]
	WHERE
		InstanceId = @InstanceId
		AND DeviceId = @DeviceId;

	--SI NO EXISTE EL DEVICE LO INSERTAMOS
	IF @currentDeviceName IS NULL
		INSERT INTO [dbo].[Device] (InstanceId, DeviceId, DeviceName, GoalId, AddedAt)
				VALUES (@InstanceId, @DeviceId, @DeviceName, NULL, GETDATE());

	--SI YA EXISTE EL DEVICE Y CAMBIO EL NOMBRE, LO ACTUALIZAMOS
	ELSE IF @currentDeviceName IS NOT NULL AND @currentDeviceName != @DeviceName
		UPDATE [dbo].[Device]
			SET DeviceName = @DeviceName
			WHERE InstanceId = @InstanceId
					AND DeviceId = @DeviceId;

END