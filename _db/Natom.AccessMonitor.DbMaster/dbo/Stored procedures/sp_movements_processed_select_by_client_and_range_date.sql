CREATE PROCEDURE [dbo].[sp_movements_processed_select_by_client_and_range_date]
(
	@ClientId INT,
	@From DATE,
	@To DATE,
	@DocketId INT = NULL
)
AS
BEGIN

	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
			SELECT
				P.[Date],

				D.DocketNumber,
				D.FirstName,
				D.LastName,
				D.DNI,
				T.[Name] AS Title,
	
				ExpectedPlace.[Name] AS ExpectedPlace,

				P.ExpectedIn,
				P.[In],
				PlaceIn.[Name] AS PlaceIn,
				GoalIn.[Name] AS GoalIn,

				P.ExpectedOut,
				P.[Out],
				PlaceOut.[Name] AS PlaceOut,
				GoalOut.[Name] AS GoalOut,
				P.[OutWasEstimated],

				P.PermanenceTime
			FROM
				[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] P WITH(NOLOCK)
				INNER JOIN [dbo].[Docket] D WITH(NOLOCK) ON D.DocketId = P.DocketId
				INNER JOIN [dbo].[Title] T WITH(NOLOCK) ON T.TitleId = D.TitleId
				--IN
				INNER JOIN [dbo].[Place] ExpectedPlace WITH(NOLOCK) ON P.ExpectedPlaceId = ExpectedPlace.PlaceId
				LEFT JOIN [dbo].[Place] PlaceIn WITH(NOLOCK) ON P.InPlaceId = PlaceIn.PlaceId
				LEFT JOIN [dbo].[Goal] GoalIn WITH(NOLOCK) ON P.InGoalId = GoalIn.GoalId

				--OUT
				LEFT JOIN [dbo].[Place] PlaceOut WITH(NOLOCK) ON P.OutPlaceId = PlaceOut.PlaceId
				LEFT JOIN [dbo].[Goal] GoalOut WITH(NOLOCK) ON P.OutGoalId = GoalOut.GoalId
			WHERE
				P.[Date] BETWEEN ''' + CAST(@From as varchar) + ''' AND ''' + CAST(@To as varchar) + '''
				' + (CASE WHEN @DocketId IS NOT NULL THEN ' AND P.DocketId = ' + cast(@DocketId AS varchar) + ' ' ELSE '' END) + '
	';

	EXEC sp_executesql @SQL

END