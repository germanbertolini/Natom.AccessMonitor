CREATE PROCEDURE [dbo].[sp_usuarios_recover_by_id]
(
	@Scope NVARCHAR(20),
	@UsuarioId INT,
	@SecretConfirmation CHAR(32),
	@ByUsuarioId INT
)
AS
BEGIN

	DECLARE @Result INT = 0;
	   
	BEGIN TRANSACTION

		UPDATE [dbo].[Usuario]
			SET 
			   [SecretConfirmacion] = @SecretConfirmation,
			   [FechaHoraConfirmacionEmail] = NULL,
			   [Clave] = NULL
			WHERE
				[Scope] = @Scope
				AND [UsuarioId] = @UsuarioId;

		SET @Result = @@ROWCOUNT;

		IF @Result = 1
			EXEC [$(DbMaster)].[dbo].[sp_history_insert] 0, 'Usuario', @UsuarioId, @ByUsuarioId, 'Envio mail de recuperación de clave';


	COMMIT TRANSACTION;


	IF @Result = 1
		SELECT * FROM [dbo].[Usuario]
			WHERE UsuarioId = @UsuarioId;
	   

END