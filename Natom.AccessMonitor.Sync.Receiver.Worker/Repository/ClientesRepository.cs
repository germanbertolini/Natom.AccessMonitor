using Natom.Extensions.Configuration.Services;
using Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Results;
using Natom.AccessMonitor.Sync.Receiver.Worker.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker.Repository
{
    public class ClientesRepository
    {
        private string _connectionString;

        public ClientesRepository(ConfigurationService configuration)
        {
            _connectionString = configuration.GetValueAsync("ConnectionStrings.DbMaster").GetAwaiter().GetResult();
        }

        public async Task<List<spClientesSelectAllResult>> GetListAsync()
        {
            var clientes = new List<spClientesSelectAllResult>();
            using (var connection = new SqlConnection(_connectionString))
            {
                clientes = (await connection.RetryableQueryAsync<spClientesSelectAllResult>($"EXEC [dbo].[sp_clientes_select_all]")).ToList();
            }
            return clientes;
        }
    }
}
