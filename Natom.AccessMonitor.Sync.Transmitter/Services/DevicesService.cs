﻿using Anviz.SDK;
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
using System.Web;

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
                    var connected = device.ConnectionWrapper.TryConnectionAsync().GetAwaiter().GetResult();
                    if (connected && !cancellationToken.IsCancellationRequested)
                        connectedDevices.Add(device);
                }, cancellationToken));
            });

            while (!cancellationToken.IsCancellationRequested && tasks.Any(task => !task.IsCompleted && !task.IsFaulted && !task.IsCanceled))
                Task.Delay(100).Wait();

            return connectedDevices;
        }

        public static async Task GetAndStoreRecordsFromDevicesAsync(List<DeviceConfig> devices, CancellationToken cancellationToken, bool resyncAllRegisters = false)
        {
            if (!Directory.Exists(cStorageFolderName))
                Directory.CreateDirectory(cStorageFolderName);

            _synchronizingDevices = true;


#if DEBUG   
            //INICIO MOCK
            Dictionary<ulong, bool> statusPersonas = new Dictionary<ulong, bool>();
            var random = new Random();
            //FIN MOCK
#endif      


            foreach (var device in devices)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = new DataBlockForSyncDto();
                data.DeviceId = (long)device.DeviceId;
                data.DeviceName = device.Name;
                data.LastConfigurationAt = device.LastConfigUpdateAt ?? device.AddedAt;
                data.DeviceSerialNumber = device.DeviceSerialNumber;
                data.DeviceModel = device.DeviceModel;
                data.DeviceBrand = device.DeviceBrand;
                data.DeviceDateTimeFormat = device.DeviceDateTimeFormat;
                data.DeviceFirmwareVersion = device.DeviceFirmwareVersion;
                data.DeviceIP = $"{device.DeviceHost}:{device.DevicePort}";
                data.DeviceUser = device.User;
                data.DevicePass = device.Password;

#if DEBUG       
                //INICIO MOCK
                data.Movements = new List<MovementDto>();
                for (int i = 0; i < 200000; i++)
                {
                    ulong docketNumber = (ulong)random.Next(0, 10);
                    if (!statusPersonas.ContainsKey(docketNumber))
                        statusPersonas.Add(docketNumber, true);
                    else
                        statusPersonas[docketNumber] = !statusPersonas[docketNumber];

                    data.Movements.Add(new MovementDto()
                    {
                        DateTime = DateTime.Now.AddSeconds(-random.Next(1, 50)),
                        DocketNumber = (long)docketNumber,
                        MovementType = statusPersonas[docketNumber] ? "I" : "O"
                    });

                    cancellationToken.ThrowIfCancellationRequested();
                }
                //FIN MOCK

#else           

                //OBTIENE LOS DATOS PEGANDOLE AL RELOJ
                try
                {
                    data.Movements = await device.ConnectionWrapper.GetMovementsAsync(onlyNew: !resyncAllRegisters, cancellationToken);
                }
                catch (Exception ex)
                {
                    string e = ex.ToString();

                    if (resyncAllRegisters) //SI ES RESINCRONIZACIÓN COMPLETA, SE DEBE ELEVAR EL ERROR YA QUE EN ESTOS CASOS NO HAY PROXIMO LLAMADO PARA REINTENTAR
                        throw ex;

                    continue; //SI FALLA, SE DEJA REINTENTO PARA PROXIMO LLAMADO
                }

#endif

                var strData = JsonConvert.SerializeObject(data);
                var dataBytes = Encoding.UTF8.GetBytes(strData);
                strData = Convert.ToBase64String(dataBytes);
                strData = EncryptService.Encrypt(strData, cStoragePassword);
                File.WriteAllText($"{cStorageFolderName}\\{Guid.NewGuid():N}", strData);

                SetLastDeviceSynchronization(device.RelojId, DateTime.Now);
            }

            _synchronizingDevices = false;
        }

        public static async Task SyncStoredDataToServerAsync(List<DeviceConfig> devices, CancellationToken cancellationToken)
        {
            _synchronizingServer = true;
            var configData = JsonConvert.SerializeObject(new {
                stsm = ConfigService.Config.SyncToServerMinutes,
                sfdm = ConfigService.Config.SyncFromDevicesMinutes
            });

            Uri baseUri = new Uri(ConfigService.Config.ServiceURL);
            Uri uploadUri = new Uri(baseUri, "Sync/Upload?rules=" + HttpUtility.UrlEncode(Uri.EscapeUriString(configData)));

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
                {
                    File.Delete($"{cStorageFolderName}\\{file}");

                    //Si el servidor envia nueva configuración, la impacta en Config
                    if (result.Config != null)
                    {
                        bool update = false;
                        if (result.Config.newSyncFromDevicesMinutes != null)
                        {
                            ConfigService.Config.SyncFromDevicesMinutes = result.Config.newSyncFromDevicesMinutes;
                            update = true;
                        }
                        if (result.Config.newSyncToServerMinutes != null)
                        {
                            ConfigService.Config.SyncToServerMinutes = result.Config.newSyncToServerMinutes;
                            update = true;
                        }
                        if (update)
                            ConfigService.SaveConfig(ConfigService.Config);
                    }
                }
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
