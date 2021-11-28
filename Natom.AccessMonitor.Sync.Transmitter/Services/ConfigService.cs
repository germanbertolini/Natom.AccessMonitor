using Natom.AccessMonitor.Sync.Transmitter.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Services
{
    public static class ConfigService
    {
        private const string cDevicesPassword = "D3v1c3**5s";

        private static TransmitterConfig _config = null;
        private static List<DeviceConfig> _devices = null;

        public static TransmitterConfig Config
        {
            get
            {
                if (_config == null)
                {
                    if (System.IO.File.Exists("transmitter.cfg"))
                    {
                        try
                        {
                            var config = System.IO.File.ReadAllText("transmitter.cfg");
                            var passwd = NetworkService.GetMacAddress().ToString();
                            config = EncryptService.Decrypt(config, passwd);
                            var configBytes = System.Convert.FromBase64String(config);
                            config = System.Text.Encoding.UTF8.GetString(configBytes);
                            _config = JsonConvert.DeserializeObject<TransmitterConfig>(config);
                        }
                        catch (Exception ex)
                        {
                            _config = null;
                        }
                    }
                }
                return _config;
            }
        }

        public static void SaveConfig(TransmitterConfig newConfig)
        {
            if (string.IsNullOrEmpty(newConfig.InstanceId))
                newConfig.InstanceId = Guid.NewGuid().ToString("N");

            var config = JsonConvert.SerializeObject(newConfig);
            var configBytes = System.Text.Encoding.UTF8.GetBytes(config);
            config = System.Convert.ToBase64String(configBytes);
            var passwd = NetworkService.GetMacAddress().ToString();
            config = EncryptService.Encrypt(config, passwd);
            System.IO.File.WriteAllText("transmitter.cfg", config);
        }

        public static List<DeviceConfig> Devices
        {
            get
            {
                if (_devices == null)
                {
                    if (System.IO.File.Exists("devices.cfg"))
                    {
                        try
                        {
                            var devices = System.IO.File.ReadAllText("devices.cfg");
                            devices = EncryptService.Decrypt(devices, cDevicesPassword);
                            var devicesBytes = System.Convert.FromBase64String(devices);
                            devices = System.Text.Encoding.UTF8.GetString(devicesBytes);
                            _devices = JsonConvert.DeserializeObject<List<DeviceConfig>>(devices);

                            _devices.ForEach(device =>
                            {
                                if (device.DeviceBrand.ToLower().Equals("anviz"))
                                    device.ConnectionWrapper = new DeviceWrappers.AnvizDeviceWrapper(device);
                            });
                        }
                        catch (Exception ex)
                        {
                            _devices = null;
                        }
                    }
                }
                return _devices;
            }
        }

        public static void SaveDevices(List<DeviceConfig> newDevices)
        {
            var devices = JsonConvert.SerializeObject(newDevices);
            var devicesBytes = System.Text.Encoding.UTF8.GetBytes(devices);
            devices = System.Convert.ToBase64String(devicesBytes);
            devices = EncryptService.Encrypt(devices, cDevicesPassword);
            System.IO.File.WriteAllText("devices.cfg", devices);

            _devices = newDevices;

            _devices.ForEach(device =>
            {
                if (device.ConnectionWrapper == null)
                {
                    if (device.DeviceBrand.ToLower().Equals("anviz"))
                        device.ConnectionWrapper = new DeviceWrappers.AnvizDeviceWrapper(device);
                }
            });
        }
    }
}
