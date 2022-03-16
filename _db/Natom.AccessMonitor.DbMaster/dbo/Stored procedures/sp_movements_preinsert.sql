CREATE PROCEDURE [dbo].[sp_movements_preinsert]
(
	@ClientId INT
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + ']'') AND type in (N''U''))
			CREATE TABLE [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + ']
			(
				[InstanceId] CHAR(32) NOT NULL,
				[DeviceId] INT NOT NULL,
				[DateTime] DATETIME NOT NULL,
				[DocketNumber] INT NOT NULL,
				[MovementType] CHAR(1) NOT NULL,
				[GoalId] INT,
				[ProcessedAt] DATETIME,
				CONSTRAINT [PK_zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '] PRIMARY KEY CLUSTERED ([DateTime] ASC, [InstanceId] ASC, [DeviceId] ASC, [DocketNumber] ASC)
			);

		IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed]'') AND type in (N''U''))
			CREATE TABLE [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed]
			(
				[Date] DATE NOT NULL,
				[DocketId] INT NOT NULL,

				[InTime] TIME NOT NULL,
				[InGoalId] INT NOT NULL,
				[InPlaceId] INT NOT NULL,
				[InDeviceId] INT,
				[InDeviceMovementType] CHAR(1),
				[InWasEstimated] BIT NOT NULL,
				[InProcessedAt] DATETIME NOT NULL,

				[OutTime] TIME,
				[OutGoalId] INT,
				[OutPlaceId] INT,
				[OutDeviceId] INT,
				[OutDeviceMovementType] CHAR(1),
				[OutWasEstimated] BIT,
				[OutProcessedAt] DATETIME,

				[PermanenceTime] TIME,

				CONSTRAINT [PK_zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] PRIMARY KEY CLUSTERED ([Date] ASC, [DocketId] ASC, [InTime] ASC)
			);
	';

	EXEC sp_executesql @SQL

END