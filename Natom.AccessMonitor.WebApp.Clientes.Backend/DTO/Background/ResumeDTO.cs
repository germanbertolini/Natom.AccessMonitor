﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Background
{
    public class ResumeDTO
    {
        [JsonProperty("current_year")]
        public int CurrentYear { get; set; }

        [JsonProperty("organization")]
        public OrganizationDTO Organization { get; set; }

        [JsonProperty("unassigned_devices")]
        public List<string> UnassignedDevices { get; set; }

        [JsonProperty("places_without_hours")]
        public List<string> PlacesWithoutHours { get; set; }

        [JsonProperty("syncs_times")]
        public List<SyncTimeDTO> SyncsTimes { get; set; }
    }
}
