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
        private static TransmitterConfig _config = null;
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

        public static void Save(TransmitterConfig newConfig)
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
    }
}
