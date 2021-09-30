using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Entities.DTO
{
    public class StartActivationHandshakeDto
    {
        public string InstanceId { get; set; }
        public string InstallationAlias { get; set; }
        public string InstallerName { get; set; }
        public string ClientName { get; set; }
        public string ClientCUIT { get; set; }
    }
}
