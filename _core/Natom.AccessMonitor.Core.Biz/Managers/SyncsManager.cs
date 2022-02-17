using Dapper;
using Microsoft.Data.SqlClient;
using Natom.AccessMonitor.Core.Biz.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Managers
{
    public class SyncsManager : BaseManager
    {
        public SyncsManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public async Task<List<spDeviceListByClientIdResult>> ListDevicesByClienteAsync(string search, int skip, int take, int clienteId)
        {
            var connectionString = await _configuration.GetValueAsync("ConnectionStrings.DbSecurity");

            List<spDeviceListByClientIdResult> devices = null;
            using (var db = new SqlConnection(connectionString))
            {
                var sql = "EXEC [dbo].[sp_device_list_by_clientid] @ClienteId, @Search, @Skip, @Take";
                var _params = new { ClienteId = clienteId, Search = search, Skip = skip, Take = take };
                devices = (await db.QueryAsync<spDeviceListByClientIdResult>(sql, _params)).ToList();
            }
            return devices;
        }
    }
}
