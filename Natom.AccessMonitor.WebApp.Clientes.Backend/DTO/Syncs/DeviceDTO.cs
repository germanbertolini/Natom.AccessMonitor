using Natom.AccessMonitor.Core.Biz.Entities.Results;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Syncs
{
    public class DeviceDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("encrypted_instance_id")]
        public string EncryptedInstanceId { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("device_id")]
        public string DeviceID { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }

        [JsonProperty("goal")]
        public string Goal { get; set; }

        [JsonProperty("sync_name")]
        public string SyncName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_is_online")]
        public bool StatusIsOnline { get; set; }

        public DeviceDTO From(spDeviceListByClientIdResult entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.Id);
            EncryptedInstanceId = EncryptionService.Encrypt(entity.InstanceId);
            Nombre = entity.DeviceName;
            DeviceID = entity.DeviceId;
            Place = entity.Place;
            Goal = entity.Goal;
            SyncName = entity.SyncName;
            Status = entity.IsOnline ? "Conectado" : "Desconectado";
            StatusIsOnline = entity.IsOnline;

            return this;
        }
    }
}
