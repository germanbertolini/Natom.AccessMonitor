using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Entities
{
    public class TransmitterDTO
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class TransmitterDTO<TObject> : TransmitterDTO
    {
        [JsonProperty("data")]
        public TObject Data { get; set; }
    }
}
