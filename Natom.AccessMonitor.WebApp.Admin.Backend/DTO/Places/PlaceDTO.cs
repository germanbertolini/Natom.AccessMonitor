using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Admin.Backend.DTO.Places
{
    public class PlaceDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        public PlaceDTO From(Place entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.PlaceId);
            Name = entity.Name;
            Address = entity.Address;
            Activo = !entity.RemovedAt.HasValue;

            return this;
        }

        public Place ToModel(int clientId)
        {
            return new Place
            {
                PlaceId = EncryptionService.Decrypt<int>(EncryptedId),
                Name = Name,
                Address = Address,
                Lat = null,
                Lng = null,
                ClientId = clientId
            };
        }
    }
}
