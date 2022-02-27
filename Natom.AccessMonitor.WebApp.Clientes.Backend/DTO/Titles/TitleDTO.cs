using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Services.Auth.Entities.Models;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Titles
{
    public class TitleDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        public TitleDTO From(Title entity, string status = null)
        {
            EncryptedId = EncryptionService.Encrypt<Title>(entity.TitleId);
            Name = entity.Name;
            Activo = !entity.RemovedAt.HasValue;

            return this;
        }

        public Title ToModel(int clienteId)
        {
            return new Title
            {
                TitleId = EncryptionService.Decrypt<int, Title>(EncryptedId),
                Name = Name,
                ClienteId = clienteId,
                RemovedAt = null
            };
        }
    }
}
