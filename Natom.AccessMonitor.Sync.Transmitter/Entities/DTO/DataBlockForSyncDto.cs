using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Entities.DTO
{
    public class DataBlockForSyncDto
    {
        [JsonProperty("i")]
        public long DeviceId { get; set; }

        [JsonProperty("n")]
        public string DeviceName { get; set; }

        [JsonProperty("lca")]
        public DateTime LastConfigurationAt { get; set; }

        [JsonProperty("sn")]
        public string DeviceSerialNumber { get; set; }

        [JsonProperty("mdl")]
        public string DeviceModel { get; set; }

        [JsonProperty("brd")]
        public string DeviceBrand { get; set; }

        [JsonProperty("dtf")]
        public string DeviceDateTimeFormat { get; set; }

        [JsonProperty("fvs")]
        public string DeviceFirmwareVersion { get; set; }

        [JsonProperty("l")]
        public List<MovementDto> Movements { get; set; }
    }
}
