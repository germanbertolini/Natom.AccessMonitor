using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Entities
{
    public class DeviceConfig
    {
        public ulong DeviceId { get; set; }
        public string DeviceHost { get; set; }
        public uint DevicePort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool AuthenticateConnection { get; set; }

        public string Name { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
