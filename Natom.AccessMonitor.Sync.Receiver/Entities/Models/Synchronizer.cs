using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Entities.Models
{
	[Table("Synchronizer")]
	public class Synchronizer
	{
		public string InstanceId { get; set; }
		public string InstallationAlias { get; set; }
		public string InstallerName { get; set; }
		public DateTime InstalledAt { get; set; }
		public DateTime? ActivatedAt { get; set; }
		public int? CurrentSyncToServerMinutes { get; set; }
		public int? NewSyncToServerMinutes { get; set; }
		public int? CurrentSyncFromDevicesMinutes { get; set; }
		public int? NewSyncFromDevicesMinutes { get; set; }
		public int? ClientId { get; set; }
		public DateTime? RemovedAt { get; set; }
	}
}
