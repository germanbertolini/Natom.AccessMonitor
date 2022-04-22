using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Results
{
    public class spReporteEstadisticaAsistenciaResult
    {
		public string Empleado { get; set; }
		public string Legajo { get; set; }
		public string Cargo { get; set; }
		public int DiasLaborales { get; set; }
		public int DiasTrabajados { get; set; }
		public int LLegadasTardeMinutos { get; set; }
		public int SalidasTempranoMinutos { get; set; }
		public int DiasAusente { get; set; }
		public decimal TiempoExtraHoras { get; set; }

	}
}
