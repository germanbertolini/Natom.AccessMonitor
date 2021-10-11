using Dapper;
using Dapper.Contrib.Extensions;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Sync.Receiver.Entities.Models;
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
                await connection.InsertAsync(synchronizer);
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
    }
}
