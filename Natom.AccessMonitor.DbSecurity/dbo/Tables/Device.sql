CREATE TABLE [dbo].[Device]
(
	[InstanceId] CHAR(32) NOT NULL,
	[DeviceId] INT NOT NULL,
	[DeviceName] NVARCHAR(50) NOT NULL,
	[GoalId] INT,
	[AddedAt] DATETIME NOT NULL,
	CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED ([InstanceId] ASC, [DeviceId] ASC)
);