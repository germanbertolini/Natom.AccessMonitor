using Anviz.SDK.Responses;
using Natom.AccessMonitor.Sync.Transmitter.DeviceWrappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Entities
{
    public class DeviceConfig
    {
        [JsonIgnore]
        public IDeviceWrapper ConnectionWrapper { get; set; }

        [JsonProperty("rid")]
        public string RelojId { get; set; }

        [JsonProperty("did")]
        public ulong DeviceId { get; set; }

        [JsonProperty("dht")]
        public string DeviceHost { get; set; }

        [JsonProperty("dpt")]
        public uint DevicePort { get; set; }

        [JsonProperty("us")]
        public string User { get; set; }

        [JsonProperty("pwd")]
        public string Password { get; set; }

        [JsonProperty("ac")]
        public bool AuthenticateConnection { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isb")]
        public string InstalledBy { get; set; }

        [JsonProperty("adt")]
        public DateTime AddedAt { get; set; }

        [JsonProperty("lca")]
        public DateTime? LastConfigUpdateAt { get; set; }

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
    }
}
