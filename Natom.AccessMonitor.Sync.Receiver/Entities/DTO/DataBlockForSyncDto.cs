using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Entities.DTO
{
    public class DataBlockForSyncDto
    {
        [JsonProperty("i")]
        public ulong DeviceId { get; set; }

        [JsonProperty("n")]
        public string DeviceName { get; set; }

        [JsonProperty("l")]
        public List<MovementDto> Movements { get; set; }
    }
}
