using Dapper;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Sync.Receiver.Worker.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker.Repository
{
    public class SynchronizerRepository
    {
        private string _connectionString;

        public SynchronizerRepository(ConfigurationService configuration)
        {
            _connectionString = configuration.GetValueAsync("ConnectionStrings.DbSecurity").GetAwaiter().GetResult();
        }

        public async Task AddOrUpdateDeviceInfoAsync(string syncInstanceId, long deviceId, string deviceName, DateTime? lastConfigurationAt,
                                                        string serialNumber, string model, string brand, string dateTimeFormat, string firmwareVersion,
                                                        string ip, string user, string pass, DateTime lastSyncRegistered)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.RetryableExecuteAsync("EXEC [dbo].[sp_device_add_or_update] @InstanceId, @DeviceId, @DeviceName, @LastConfigurationAt, @SerialNumber, @Model, @Brand, @DateTimeFormat, @FirmwareVersion, @IP, @User, @Pass, @LastSyncRegistered",
                                                            new {
                                                                InstanceId = syncInstanceId,
                                                                DeviceId = deviceId,
                                                                DeviceName = deviceName,
                                                                LastConfigurationAt = lastConfigurationAt,
                                                                SerialNumber = serialNumber,
                                                                Model = model,
                                                                Brand = brand,
                                                                DateTimeFormat = dateTimeFormat,
                                                                FirmwareVersion = firmwareVersion,
                                                                IP = ip,
                                                                User = user,
                                                                Pass = pass,
                                                                LastSyncRegistered = lastSyncRegistered
                                                            });
            }
        }
    }
}
