using Natom.AccessMonitor.Core.Biz.Entities.Results;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Syncs
{
    public class DeviceSyncConfigDTO
    {
        [JsonProperty("encrypted_instance_id")]
        public string EncryptedInstanceId { get; set; }

        [JsonProperty("interval_mins_to_server")]
        public int? IntervalMinsToServer { get; set; }

        [JsonProperty("interval_mins_from_device")]
        public int? IntervalMinsFromDevice { get; set; }

        [JsonProperty("last_sync")]
        public DateTime? LastSync { get; set; }

        [JsonProperty("next_sync")]
        public DateTime? NextSync { get; set; }

        public DeviceSyncConfigDTO From(spSynchronizerSelectConfigByIdResult config)
        {
            return new DeviceSyncConfigDTO
            {
                EncryptedInstanceId = EncryptionService.Encrypt(config.InstanceId),
                IntervalMinsFromDevice = config.SyncFromDevicesMinutes,
                IntervalMinsToServer = config.SyncToServerMinutes,
                LastSync = config.LastSyncAt,
                NextSync = config.NextSyncAt
            };
        }
    }
}
