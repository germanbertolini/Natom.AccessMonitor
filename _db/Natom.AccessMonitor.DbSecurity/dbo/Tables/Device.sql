CREATE TABLE [dbo].[Device]
(
	[InstanceId] CHAR(32) NOT NULL,
	[DeviceId] INT NOT NULL,
	[DeviceName] NVARCHAR(50) NOT NULL,
	[GoalId] INT,
	[AddedAt] DATETIME NOT NULL,
	[LastConfigurationAt] DATETIME,
	[SerialNumber] NVARCHAR(50),
	[Model] NVARCHAR(30),
	[Brand] NVARCHAR(30),
	[DateTimeFormat] NVARCHAR(20),
	[FirmwareVersion] NVARCHAR(30)
	CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED ([InstanceId] ASC, [DeviceId] ASC)
);