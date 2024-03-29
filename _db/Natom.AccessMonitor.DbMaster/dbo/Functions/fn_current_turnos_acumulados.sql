﻿CREATE FUNCTION [dbo].[fn_current_turnos_acumulados]
(
	@ClientId INT,
	@PlaceId INT = NULL
)
RETURNS TABLE
AS
	RETURN
		SELECT
			*
		FROM
			(
				SELECT
					R.DocketId,
					R.DocketNumber,
					R.PlaceId,
					cast(R.[From] AS DATE) AS [Date],
					R.[From],
					CASE WHEN R.[To] < R.[From] THEN
						DATEADD(DAY, 1, R.[To])
					ELSE
						R.[To]
					END AS [To]
				FROM
				(
					SELECT
						R.DocketId,
						D.DocketNumber,
						D.PlaceId,
						DATEADD(ms, DATEDIFF(ms, '00:00:00', R.[From]), CONVERT(DATETIME, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)))) AS [From],
						DATEADD(ms, DATEDIFF(ms, '00:00:00', R.[To]), CONVERT(DATETIME, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)))) AS [To]
					FROM
						[dbo].[Docket] D WITH(NOLOCK)
						INNER JOIN [dbo].[DocketRange] R WITH(NOLOCK) ON R.DocketId = D.DocketId AND R.[From] IS NOT NULL AND R.[To] IS NOT NULL AND R.[DayOfWeek] = [dbo].[fn_weekday](DATEADD(DAY, -1, GETDATE()))
					WHERE
						D.Active = 1
						AND D.ApplyInOutControl = 1
						AND D.ClientId = @ClientId
						AND D.PlaceId = COALESCE(@PlaceId, D.PlaceId)

					UNION

					SELECT
						R.DocketId,
						D.DocketNumber,
						D.PlaceId,
						DATEADD(ms, DATEDIFF(ms, '00:00:00', R.[From]), CONVERT(DATETIME, CAST(GETDATE() AS DATE))) AS [From],
						DATEADD(ms, DATEDIFF(ms, '00:00:00', R.[To]), CONVERT(DATETIME, CAST(GETDATE() AS DATE))) AS [To]
					FROM
						[dbo].[Docket] D WITH(NOLOCK)
						INNER JOIN [dbo].[DocketRange] R WITH(NOLOCK) ON R.DocketId = D.DocketId AND R.[From] IS NOT NULL AND R.[To] IS NOT NULL AND R.[DayOfWeek] = [dbo].[fn_weekday](GETDATE())
					WHERE
						D.Active = 1
						AND D.ApplyInOutControl = 1
						AND D.ClientId = @ClientId
						AND D.PlaceId = COALESCE(@PlaceId, D.PlaceId)

					UNION

					SELECT
						R.DocketId,
						D.DocketNumber,
						D.PlaceId,
						DATEADD(ms, DATEDIFF(ms, '00:00:00', R.[From]), CONVERT(DATETIME, DATEADD(DAY, 1, CAST(GETDATE() AS DATE)))) AS [From],
						DATEADD(ms, DATEDIFF(ms, '00:00:00', R.[To]), CONVERT(DATETIME, DATEADD(DAY, 1, CAST(GETDATE() AS DATE)))) AS [To]
					FROM
						[dbo].[Docket] D WITH(NOLOCK)
						INNER JOIN [dbo].[DocketRange] R WITH(NOLOCK) ON R.DocketId = D.DocketId AND R.[From] IS NOT NULL AND R.[To] IS NOT NULL AND R.[DayOfWeek] = [dbo].[fn_weekday](DATEADD(DAY, 1, GETDATE()))
					WHERE
						D.Active = 1
						AND D.ApplyInOutControl = 1
						AND D.ClientId = @ClientId
						AND D.PlaceId = COALESCE(@PlaceId, D.PlaceId)
				) R
			) R
		WHERE
			CAST(R.[From] AS date) = CAST(GETDATE() AS date)
			AND DATEADD(MINUTE, -10, R.[From]) <= GETDATE()