using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Entities.DTO
{
    public class MovementDto
    {
        [JsonProperty("dn")]
        public long DocketNumber { get; set; }

        [JsonProperty("t")]
        public string MovementType { get; set; }

        [JsonProperty("dt")]
        public DateTime DateTime { get; set; }
    }
}
