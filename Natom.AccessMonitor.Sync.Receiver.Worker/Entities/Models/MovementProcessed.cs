using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Models
{
    public class MovementProcessed
    {
		public DateTime Date { get; set; }
		public string DocketNumber { get; set; }
		public int? DocketId { get; set; }
		public int ExpectedPlaceId { get; set; }
		public TimeSpan ExpectedIn { get; set; }
		public DateTime? In { get; set; }
		public int? InGoalId { get; set; }
		public int? InPlaceId { get; set; }
		public int? InDeviceId { get; set; }
		public string InDeviceMovementType { get; set; }
		public bool? InWasEstimated { get; set; }
		public DateTime? InProcessedAt { get; set; }

		public TimeSpan ExpectedOut { get; set; }
		public DateTime? Out { get; set; }
		public int? OutGoalId { get; set; }
		public int? OutPlaceId { get; set; }
		public int? OutDeviceId { get; set; }
		public string OutDeviceMovementType { get; set; }
		public bool? OutWasEstimated { get; set; }
		public DateTime? OutProcessedAt { get; set; }

		public TimeSpan? PermanenceTime { get; set; }
    }
}
