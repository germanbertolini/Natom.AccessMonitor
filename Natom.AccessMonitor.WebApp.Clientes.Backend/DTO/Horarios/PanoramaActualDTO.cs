using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Core.Biz.Entities.Results;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Places;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Horarios
{
    public class PanoramaActualDTO
    {
        [JsonProperty("dia_nombre")]
        public string DiaNombre { get; set; }

        [JsonProperty("cantidad_total")]
        public int CantidadTotal { get; set; }

        [JsonProperty("cantidad_presentes")]
        public int CantidadPresentes { get; set; }

        [JsonProperty("cantidad_ausentes")]
        public int CantidadAusentes { get; set; }

        [JsonProperty("porcentaje_asistencia")]
        public int PorcentajeAsistencia { get; set; }

        public PanoramaActualDTO From(spPanoramaActualResult model)
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:      this.DiaNombre = "Lunes";       break;
                case DayOfWeek.Tuesday:     this.DiaNombre = "Martes";      break;
                case DayOfWeek.Wednesday:   this.DiaNombre = "Miercoles";   break;
                case DayOfWeek.Thursday:    this.DiaNombre = "Jueves";      break;
                case DayOfWeek.Friday:      this.DiaNombre = "Viernes";     break;
                case DayOfWeek.Saturday:    this.DiaNombre = "Sabado";      break;
                case DayOfWeek.Sunday:      this.DiaNombre = "Domingo";     break;
            }
            this.DiaNombre += " " + DateTime.Now.ToString("dd/MM");
            this.CantidadTotal = model.CantidadTotal;
            this.CantidadAusentes = model.CantidadAusentes;
            this.CantidadPresentes = model.CantidadPresentes;

            return this;
        }
    }
}
