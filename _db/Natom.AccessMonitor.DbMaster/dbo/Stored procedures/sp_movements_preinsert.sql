CREATE PROCEDURE [dbo].[sp_movements_preinsert]
(
	@ClientId INT
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[zMovement_Client' + CAST(@ClientId AS VARCHAR) + ']'') AND type in (N''U''))
			CREATE TABLE [dbo].[zMovement_Client' + CAST(@ClientId AS VARCHAR) + ']
			(
				[InstanceId] CHAR(32) NOT NULL,
				[DeviceId] INT NOT NULL,
				[DateTime] DATETIME NOT NULL,
				[DocketNumber] INT NOT NULL,
				[MovementType] CHAR(1) NOT NULL,
				[GoalId] INT,
				CONSTRAINT [PK_zMovement_Client' + CAST(@ClientId AS VARCHAR) + '] PRIMARY KEY CLUSTERED ([DateTime] ASC, [InstanceId] ASC, [DeviceId] ASC, [DocketNumber] ASC)
			);
	';

	EXEC sp_executesql @SQL

END