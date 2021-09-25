using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Entities
{
    public class TransmitterConfig
    {
        [JsonProperty("z")]
        public string InstanceId { get; set; }

        [JsonProperty("r")]
        public string ServiceURL { get; set; }
    }
}
