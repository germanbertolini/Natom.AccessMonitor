using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Services.MQ.WorkerUtilities.Config
{
    public class WorkerMQConfig
    {
        public string Name { get; set; }
        public string InstanceName { get; set; }
        public QueueConfig Queue { get; set; }
        public ProcessConfig Process { get; set; }
    }
}
