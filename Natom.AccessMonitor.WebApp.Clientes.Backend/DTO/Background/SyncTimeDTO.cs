using Natom.AccessMonitor.Core.Biz.Entities.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Background
{
    public class SyncTimeDTO
    {
        [JsonProperty("instance_id")]
        public string InstanceId { get; set; }

        [JsonProperty("installer_name")]
        public string InstallerName { get; set; }

        [JsonProperty("last_sync_at")]
        public DateTime? LastSyncAt { get; set; }

        [JsonProperty("next_sync_at")]
        public DateTime? NextSyncAt { get; set; }

        [JsonProperty("each_miliseconds")]
        public long EachMiliseconds { get; set; }

        [JsonProperty("next_on_miliseconds")]
        public long NextOnMiliseconds { get; set; }

        public SyncTimeDTO From(spSynchronizerSelectSyncTimesResult data)
        {
            InstanceId = data.InstanceId;
            InstallerName = data.InstallerName;
            LastSyncAt = data.LastSyncAt;
            NextSyncAt = data.NextSyncAt;
            EachMiliseconds = data.EachMiliseconds;
            NextOnMiliseconds = data.NextOnMiliseconds;
            return this;
        }
    }
}
