using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Places
{
    public class PlaceListDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }


        public PlaceListDTO From(Place entity)
        {
            EncryptedId = EncryptionService.Encrypt<Place>(entity.PlaceId);
            Name = entity.Name;

            return this;
        }
    }
}
