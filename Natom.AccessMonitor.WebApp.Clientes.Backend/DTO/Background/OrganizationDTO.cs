using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Background
{
    public class OrganizationDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("registered_at")]
        public DateTime RegisteredAt { get; set; }

        [JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        [JsonProperty("business_name")]
        public string BusinessName { get; set; }

        [JsonProperty("country_icon")]
        public string CountryIcon { get; set; }

        public OrganizationDTO From(Cliente cliente)
        {
            EncryptedId = EncryptionService.Encrypt(cliente.ClienteId);
            RegisteredAt = cliente.RegisterAt;
            BusinessName = cliente.RazonSocial;
            CountryIcon = "arg";
            PictureUrl = "/assets/img/buildings.png";
            return this;
        }
    }
}
