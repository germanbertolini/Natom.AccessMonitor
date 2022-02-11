﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Admin.Backend.Model.Results
{
    public class spDeviceListBySyncIdResult
    {
		public string DeviceId { get; set; }
		public string DeviceName { get; set; }
		public DateTime? DeviceLastConfigurationAt { get; set; }
		public string DeviceSerialNumber { get; set; }
		public string DeviceModel { get; set; }
		public string DeviceBrand { get; set; }
		public string DeviceFirmwareVersion { get; set; }

		public int TotalFiltrados { get; set; }
		public int TotalRegistros { get; set; }
	}
}