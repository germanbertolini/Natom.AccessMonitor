using Natom.AccessMonitor.Sync.Transmitter.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Services
{
    public static class ActivationService
    {
        public static async Task<string> StartAsync()
        {
            var data = new StartActivationHandshakeDto
            {
                InstanceId = ConfigService.Config.InstanceId,
                InstallationAlias = ConfigService.Config.InstallationAlias,
                InstallerName = ConfigService.Config.InstallerName,
                ClientName = ConfigService.Config.ClientName,
                ClientCUIT = ConfigService.Config.ClientCUIT
            };
            Uri baseUri = new Uri(ConfigService.Config.ServiceURL);
            Uri startHandshakeUri = new Uri(baseUri, "Activation/Start");
            var response = await NetworkService.DoHttpPostAsync<dynamic>(startHandshakeUri.AbsoluteUri, data);
            if (!response.Success)
                throw new Exception($"Error en el servidor al iniciar handshake: {response.Error}");
            return response.Data.accessToken;
        }

        public static async Task WaitForAprobacionAsync()
        {
            while(true)
            {
                Uri baseUri = new Uri(ConfigService.Config.ServiceURL);
                Uri startHandshakeUri = new Uri(baseUri, "Activation/Status");
                var response = await NetworkService.DoHttpGetAsync<dynamic>(startHandshakeUri.AbsoluteUri);
                if (response.Success && response.Data.activated == true)
                    break;
                await Task.Delay(5000);
            }
        }

        public static void Activate()
        {
            ConfigService.Config.ActivatedAt = DateTime.Now;
            ConfigService.SaveConfig(ConfigService.Config);
        }

        public static void Inactivate()
        {
            ConfigService.Config.ActivatedAt = null;
            ConfigService.SaveConfig(ConfigService.Config);
        }

        public static async Task<string> ConfirmActivationAsync()
        {
            Uri baseUri = new Uri(ConfigService.Config.ServiceURL);
            Uri startHandshakeUri = new Uri(baseUri, "Activation/Confirm");
            var response = await NetworkService.DoHttpPostAsync<dynamic>(startHandshakeUri.AbsoluteUri);
            if (!response.Success)
                throw new Exception($"Error en el servidor al iniciar handshake: {response.Error}");
            return response.Data.accessToken;
        }
    }
}
