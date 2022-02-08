CREATE PROCEDURE [dbo].[sp_usuarios_recover_by_email]
(
	@Scope NVARCHAR(20),
	@Email NVARCHAR(50),
	@SecretConfirmation CHAR(32),
	@ByUsuarioId INT
)
AS
BEGIN

	DECLARE @UsuarioId INT = 0;
	DECLARE @Result INT = 0;
	   
	BEGIN TRANSACTION

		SELECT @UsuarioId = UsuarioId
		FROM [dbo].[Usuario]
			WHERE
				[Scope] = @Scope
				AND [Email] = @Email;


		IF @UsuarioId <> 0
		BEGIN
			UPDATE [dbo].[Usuario]
				SET 
				   [SecretConfirmacion] = @SecretConfirmation,
				   [FechaHoraConfirmacionEmail] = NULL,
				   [Clave] = NULL
				WHERE
					UsuarioId = @UsuarioId;

			SET @Result = @@ROWCOUNT;

			IF @Result = 1
				EXEC [$(DbMaster)].[dbo].[sp_history_insert] 0, 'Usuario', @UsuarioId, @ByUsuarioId, 'Envio mail de recuperación de clave (solicitado por el mismo)';
		END


	COMMIT TRANSACTION;
	   
	
	IF @Result = 1
		SELECT * FROM [dbo].[Usuario]
			WHERE UsuarioId = @UsuarioId;

END