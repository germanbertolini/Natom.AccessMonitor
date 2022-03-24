CREATE PROCEDURE [dbo].[sp_panorama_actual]
(
	@ClientId INT,
	@PlaceId INT = NULL
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		SELECT
			T.DocketId,
			T.[Date],
			T.[From] AS ExpectedIn,
			P.[In] AS RealIn,
			T.[To] AS ExpectedOut,
			P.[Out] AS RealOut
		FROM
			[dbo].[fn_current_turnos] (' + CAST(@ClientId AS VARCHAR) + ', ' + (CASE WHEN @PlaceId IS NULL THEN 'NULL' ELSE CAST(@PlaceId AS VARCHAR) END) + ') AS T
			LEFT JOIN [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] AS P WITH(NOLOCK) ON T.DocketId = P.DocketId AND T.[Date] = P.[Date]
	';

	EXEC sp_executesql @SQL

END