CREATE TABLE [dbo].[Device]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[InstanceId] CHAR(32) NOT NULL,
	[DeviceId] INT NOT NULL,
	[DeviceName] NVARCHAR(50) NOT NULL,
	[DeviceIP] NVARCHAR(50),
	[DeviceUser] NVARCHAR(50),
	[DevicePassword] NVARCHAR(50),
	[DeviceIsOnline] BIT NOT NULL DEFAULT 1,
	[GoalId] INT,
	[AddedAt] DATETIME NOT NULL,
	[LastConfigurationAt] DATETIME,
	[SerialNumber] NVARCHAR(50),
	[Model] NVARCHAR(30),
	[Brand] NVARCHAR(30),
	[DateTimeFormat] NVARCHAR(20),
	[FirmwareVersion] NVARCHAR(30)
);