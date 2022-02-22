using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Entities.Results
{
    public class spSynchronizerRegisterSyncAndGetConfigResult
    {
        public int? NewSyncToServerMinutes { get; set; }
        public int? NewSyncFromDevicesMinutes { get; set; }
    }
}
