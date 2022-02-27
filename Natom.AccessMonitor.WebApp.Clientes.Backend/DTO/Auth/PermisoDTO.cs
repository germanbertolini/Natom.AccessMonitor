﻿using Natom.AccessMonitor.Services.Auth.Entities.Models;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Auth
{
    public class PermisoDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        public PermisoDTO From(Permiso entity)
        {
            EncryptedId = EncryptionService.Encrypt<Permiso>(entity.PermisoId);
            Descripcion = entity.Descripcion;

            return this;
        }
    }
}
