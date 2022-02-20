using Natom.AccessMonitor.WebApp.Admin.Backend.Model.Results;
using Natom.AccessMonitor.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Admin.Backend.DTO.Synchronizers
{
    public class DeviceDTO
    {
		[JsonProperty("encrypted_id")]
		public string EncryptedId { get; set; }

		[JsonProperty("device_id")]
		public string DeviceId { get; set; }

		[JsonProperty("device_name")]
		public string DeviceName { get; set; }

		[JsonProperty("device_last_configuration_at")]
		public DateTime? DeviceLastConfigurationAt { get; set; }

		[JsonProperty("device_serial_number")]
		public string DeviceSerialNumber { get; set; }

		[JsonProperty("device_model")]
		public string DeviceModel { get; set; }

		[JsonProperty("device_brand")]
		public string DeviceBrand { get; set; }

		[JsonProperty("device_firmware_version")]
		public string DeviceFirmwareVersion { get; set; }

		public DeviceDTO From(spDeviceListBySyncIdResult model)
		{
			this.EncryptedId = EncryptionService.Encrypt(model.Id);
			this.DeviceId = model.DeviceId;
			this.DeviceName = model.DeviceName;
			this.DeviceLastConfigurationAt = model.DeviceLastConfigurationAt;
			this.DeviceSerialNumber = model.DeviceSerialNumber;
			this.DeviceModel = model.DeviceModel;
			this.DeviceBrand = model.DeviceBrand;
			this.DeviceFirmwareVersion = model.DeviceFirmwareVersion;

			return this;
		}
	}
}
