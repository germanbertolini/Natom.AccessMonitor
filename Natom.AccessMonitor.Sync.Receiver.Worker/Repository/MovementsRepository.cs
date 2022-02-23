using Dapper;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Sync.Entities.DTO;
using Natom.AccessMonitor.Sync.Receiver.Worker.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
            var sql = "INSERT INTO [dbo].[zMovement_Client" + clientId + "] " +
                        "   ( " +
                        "       [InstanceId], " +
                        "       [DeviceId], " +
                        "       [DateTime], " +
                        "       [DocketNumber], " +
                        "       [MovementType], " +
                        "       [GoalId] " +
                        "   ) " +
                        "VALUES " +
                        "   (@InstanceId " +
                        "       ,@DeviceId " +
                        "       ,@DateTime " +
                        "       ,@DocketNumber " +
                        "       ,@MovementType " +
                        "       ,@GoalId); ";

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
                    bool isDuplicatedPK = ex is SqlException && ((SqlException)ex).Number.Equals(2627);
                    if (!isDuplicatedPK)
                        throw ex;
                }
            }                
        }
    }
}
