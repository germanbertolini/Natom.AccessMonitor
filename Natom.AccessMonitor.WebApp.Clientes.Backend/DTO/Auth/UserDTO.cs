using Natom.AccessMonitor.Services.Auth.Entities.Models;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Auth
{
    public class UserDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("picture_url")]
        public string PictureURL { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("registered_at")]
        public DateTime RegisteredAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("business_name")]
        public string BusinessName { get; set; }

        [JsonProperty("business_role_name")]
        public string BusinessRoleName { get; set; }

        [JsonProperty("country_icon")]
        public string CountryIcon { get; set; }

        [JsonProperty("permisos")]
        public List<string> Permisos { get; set; }

        public UserDTO From(Usuario entity, string status = null)
        {
            EncryptedId = EncryptionService.Encrypt(entity.UsuarioId);
            FirstName = entity.Nombre;
            LastName = entity.Apellido;
            Email = entity.Email;
            PictureURL = "/assets/img/user.png";
            RegisteredAt = entity.FechaHoraAlta;
            Status = status;
            Permisos = entity.Permisos?.Select(permiso => EncryptionService.Encrypt(permiso.PermisoId)).ToList();
            BusinessName = entity.ClienteNombre;
            BusinessRoleName = entity.ClienteId == null
                                        ? "Administrador Natom"
                                        : Permisos.Any(p => p.Equals("*")) ? "Administrador" : "Administrativo";
            CountryIcon = "arg";

            return this;
        }
    }
}