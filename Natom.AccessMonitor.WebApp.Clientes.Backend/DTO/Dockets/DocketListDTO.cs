using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Dockets
{
    public class DocketListDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("docket_number")]
        public string DocketNumber { get; set; }

        [JsonProperty("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [JsonProperty("employee_last_name")]
        public string EmployeeLastName { get; set; }

        [JsonProperty("employee_dni")]
        public string EmployeeDNI { get; set; }

        [JsonProperty("employee_title")]
        public string EmployeeTitle { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        public DocketListDTO From(Docket entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.DocketId);
            DocketNumber = entity.DocketNumber;
            EmployeeFirstName = entity.FirstName;
            EmployeeLastName = entity.LastName;
            EmployeeDNI = entity.DNI;
            EmployeeTitle = entity.Title.Name;
            Activo = entity.Active;

            return this;
        }
    }
}
