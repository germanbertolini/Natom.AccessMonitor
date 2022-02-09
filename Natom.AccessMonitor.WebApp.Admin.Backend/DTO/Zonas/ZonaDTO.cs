using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Admin.Backend.DTO.Zonas
{
    public class ZonaDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        public ZonaDTO From(Zona entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.ZonaId);
            Descripcion = entity.Descripcion;
            Activo = entity.Activo;

            return this;
        }

        public Zona ToModel()
        {
            return new Zona
            {
                ZonaId = EncryptionService.Decrypt<int>(EncryptedId),
                Descripcion = Descripcion,
                Activo = Activo
            };
        }
    }
}
