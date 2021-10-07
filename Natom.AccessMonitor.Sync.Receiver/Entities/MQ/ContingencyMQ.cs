using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Entities.MQ
{
    public class ContingencyMQ
    {
        public QueueMQ QueueMQ { get; set; }
        public MessageMQ MessageMQ { get; set; }
    }
}
