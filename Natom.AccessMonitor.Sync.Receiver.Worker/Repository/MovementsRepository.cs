using Dapper;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Sync.Entities.DTO;
using Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Models;
using Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Results;
using Natom.AccessMonitor.Sync.Receiver.Worker.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker.Repository
{
    public class MovementsRepository
    {
        private string _connectionString;

        public MovementsRepository(ConfigurationService configuration)
        {
            _connectionString = configuration.GetValueAsync("ConnectionStrings.DbMaster").GetAwaiter().GetResult();
        }

        public async Task BulkInsertAsync(int clientId, string syncInstanceId, DataBlockForSyncDto dataBlock, int? deviceGoalId)
        {
            var sql = "INSERT INTO [dbo].[zMovement_Client" + clientId.ToString().PadLeft(3, '0') + "] " +
                        "   ( " +
                        "       [InstanceId], " +
                        "       [DeviceId], " +
                        "       [DateTime], " +
                        "       [DocketNumber], " +
                        "       [MovementType], " +
                        "       [GoalId], " +
                        "       [ProcessedAt] " +
                        "   ) " +
                        "VALUES " +
                        "   (@InstanceId " +
                        "       ,@DeviceId " +
                        "       ,@DateTime " +
                        "       ,@DocketNumber " +
                        "       ,@MovementType " +
                        "       ,@GoalId " +
                        "       ,NULL); ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.RetryableExecuteAsync($"EXEC [dbo].[sp_movements_preinsert] @ClientId = {clientId}");

                try
                {
                    if (dataBlock.Movements != null)
                    {
                        await connection.RetryableExecuteAsync(sql, from m in dataBlock.Movements
                                                                    select new
                                                                    {
                                                                        InstanceId = syncInstanceId,
                                                                        DeviceId = dataBlock.DeviceId,
                                                                        DateTime = m.DateTime,
                                                                        DocketNumber = m.DocketNumber,
                                                                        MovementType = m.MovementType,
                                                                        GoalId = deviceGoalId
                                                                    });
                    }
                }
                catch (Exception ex)
                {
                    bool isUniqueIndexRestriction = ex is SqlException && ((SqlException)ex).Number.Equals(2601);
                    if (!isUniqueIndexRestriction)
                        throw ex;
                }
            }                
        }
        
        public async Task<spMovementsGetOutPromedioResult> GetOutPromedioAsync(int clientId, int docketId)
        {
            var result = new spMovementsGetOutPromedioResult();

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "EXEC [dbo].[sp_movements_get_out_promedio] @ClientId, @DocketId";
                using (var results = connection.QueryMultiple(sql, new { ClientId = clientId, DocketId = docketId }))
                {
                    result = (await results.ReadAsync<spMovementsGetOutPromedioResult>()).First();
                }
            }
            return result;
        }

        public async Task<spMovementsProcessorSelectByClientResult> GetPendingToProcessByClientAsync(int clientId)
        {
            var movements = new spMovementsProcessorSelectByClientResult();
            movements.Movements = new List<spMovementsProcessorSelectByClientDetailMovementsResult>();
            movements.DocketsRanges = new List<spMovementsProcessorSelectByClientDetailDocketRangesResult>();

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var sql = "EXEC [dbo].[sp_movements_processor_select_by_client] @ClientId";
                    using (var results = connection.QueryMultiple(sql, new { ClientId = clientId }))
                    {
                        movements.Movements = (await results.ReadAsync<spMovementsProcessorSelectByClientDetailMovementsResult>()).ToList();
                        if (movements.Movements.Count > 0)
                        {
                            movements.DocketsRanges = (await results.ReadAsync<spMovementsProcessorSelectByClientDetailDocketRangesResult>()).ToList();
                            movements.LastInOut = (await results.ReadAsync<MovementProcessed>()).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    bool tableNotExistsYet = ex is SqlException && ((SqlException)ex).Number.Equals(208);
                    if (!tableNotExistsYet)
                        throw ex;
                }
            }
            return movements;
        }

        public async Task ProcessedUpdateAsync(int clientId, List<MovementProcessed> movementsToUpdate, CancellationToken cancellationToken)
        {
            if (movementsToUpdate.Count == 0)
                return;

            var sql = "UPDATE [dbo].[zMovement_Client" + clientId.ToString().PadLeft(3, '0') + "_Processed] " +
                        " SET " +
                        "       [Out] = @Out, " +
                        "       [OutGoalId] = @OutGoalId, " +
                        "       [OutPlaceId] = @OutPlaceId, " +
                        "       [OutDeviceId] = @OutDeviceId, " +
                        "       [OutDeviceMovementType] = @OutDeviceMovementType, " +
                        "       [OutWasEstimated] = @OutWasEstimated, " +
                        "       [OutProcessedAt] = @OutProcessedAt, " +
                        "       [PermanenceTime] = @PermanenceTime " +
                        " WHERE " +
                        "       [Date] = @Date " +
                        "       AND [DocketNumber] = @DocketNumber " +
                        "       AND [In] = @In ";

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    
                    await connection.RetryableExecuteAsync(sql, from m in movementsToUpdate
                                                                select new
                                                                {
                                                                    Date = m.Date,
                                                                    DocketNumber = m.DocketNumber,
                                                                    In = m.In,
                                                                    Out = m.Out,
                                                                    OutGoalId = m.OutGoalId,
                                                                    OutPlaceId = m.OutPlaceId,
                                                                    OutDeviceId = m.OutDeviceId,
                                                                    OutDeviceMovementType = m.OutDeviceMovementType,
                                                                    OutWasEstimated = m.OutWasEstimated,
                                                                    OutProcessedAt = m.OutProcessedAt,
                                                                    PermanenceTime = m.PermanenceTime
                                                                },
                                                                cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    bool isUniqueIndexRestriction = ex is SqlException && ((SqlException)ex).Number.Equals(2601);
                    if (!isUniqueIndexRestriction)
                        throw ex;
                }
            }
        }

        public async Task ProcessedInsertAsync(int clientId, List<MovementProcessed> movementsToAdd, CancellationToken cancellationToken)
        {
            if (movementsToAdd.Count == 0)
                return;

            var sql = "INSERT INTO [dbo].[zMovement_Client" + clientId.ToString().PadLeft(3, '0') + "_Processed] " +
                        "   ( " +
                        "       [Date], " +
                        "       [DocketNumber], " +
                        "       [DocketId], " +
                        "       [ExpectedPlaceId], " +
                        "       [ExpectedIn], " +
                        "       [In], " +
                        "       [InGoalId], " +
                        "       [InPlaceId], " +
                        "       [InDeviceId], " +
                        "       [InDeviceMovementType], " +
                        "       [InWasEstimated], " +
                        "       [InProcessedAt], " +
                        "       [ExpectedOut], " +
                        "       [Out], " +
                        "       [OutGoalId], " +
                        "       [OutPlaceId], " +
                        "       [OutDeviceId], " +
                        "       [OutDeviceMovementType], " +
                        "       [OutWasEstimated], " +
                        "       [OutProcessedAt], " +
                        "       [PermanenceTime] " +
                        "   ) " +
                        "VALUES " +
                        "   ( " +
                        "       @Date, " +
                        "       @DocketNumber, " +
                        "       @DocketId, " +
                        "       @ExpectedPlaceId, " +
                        "       @ExpectedIn, " +
                        "       @In, " +
                        "       @InGoalId, " +
                        "       @InPlaceId, " +
                        "       @InDeviceId, " +
                        "       @InDeviceMovementType, " +
                        "       @InWasEstimated, " +
                        "       @InProcessedAt, " +
                        "       @ExpectedOut, " +
                        "       @Out, " +
                        "       @OutGoalId, " +
                        "       @OutPlaceId, " +
                        "       @OutDeviceId, " +
                        "       @OutDeviceMovementType, " +
                        "       @OutWasEstimated, " +
                        "       @OutProcessedAt, " +
                        "       @PermanenceTime " +
                        "   ); ";

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.RetryableExecuteAsync(sql, from m in movementsToAdd
                                                                select new
                                                                {
                                                                    Date = m.Date,
                                                                    DocketNumber = m.DocketNumber,
                                                                    DocketId = m.DocketId,
                                                                    ExpectedPlaceId = m.ExpectedPlaceId,
                                                                    ExpectedIn = m.ExpectedIn,
                                                                    In = m.In,
                                                                    InGoalId = m.InGoalId,
                                                                    InPlaceId = m.InPlaceId,
                                                                    InDeviceId = m.InDeviceId,
                                                                    InDeviceMovementType = m.InDeviceMovementType,
                                                                    InWasEstimated = m.InWasEstimated,
                                                                    InProcessedAt = m.InProcessedAt,
                                                                    ExpectedOut = m.ExpectedOut,
                                                                    Out = m.Out,
                                                                    OutGoalId = m.OutGoalId,
                                                                    OutPlaceId = m.OutPlaceId,
                                                                    OutDeviceId = m.OutDeviceId,
                                                                    OutDeviceMovementType = m.OutDeviceMovementType,
                                                                    OutWasEstimated = m.OutWasEstimated,
                                                                    OutProcessedAt = m.OutProcessedAt,
                                                                    PermanenceTime = m.PermanenceTime
                                                                },
                                                                cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    bool isUniqueIndexRestriction = ex is SqlException && (((SqlException)ex).Number.Equals(2601));
                    if (!isUniqueIndexRestriction)
                        throw ex;
                }
            }
        }

        public async Task MarkAsProcessedAsync(int clientId, List<long> movementIds)
        {
            if (movementIds.Count == 0 || clientId == 0)
                return;

            var sql = "UPDATE [dbo].[zMovement_Client" + clientId.ToString().PadLeft(3, '0') + "] WITH(READPAST)" +
                        " SET " +
                        "       ProcessedAt = GETDATE() " +
                        " WHERE " +
                        "       [MovementId] IN (" + String.Join(',', movementIds) + ") ";

            using (var connection = new SqlConnection(_connectionString))
            {
                if (movementIds.Count > 0)
                    await connection.RetryableExecuteAsync(sql);
            }
        }
    }
}
