CREATE PROCEDURE [dbo].[sp_usuarios_list_by_cliente_and_scope]
(
	@Scope NVARCHAR(20),
	@ClienteId INT = NULL,
	@Search NVARCHAR(100) = NULL,
	@Skip INT,
	@Take INT
)
AS
BEGIN

	DECLARE @TotalRegistros INT = 0;
	DECLARE @TotalFiltrados INT = 0;
	
	SELECT
		U.UsuarioId,
		U.Email AS Usuario,
		U.Nombre,
		U.Apellido,
		U.FechaHoraAlta,
		CASE WHEN FechaHoraBaja IS NOT NULL THEN
			'Inactivo'
		WHEN FechaHoraConfirmacionEmail IS NULL THEN
			'Pendiente de confirmación de Email'
		ELSE
			'Activo'
		END AS Estado,
		COALESCE(C.RazonSocial, 'Natom') AS ClienteRazonSocial,
		C.CUIT AS ClienteCUIT
	INTO
		#TMP_USUARIOS
	FROM
		[dbo].[Usuario] U WITH(NOLOCK)
		LEFT JOIN [$(DbMaster)].[dbo].[Cliente] C WITH(NOLOCK) ON C.ClienteId = U.ClienteId
	WHERE
		U.Scope = @Scope
		AND U.ClienteId = COALESCE(@ClienteId, 0)
	ORDER BY
		U.Nombre, U.Apellido;


	SET @TotalRegistros = COALESCE((SELECT COUNT(*) FROM #TMP_USUARIOS), 0);

	SELECT
		*
	INTO
		#TMP_USUARIOS_FILTRADOS
	FROM
		#TMP_USUARIOS
	WHERE
		@Search IS NULL
		OR
		(
			@Search IS NOT NULL
			AND
			(
				Usuario LIKE '%' + @Search + '%'
				OR Nombre LIKE '%' + @Search + '%'
				OR Apellido LIKE '%' + @Search + '%'
				OR Estado LIKE '%' + @Search + '%'
				OR ClienteRazonSocial LIKE '%' + @Search + '%'
			)
		);

	SET @TotalFiltrados = COALESCE((SELECT COUNT(*) FROM #TMP_USUARIOS_FILTRADOS), 0);


	SELECT
		F.*,
		@TotalFiltrados AS TotalFiltrados,
		@TotalRegistros AS TotalRegistros
	FROM
		#TMP_USUARIOS_FILTRADOS F
	ORDER BY
		F.Nombre ASC, F.Apellido ASC
	OFFSET @Skip ROWS
	FETCH NEXT @Take ROWS ONLY;

END