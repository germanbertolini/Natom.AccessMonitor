using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Entities.DTO
{
    public class TransmitterResponseDto
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("config")]
        public dynamic Config { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class TransmitterResponseDto<TObject> : TransmitterResponseDto
    {
        [JsonProperty("data")]
        public TObject Data { get; set; }
    }
}
