using Dapper;
using Dapper.Contrib.Extensions;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Sync.Receiver.Entities.Models;
using Natom.AccessMonitor.Sync.Receiver.Entities.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Repositories
{
    public class SynchronizerRepository
    {
        private readonly string _connectionString;

        public SynchronizerRepository(ConfigurationService configuration)
        {
            _connectionString = configuration.GetValueAsync("ConnectionStrings.DbSecurity").GetAwaiter().GetResult();
        }

        public async Task InsertAsPendingAsync(Synchronizer synchronizer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAsync(synchronizer);
                }
                catch (SqlException ex)
                {
                    if (!ex.Message.Contains("Violation of PRIMARY KEY constraint 'PK_Synchronizer'"))
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task MarkAsCompletedAsync(string syncInstanceId, int clientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("UPDATE Synchronizer SET ActivatedAt = @ActivatedAt, ClientId = @ClientId WHERE InstanceId = @InstanceId",
                                                new
                                                {
                                                    ActivatedAt = DateTime.Now,
                                                    ClientId = clientId,
                                                    InstanceId = syncInstanceId
                                                });
            }
        }

        public async Task<spSynchronizerRegisterSyncAndGetConfigResult> RegisterSyncAndGetConfigAsync(string syncInstanceId, int? currentSyncToServerMinutes, int? currentSyncFromDevicesMinutes)
        {
            spSynchronizerRegisterSyncAndGetConfigResult config = null;
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "EXEC [dbo].[sp_synchronizer_register_sync_and_get_config] @InstanceId, @CurrentSyncToServerMinutes, @CurrentSyncFromDevicesMinutes";
                var _params = new { InstanceId = syncInstanceId, CurrentSyncToServerMinutes = currentSyncToServerMinutes, CurrentSyncFromDevicesMinutes = currentSyncFromDevicesMinutes };
                config = (await db.QueryAsync<spSynchronizerRegisterSyncAndGetConfigResult>(sql, _params)).First();
            }
            return config;
        }

        public async Task RegisterConnectionAsync(string syncInstanceId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "EXEC [dbo].[sp_synchronizer_register_connection] @InstanceId";
                var _params = new { InstanceId = syncInstanceId };
                await db.ExecuteAsync(sql, _params);
            }
        }
    }
}
