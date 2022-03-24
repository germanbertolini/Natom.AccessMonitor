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
    public class PanoramaPorcentajesDTO
    {
        [JsonProperty("porcentaje_7")]
        public int Porcentaje_7 { get; set; }

        [JsonProperty("porcentaje_15")]
        public int Porcentaje_15 { get; set; }

        [JsonProperty("porcentaje_30")]
        public int Porcentaje_30 { get; set; }

        [JsonProperty("places")]
        public List<PlaceListDTO> Places { get; set; }

        public PanoramaPorcentajesDTO From(List<spPanoramaPorcentajesResult> model, List<Place> places)
        {
            var cantidadTotal_7 = model.FirstOrDefault(m => m.Modalidad == 7)?.CantidadTotal ?? 0;
            var cantidadPresentes_7 = model.FirstOrDefault(m => m.Modalidad == 7)?.CantidadPresentes ?? 0;
            var cantidadTotal_15 = model.FirstOrDefault(m => m.Modalidad == 15)?.CantidadTotal ?? 0;
            var cantidadPresentes_15 = model.FirstOrDefault(m => m.Modalidad == 15)?.CantidadPresentes ?? 0;
            var cantidadTotal_30 = model.FirstOrDefault(m => m.Modalidad == 30)?.CantidadTotal ?? 0;
            var cantidadPresentes_30 = model.FirstOrDefault(m => m.Modalidad == 30)?.CantidadPresentes ?? 0;

            this.Porcentaje_7 = 0;
            if (cantidadTotal_7 > 0)
                this.Porcentaje_7 = (Convert.ToInt32((decimal)cantidadPresentes_7 * 100 / (decimal)cantidadTotal_7));

            this.Porcentaje_15 = 0;
            if (cantidadTotal_15 > 0)
                this.Porcentaje_15 = (Convert.ToInt32((decimal)cantidadPresentes_15 * 100 / (decimal)cantidadTotal_15));

            this.Porcentaje_30 = 0;
            if (cantidadTotal_30 > 0)
                this.Porcentaje_30 = (Convert.ToInt32((decimal)cantidadPresentes_30 * 100 / (decimal)cantidadTotal_30));


            this.Places = places.Select(p => new PlaceListDTO().From(p)).OrderBy(p => p.Name).ToList();


            return this;
        }
    }
}
