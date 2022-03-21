using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Services.MQ.WorkerUtilities.Config
{
    public class WorkerMQConfig : WorkerConfig
    {
        public QueueConfig Queue { get; set; }
    }
}
