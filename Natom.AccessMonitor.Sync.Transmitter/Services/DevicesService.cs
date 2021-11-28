using Anviz.SDK;
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
        private static Dictionary<string, DateTime> _lastSynchronizing = null;


        public static bool IsSynchronizing() => _synchronizingDevices || _synchronizingServer;

        public static void SetLastDeviceSynchronization(string relojId, DateTime lastSync)
        {
            if (_lastSynchronizing == null)
                _lastSynchronizing = new Dictionary<string, DateTime>();

            if (!_lastSynchronizing.ContainsKey(relojId))
                _lastSynchronizing.Add(relojId, lastSync);

            _lastSynchronizing[relojId] = lastSync;

            try
            {
                var data = JsonConvert.SerializeObject(_lastSynchronizing);
                var dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
                data = System.Convert.ToBase64String(dataBytes);
                data = EncryptService.Encrypt(data, cStoragePassword);
                System.IO.File.WriteAllText("synchronization.info", data);
            }
            catch (Exception ex)
            {
                string e = ex.ToString();
                //NADA, VOLVERA A REINTENTAR EN EL PROXIMO LLAMADO
            }
        }

        public static Dictionary<string, DateTime> GetLastDeviceSynchronization()
        {
            if (_lastSynchronizing == null && File.Exists("synchronization.info"))
            {
                try
                {
                    var data = System.IO.File.ReadAllText("synchronization.info");
                    data = EncryptService.Decrypt(data, cStoragePassword);
                    var dataBytes = System.Convert.FromBase64String(data);
                    data = System.Text.Encoding.UTF8.GetString(dataBytes);
                    _lastSynchronizing = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(data);
                }
                catch (Exception ex)
                {
                    string e = ex.ToString();
                    /* NADA */
                }
            }

            if (_lastSynchronizing == null)
                _lastSynchronizing = new Dictionary<string, DateTime>();

            return _lastSynchronizing;
        }

        public static async Task<List<DeviceConfig>> GetConnectedDevicesAsync(List<DeviceConfig> devices)
        {
            var tasks = new List<Task>();
            var connectedDevices = new List<DeviceConfig>();
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            ConfigService.Devices.ForEach(device =>
            {
                tasks.Add(Task.Run(() =>
                {
                    var connected = DevicesService.ValidarConexionAsync(device).GetAwaiter().GetResult();
                    if (connected && !cancellationToken.IsCancellationRequested)
                        connectedDevices.Add(device);
                }, cancellationToken));
            });

            while (!cancellationToken.IsCancellationRequested && tasks.Any(task => !task.IsCompleted && !task.IsFaulted && !task.IsCanceled))
                Task.Delay(100).Wait();

            return connectedDevices;
        }

        public static async Task<bool> ValidarConexionAsync(DeviceConfig device)
        {
            bool valido = false;

            try
            {
                await DevicesService.ThrowIfConexionInvalidaAsync(device);
                valido = true;
            }
            catch (Exception ex)
            {
                //NADA
            }

            return valido;
        }

        public static async Task<DeviceConfig> ThrowIfConexionInvalidaAsync(DeviceConfig device)
        {
            DeviceConfig toReturn = null;

            try
            {


                var manager = new AnvizManager();
                manager.ConnectionUser = device.User;
                manager.ConnectionPassword = device.Password;
                manager.AuthenticateConnection = device.AuthenticateConnection;

                var anvizDevice = await manager.TryConnection(device.DeviceHost, (int)device.DevicePort);

                toReturn = new DeviceConfig
                {
                    DeviceId = anvizDevice.DeviceId
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return toReturn;
        }

        public static void GetAndStoreRecordsFromDevices(List<DeviceConfig> devices, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(cStorageFolderName))
                Directory.CreateDirectory(cStorageFolderName);

            _synchronizingDevices = true;

            //INICIO MOCK
            //Dictionary<ulong, bool> statusPersonas = new Dictionary<ulong, bool>();
            //var random = new Random();
            //FIN MOCK

            foreach (var device in devices)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = new DataBlockForSyncDto();
                data.DeviceId = (long)device.DeviceId;
                data.DeviceName = device.Name;

                //INICIO MOCK
                //data.Movements = new List<MovementDto>();
                //for (int i = 0; i < 200000; i ++)
                //{
                //    ulong docketNumber = (ulong)random.Next(0, 10);
                //    if (!statusPersonas.ContainsKey(docketNumber))
                //        statusPersonas.Add(docketNumber, true);
                //    else
                //        statusPersonas[docketNumber] = !statusPersonas[docketNumber];

                //    data.Movements.Add(new MovementDto()
                //    {
                //        DateTime = DateTime.Now.AddSeconds(-random.Next(1, 50)),
                //        DocketNumber = (long)docketNumber,
                //        MovementType = statusPersonas[docketNumber] ? "I" : "O"
                //    });

                //    cancellationToken.ThrowIfCancellationRequested();
                //}
                //FIN MOCK

                var strData = JsonConvert.SerializeObject(data);
                var dataBytes = System.Text.Encoding.UTF8.GetBytes(strData);
                strData = System.Convert.ToBase64String(dataBytes);
                strData = EncryptService.Encrypt(strData, cStoragePassword);
                System.IO.File.WriteAllText($"{cStorageFolderName}\\{Guid.NewGuid():N}", strData);

                SetLastDeviceSynchronization(device.RelojId, DateTime.Now);
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
