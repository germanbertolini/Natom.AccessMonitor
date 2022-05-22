using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Results
{
	public class spReporteMensualAsistenciaResult
	{
		public string Empleado { get; set; }
		public string Legajo { get; set; }
		public string Cargo { get; set; }
		public string FechaJornada { get; set; }
		public string DiaSemana { get; set; }
		public string Turno1Entrada { get; set; }
		public string Turno1Salida { get; set; }
		public string Turno2Entrada { get; set; }
		public string Turno2Salida { get; set; }
		public decimal? LLegadaTardeHoras { get; set; }
		public decimal? SalidaTempranaHoras { get; set; }
		public decimal TiempoExtraHoras { get; set; }
		public decimal AusenteHoras { get; set; }
		public decimal TrabajadasHoras { get; set; }

		public decimal DiaTrabajado { get; set; }
		public decimal DiaAusente { get; set; }
		public int VecesLLegadasTarde { get; set; }
		public int VecesSalidasTemprano { get; set; }
	}
}
