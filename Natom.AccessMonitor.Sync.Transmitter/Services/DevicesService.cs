using Natom.AccessMonitor.Sync.Transmitter.Entities;
using Natom.AccessMonitor.Sync.Transmitter.Entities.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Services
{
    public static class DevicesService
    {
        private const string cStorageFolderName = "SyncQueue";
        private const string cStoragePassword = "S1ncr0n1z4c10N";
        private static bool _synchronizingDevices = false;
        private static bool _synchronizingServer = false;

        public static bool IsSynchronizing() => _synchronizingDevices || _synchronizingServer;

        public static Task<List<DeviceConfig>> GetDisconnectedDevicesAsync(List<DeviceConfig> devices)
        {
            return Task.FromResult(new List<DeviceConfig>());
        }

        public static void GetAndStoreRecordsFromDevices(List<DeviceConfig> devices, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(cStorageFolderName))
                Directory.CreateDirectory(cStorageFolderName);

            _synchronizingDevices = true;

            //INICIO MOCK
            Dictionary<ulong, bool> statusPersonas = new Dictionary<ulong, bool>();
            var random = new Random();
            //FIN MOCK

            foreach (var device in devices)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = new DataBlockForSyncDto();
                data.DeviceId = device.DeviceId;
                data.DeviceName = device.Name;

                //INICIO MOCK
                data.Movements = new List<MovementDto>();
                for (int i = 0; i < 200000; i ++)
                {
                    ulong docketNumber = (ulong)random.Next(0, 10);
                    if (!statusPersonas.ContainsKey(docketNumber))
                        statusPersonas.Add(docketNumber, true);
                    else
                        statusPersonas[docketNumber] = !statusPersonas[docketNumber];

                    data.Movements.Add(new MovementDto()
                    {
                        DateTime = DateTime.Now.AddSeconds(-random.Next(1, 50)),
                        DocketNumber = docketNumber,
                        MovementType = statusPersonas[docketNumber] ? "I" : "O"
                    });

                    cancellationToken.ThrowIfCancellationRequested();
                }
                //FIN MOCK

                var strData = JsonConvert.SerializeObject(data);
                var dataBytes = System.Text.Encoding.UTF8.GetBytes(strData);
                strData = System.Convert.ToBase64String(dataBytes);
                strData = EncryptService.Encrypt(strData, cStoragePassword);
                System.IO.File.WriteAllText($"{cStorageFolderName}\\{Guid.NewGuid():N}", strData);
            }

            _synchronizingDevices = false;
        }

        public static async Task SyncStoredDataToServerAsync(List<DeviceConfig> devices, CancellationToken cancellationToken)
        {
            _synchronizingServer = true;

            Uri baseUri = new Uri(ConfigService.Config.ServiceURL);
            Uri uploadUri = new Uri(baseUri, "Sync/Upload");

            var directory = new DirectoryInfo(cStorageFolderName);
            var files = directory.GetFiles()
                                .OrderBy(x => x.CreationTime)
                                .Take(100)
                                .Select(x => x.Name)
                                .ToList();

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var content = System.IO.File.ReadAllText($"{cStorageFolderName}\\{file}");
                content = EncryptService.Decrypt(content, cStoragePassword);
                var contentBytes = System.Convert.FromBase64String(content);
                content = System.Text.Encoding.UTF8.GetString(contentBytes);

                int retryCounter = 0;
                retry:
                var result = await NetworkService.DoHttpPostAsync(uploadUri.AbsoluteUri, content);
                if (result.Success)
                    File.Delete($"{cStorageFolderName}\\{file}");
                else
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    retryCounter++;
                    if (retryCounter == 3)
                        continue;
                    else
                        goto retry;
                }

                await Task.Delay(1000);
            }

            _synchronizingServer = false;
        }
    }
}
