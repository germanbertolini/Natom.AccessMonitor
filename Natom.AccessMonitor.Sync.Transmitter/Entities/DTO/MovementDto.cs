using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Entities.DTO
{
    public class MovementDto
    {
        [JsonProperty("dn")]
        public ulong DocketNumber { get; set; }

        [JsonProperty("t")]
        public string MovementType { get; set; }

        [JsonProperty("dt")]
        public DateTime DateTime { get; set; }
    }
}
