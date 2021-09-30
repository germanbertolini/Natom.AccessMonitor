﻿using Newtonsoft.Json;
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

        [JsonProperty("p")]
        public string InstallationAlias { get; set; }

        [JsonProperty("e")]
        public string InstallerName { get; set; }

        [JsonProperty("r")]
        public string ServiceURL { get; set; }

        [JsonProperty("cn")]
        public string ClientName { get; set; }

        [JsonProperty("cu")]
        public string ClientCUIT { get; set; }
    }
}
