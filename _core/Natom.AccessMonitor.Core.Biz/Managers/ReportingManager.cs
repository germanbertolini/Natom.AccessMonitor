using Microsoft.EntityFrameworkCore;
using Natom.AccessMonitor.Core.Biz.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Managers
{
    public class ReportingManager : BaseManager
    {
        private int _minutosMinimoParaSerHoraExtra { get; set; }

        public ReportingManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _minutosMinimoParaSerHoraExtra = Convert.ToInt32(_configuration.GetValueAsync("Reporting.Tolerancias.MinutosMinimoParaSerHoraExtra").GetAwaiter().GetResult());
        }

        public List<spMovementsProcessedSelectByClientAndRangeDateResult> GetMovementsProcessed(int clienteId, DateTime fromDate, DateTime toDate, int? docketId = null)
                            => _db.spMovementsProcessedSelectByClientAndRangeDateResult
                                    .FromSqlRaw("sp_movements_processed_select_by_client_and_range_date {0}, {1}, {2}, {3}", clienteId, fromDate, toDate, docketId)
                                    .AsEnumerable()
                                    .ToList();

        public List<spReporteMensualHorasTrabajadasResult> GetDatosReporteMensualHorasTrabajadas(int clienteId, DateTime fromDate, DateTime toDate, int? docketId = null)
        {
            var result = new List<spReporteMensualHorasTrabajadasResult>();
            var data = GetMovementsProcessed(clienteId, fromDate, toDate, docketId);
            var groupByDocket = data.GroupBy(jornal => new { Name = $"{jornal.LastName}, {jornal.FirstName}", jornal.DocketNumber, jornal.Title },
                                            (k, v) => new
                                            {
                                                k.Name,
                                                k.DocketNumber,
                                                k.Title,
                                                groupByDate = v.GroupBy(jornal => jornal.Date, (k, v) => new
                                                {
                                                    Date = k,
                                                    WorkedHours = (decimal)v.Where(j => j.PermanenceTime.HasValue).Sum(j => j.PermanenceTime.Value.TotalMinutes) / 60
                                                })                                                
                                            }).OrderBy(d => d.Title).ThenBy(d => d.Name);


            //SI NO HAY DATOS, EL PRIMER REGISTRO LO MANDAMOS VACIO PERO CON LAS FECHAS DEFINIDAS
            Dictionary<DateTime, int> indexPorFechas = null;
            if (groupByDocket.Count() == 0)
            {
                var primerElemento = new spReporteMensualHorasTrabajadasResult();
                indexPorFechas = ReporteMensualHorasTrabajadasDefinirFechas(ref primerElemento, fromDate, toDate);
                result.Add(primerElemento);
            }
            //SI HAY DATOS ENTONCES GENERAMOS LA DATA!
            else
            {
                foreach (var docket in groupByDocket)
                {
                    var obj = new spReporteMensualHorasTrabajadasResult
                    {
                        Nombre = docket.Name,
                        Legajo = docket.DocketNumber,
                        Cargo = docket.Title,
                    };

                    if (result.Count == 0)
                        indexPorFechas = ReporteMensualHorasTrabajadasDefinirFechas(ref obj, fromDate, toDate);

                    foreach (var date in docket.groupByDate)
                    {
                        if (date.WorkedHours > 0)
                            SetHorasTrabajadas(ref obj, indexPorFechas[date.Date], date.WorkedHours);
                    }


                    obj.HorasTrabajadasTotal1 = (obj.HorasTrabajadas1 ?? 0) + (obj.HorasTrabajadas2 ?? 0) + (obj.HorasTrabajadas3 ?? 0) + (obj.HorasTrabajadas4 ?? 0) + (obj.HorasTrabajadas5 ?? 0) + (obj.HorasTrabajadas6 ?? 0) + (obj.HorasTrabajadas7 ?? 0);
                    obj.HorasTrabajadasTotal2 = (obj.HorasTrabajadas8 ?? 0) + (obj.HorasTrabajadas9 ?? 0) + (obj.HorasTrabajadas10 ?? 0) + (obj.HorasTrabajadas11 ?? 0) + (obj.HorasTrabajadas12 ?? 0) + (obj.HorasTrabajadas13 ?? 0) + (obj.HorasTrabajadas14 ?? 0);
                    obj.HorasTrabajadasTotal3 = (obj.HorasTrabajadas15 ?? 0) + (obj.HorasTrabajadas16 ?? 0) + (obj.HorasTrabajadas17 ?? 0) + (obj.HorasTrabajadas18 ?? 0) + (obj.HorasTrabajadas19 ?? 0) + (obj.HorasTrabajadas20 ?? 0) + (obj.HorasTrabajadas21 ?? 0);
                    obj.HorasTrabajadasTotal4 = (obj.HorasTrabajadas22 ?? 0) + (obj.HorasTrabajadas23 ?? 0) + (obj.HorasTrabajadas24 ?? 0) + (obj.HorasTrabajadas25 ?? 0) + (obj.HorasTrabajadas26 ?? 0) + (obj.HorasTrabajadas27 ?? 0) + (obj.HorasTrabajadas28 ?? 0);
                    obj.HorasTrabajadasTotal5 = (obj.HorasTrabajadas29 ?? 0) + (obj.HorasTrabajadas30 ?? 0) + (obj.HorasTrabajadas31 ?? 0) + (obj.HorasTrabajadas32 ?? 0) + (obj.HorasTrabajadas33 ?? 0) + (obj.HorasTrabajadas34 ?? 0) + (obj.HorasTrabajadas35 ?? 0);
                    obj.HorasTrabajadasTotal6 = (obj.HorasTrabajadas36 ?? 0) + (obj.HorasTrabajadas37 ?? 0) + (obj.HorasTrabajadas38 ?? 0) + (obj.HorasTrabajadas39 ?? 0) + (obj.HorasTrabajadas40 ?? 0) + (obj.HorasTrabajadas41 ?? 0) + (obj.HorasTrabajadas42 ?? 0);

                    result.Add(obj);
                }
            }

            return result;
        }

        private Dictionary<DateTime, int> ReporteMensualHorasTrabajadasDefinirFechas(ref spReporteMensualHorasTrabajadasResult obj, DateTime from, DateTime to)
        {
            var indexPorFechas = new Dictionary<DateTime, int>();
            int index = -1;
            int indexSemanal = -1;

            DateTime date = from;
            while (date <= to)
            {
                if (index == -1)
                {
                    index = (int)date.DayOfWeek;
                    if (index == 0) index = 7;
                    indexSemanal = index;
                }
                else
                {
                    index++;
                    indexSemanal++;
                    if (indexSemanal == 8)
                        indexSemanal = 1;
                }

                indexPorFechas.Add(date, index);

                string fecha = date.Day.ToString().PadLeft(2, '0');
                string[] dias = { "LU", "MA", "MI", "JU", "VI", "SA", "DO" };
                SetFecha(ref obj, index, $"{fecha} {dias[indexSemanal - 1]}");

                date = date.AddDays(1);
            }

            return indexPorFechas;
        }

        private void SetFecha(ref spReporteMensualHorasTrabajadasResult obj, int index, string valor)
        {
            switch (index)
            {
                case 1: obj.Fecha1 = valor; break;
                case 2: obj.Fecha2 = valor; break;
                case 3: obj.Fecha3 = valor; break;
                case 4: obj.Fecha4 = valor; break;
                case 5: obj.Fecha5 = valor; break;
                case 6: obj.Fecha6 = valor; break;
                case 7: obj.Fecha7 = valor; break;
                case 8: obj.Fecha8 = valor; break;
                case 9: obj.Fecha9 = valor; break;
                case 10: obj.Fecha10 = valor; break;
                case 11: obj.Fecha11 = valor; break;
                case 12: obj.Fecha12 = valor; break;
                case 13: obj.Fecha13 = valor; break;
                case 14: obj.Fecha14 = valor; break;
                case 15: obj.Fecha15 = valor; break;
                case 16: obj.Fecha16 = valor; break;
                case 17: obj.Fecha17 = valor; break;
                case 18: obj.Fecha18 = valor; break;
                case 19: obj.Fecha19 = valor; break;
                case 20: obj.Fecha20 = valor; break;
                case 21: obj.Fecha21 = valor; break;
                case 22: obj.Fecha22 = valor; break;
                case 23: obj.Fecha23 = valor; break;
                case 24: obj.Fecha24 = valor; break;
                case 25: obj.Fecha25 = valor; break;
                case 26: obj.Fecha26 = valor; break;
                case 27: obj.Fecha27 = valor; break;
                case 28: obj.Fecha28 = valor; break;
                case 29: obj.Fecha29 = valor; break;
                case 30: obj.Fecha30 = valor; break;
                case 31: obj.Fecha31 = valor; break;
                case 32: obj.Fecha32 = valor; break;
                case 33: obj.Fecha33 = valor; break;
                case 34: obj.Fecha34 = valor; break;
                case 35: obj.Fecha35 = valor; break;
                case 36: obj.Fecha36 = valor; break;
                case 37: obj.Fecha37 = valor; break;
                case 38: obj.Fecha38 = valor; break;
                case 39: obj.Fecha39 = valor; break;
                case 40: obj.Fecha40 = valor; break;
                case 41: obj.Fecha41 = valor; break;
                case 42: obj.Fecha42 = valor; break;
            }
        }

        private void SetHorasTrabajadas(ref spReporteMensualHorasTrabajadasResult obj, int index, decimal valor)
        {
            switch (index)
            {
                case 1: obj.HorasTrabajadas1 = valor; break;
                case 2: obj.HorasTrabajadas2 = valor; break;
                case 3: obj.HorasTrabajadas3 = valor; break;
                case 4: obj.HorasTrabajadas4 = valor; break;
                case 5: obj.HorasTrabajadas5 = valor; break;
                case 6: obj.HorasTrabajadas6 = valor; break;
                case 7: obj.HorasTrabajadas7 = valor; break;
                case 8: obj.HorasTrabajadas8 = valor; break;
                case 9: obj.HorasTrabajadas9 = valor; break;
                case 10: obj.HorasTrabajadas10 = valor; break;
                case 11: obj.HorasTrabajadas11 = valor; break;
                case 12: obj.HorasTrabajadas12 = valor; break;
                case 13: obj.HorasTrabajadas13 = valor; break;
                case 14: obj.HorasTrabajadas14 = valor; break;
                case 15: obj.HorasTrabajadas15 = valor; break;
                case 16: obj.HorasTrabajadas16 = valor; break;
                case 17: obj.HorasTrabajadas17 = valor; break;
                case 18: obj.HorasTrabajadas18 = valor; break;
                case 19: obj.HorasTrabajadas19 = valor; break;
                case 20: obj.HorasTrabajadas20 = valor; break;
                case 21: obj.HorasTrabajadas21 = valor; break;
                case 22: obj.HorasTrabajadas22 = valor; break;
                case 23: obj.HorasTrabajadas23 = valor; break;
                case 24: obj.HorasTrabajadas24 = valor; break;
                case 25: obj.HorasTrabajadas25 = valor; break;
                case 26: obj.HorasTrabajadas26 = valor; break;
                case 27: obj.HorasTrabajadas27 = valor; break;
                case 28: obj.HorasTrabajadas28 = valor; break;
                case 29: obj.HorasTrabajadas29 = valor; break;
                case 30: obj.HorasTrabajadas30 = valor; break;
                case 31: obj.HorasTrabajadas31 = valor; break;
                case 32: obj.HorasTrabajadas32 = valor; break;
                case 33: obj.HorasTrabajadas33 = valor; break;
                case 34: obj.HorasTrabajadas34 = valor; break;
                case 35: obj.HorasTrabajadas35 = valor; break;
                case 36: obj.HorasTrabajadas36 = valor; break;
                case 37: obj.HorasTrabajadas37 = valor; break;
                case 38: obj.HorasTrabajadas38 = valor; break;
                case 39: obj.HorasTrabajadas39 = valor; break;
                case 40: obj.HorasTrabajadas40 = valor; break;
                case 41: obj.HorasTrabajadas41 = valor; break;
                case 42: obj.HorasTrabajadas42 = valor; break;
            }
        }

        public List<spReporteEstadisticaAsistenciaResult> GetDatosEstadisticaAsistencia(int clienteId, DateTime fromDate, DateTime toDate, int? docketId = null)
        {
            var result = new List<spReporteEstadisticaAsistenciaResult>();
            var data = GetMovementsProcessed(clienteId, fromDate, toDate, docketId);
            var groupByDocket = data.GroupBy(jornal => new { Name = $"{jornal.LastName}, {jornal.FirstName}", jornal.DocketNumber, jornal.Title },
                                            (k, v) => new
                                            {
                                                k.Name,
                                                k.DocketNumber,
                                                k.Title,
                                                Jornadas = v.GroupBy(j => new { j.Date }, (k2, v2) => new
                                                {
                                                    Ausente = v2.All(d => !d.In.HasValue),
                                                    ExpectedWorkedTimeInMinutes = (int)v2.Sum(d => d.ExpectedPermanenceTime.TotalMinutes),
                                                    WorkedTimeInMinutes = (int)v2.Where(d => d.PermanenceTime.HasValue).Sum(d => d.PermanenceTime.Value.TotalMinutes),
                                                    LLegadaTardeMinutos = (int)v2.Where(d => d.In.HasValue).Sum(d => (d.In.Value - d.ExpectedInDateTime).TotalMinutes),
                                                    SalidasTempranoMinutos = (int)v2.Where(d => d.Out.HasValue && d.OutWasEstimated == false).Sum(d => (d.ExpectedOutDateTime - d.Out.Value).TotalMinutes),
                                                    LLegadasTarde = v2.Count(d => d.In.HasValue && d.In.Value > d.ExpectedInDateTime),
                                                    SalidasTempranas = v2.Count(d => d.Out.HasValue && d.OutWasEstimated == false && d.Out.Value < d.ExpectedOutDateTime)
                                                })
                                            }).OrderBy(d => d.Title).ThenBy(d => d.Name);


            foreach (var docket in groupByDocket)
            {
                var obj = new spReporteEstadisticaAsistenciaResult
                {
                    Empleado = docket.Name,
                    Legajo = docket.DocketNumber,
                    Cargo = docket.Title
                };

                obj.DiasLaborales = docket.Jornadas.Count();
                obj.DiasTrabajados = docket.Jornadas.Where(j => !j.Ausente).Count();
                obj.LLegadasTardeMinutos = docket.Jornadas.Sum(j => j.LLegadaTardeMinutos);
                obj.SalidasTempranoMinutos = docket.Jornadas.Sum(j => j.SalidasTempranoMinutos);
                obj.DiasAusente = docket.Jornadas.Where(j => j.Ausente).Count();
                obj.DiasLLegadasTarde = docket.Jornadas.Sum(j => j.LLegadasTarde);
                obj.DiasSalidasTemprano = docket.Jornadas.Sum(j => j.SalidasTempranas);
                obj.TiempoExtraHoras = Math.Round((decimal)docket.Jornadas.Where(j => j.WorkedTimeInMinutes + _minutosMinimoParaSerHoraExtra > j.ExpectedWorkedTimeInMinutes).Sum(j => j.WorkedTimeInMinutes - j.ExpectedWorkedTimeInMinutes) / 60, 2);

                result.Add(obj);
            }


            return result;
        }

        public List<spReporteMensualAsistenciaResult> GetDatosMensualAsistencia(int clienteId, DateTime fromDate, DateTime toDate, int docketId)
        {
            var result = new List<spReporteMensualAsistenciaResult>();
            var data = GetMovementsProcessed(clienteId, fromDate, toDate, docketId);
            var groupByJournal = data.GroupBy(jornal => new { Name = $"{jornal.LastName}, {jornal.FirstName}", jornal.DocketNumber, jornal.Title, Date = jornal.Date },
                                            (k, v) => new
                                            {
                                                k.Name,
                                                k.DocketNumber,
                                                k.Title,
                                                k.Date,
                                                Turnos = v
                                            }).OrderBy(d => d.Title).ThenBy(d => d.Name);


            var docketData = groupByJournal.FirstOrDefault();
            DateTime date = fromDate;
            while (date <= toDate)
            {
                var jornada = groupByJournal.FirstOrDefault(j => j.Date.Date == date.Date);
                var turno1 = jornada?.Turnos.ElementAtOrDefault(0);
                var turno2 = jornada?.Turnos.ElementAtOrDefault(1);

                var turno1LLegadaTardeMins = 0;
                var turno1SalidaTempranaMins = 0;
                var turno1TiempoExtraMins = 0;
                var turno1TiempoTrabajadoMins = (int?)null;
                var turno1TiempoNoTrabajadoMins = (int?)null;

                var turno2LLegadaTardeMins = 0;
                var turno2SalidaTempranaMins = 0;
                var turno2TiempoExtraMins = 0;
                var turno2TiempoTrabajadoMins = (int?)null;
                var turno2TiempoNoTrabajadoMins = (int?)null;

                //////////////////////////////////////////////
                //LOGICA TURNO 1
                if (turno1 != null && turno1.In.HasValue && turno1.In.Value > turno1.ExpectedInDateTime)
                    turno1LLegadaTardeMins = (int)(turno1.In.Value - turno1.ExpectedInDateTime).TotalMinutes;

                if (turno1 != null && turno1.Out.HasValue && turno1.OutWasEstimated == false && turno1.Out.Value < turno1.ExpectedOutDateTime)
                    turno1SalidaTempranaMins = (int)(turno1.ExpectedOutDateTime - turno1.Out.Value).TotalMinutes;

                if (turno1 != null && turno1.PermanenceTime != null)
                {
                    turno1TiempoExtraMins = (int)(turno1.PermanenceTime.Value - turno1.ExpectedPermanenceTime).TotalMinutes;
                    if (turno1TiempoExtraMins + _minutosMinimoParaSerHoraExtra < 10)
                        turno1TiempoExtraMins = 0;
                    turno1TiempoTrabajadoMins = (int)turno1.PermanenceTime.Value.TotalMinutes;
                }

                if (turno1TiempoTrabajadoMins != null)
                {
                    turno1TiempoNoTrabajadoMins = (int)turno1.ExpectedPermanenceTime.TotalMinutes - turno1TiempoTrabajadoMins;
                    if (turno1TiempoNoTrabajadoMins < _minutosMinimoParaSerHoraExtra)
                        turno1TiempoNoTrabajadoMins = 0;
                }

                //////////////////////////////////////////////
                //LOGICA TURNO 2
                if (turno2 != null && turno2.In.HasValue && turno2.In.Value > turno2.ExpectedInDateTime)
                    turno2LLegadaTardeMins = (int)(turno2.In.Value - turno2.ExpectedInDateTime).TotalMinutes;

                if (turno2 != null && turno2.Out.HasValue && turno2.OutWasEstimated == false && turno2.Out.Value < turno2.ExpectedOutDateTime)
                    turno2SalidaTempranaMins = (int)(turno2.ExpectedOutDateTime - turno2.Out.Value).TotalMinutes;

                if (turno2 != null && turno2.PermanenceTime != null)
                {
                    turno2TiempoExtraMins = (int)(turno2.PermanenceTime.Value - turno2.ExpectedPermanenceTime).TotalMinutes;
                    if (turno2TiempoExtraMins + _minutosMinimoParaSerHoraExtra < 10)
                        turno2TiempoExtraMins = 0;
                    turno2TiempoTrabajadoMins = (int)turno2.PermanenceTime.Value.TotalMinutes;
                }

                if (turno2TiempoTrabajadoMins != null)
                {
                    turno2TiempoNoTrabajadoMins = (int)turno2.ExpectedPermanenceTime.TotalMinutes - turno2TiempoTrabajadoMins;
                    if (turno2TiempoNoTrabajadoMins < _minutosMinimoParaSerHoraExtra)
                        turno2TiempoNoTrabajadoMins = 0;
                }

                //////////////////////////////////////////////
                //ARMAMOS OBJETO CON LOS DATOS
                var obj = new spReporteMensualAsistenciaResult();
                obj.Empleado = docketData?.Name;
                obj.Legajo = docketData?.DocketNumber;
                obj.Cargo = docketData?.Title;
                obj.DiaSemana = GetDiaSemana(date).ToUpper();
                obj.FechaJornada = date.ToString("dd/MM/yyyy");
                obj.Turno1Entrada = turno1?.In?.TimeOfDay == null ? "-" : turno1?.In?.TimeOfDay.ToString(@"hh\:mm");
                obj.Turno1Salida = turno1?.Out?.TimeOfDay == null ? "-" : turno1?.Out?.TimeOfDay.ToString(@"hh\:mm") + ((turno1?.OutWasEstimated ?? false) ? " *" : "");   //SI ES SALIDA ESTIMADA LE PONEMOS UN ASTERISCO
                obj.Turno2Entrada = turno2?.In?.TimeOfDay == null ? "-" : turno2?.In?.TimeOfDay.ToString(@"hh\:mm") ?? "";
                obj.Turno2Salida = turno2?.Out?.TimeOfDay == null ? "-" : turno2?.Out?.TimeOfDay.ToString(@"hh\:mm") + ((turno2?.OutWasEstimated ?? false) ? " *" : "");   //SI ES SALIDA ESTIMADA LE PONEMOS UN ASTERISCO
                obj.LLegadaTardeHoras = Math.Round((decimal)(turno1LLegadaTardeMins + turno2LLegadaTardeMins) / 60, 1);
                obj.SalidaTempranaHoras = Math.Round((decimal)(turno1SalidaTempranaMins + turno2SalidaTempranaMins) / 60, 1);
                obj.TiempoExtraHoras = Math.Round((decimal)(turno1TiempoExtraMins + turno2TiempoExtraMins) / 60, 1);
                obj.AusenteHoras = turno1TiempoNoTrabajadoMins == null && turno2TiempoNoTrabajadoMins == null ? (decimal)0 : Math.Round((decimal)((turno1TiempoNoTrabajadoMins ?? 0) + (turno2TiempoNoTrabajadoMins ?? 0)) / 60, 1);
                obj.TrabajadasHoras = turno1TiempoTrabajadoMins == null && turno2TiempoTrabajadoMins == null ? (decimal)0 : Math.Round((decimal)((turno1TiempoTrabajadoMins ?? 0) + (turno2TiempoTrabajadoMins ?? 0)) / 60, 1);

                //CASO DOBLE TURNO
                if (turno1 != null && turno2 != null)
                    obj.DiaTrabajado = (decimal)(turno1.PermanenceTime.HasValue ? 0.5 : 0) + (decimal)(turno2.PermanenceTime.HasValue ? 0.5 : 0);
                //CASO UNICO TURNO
                else if (turno1 != null)
                    obj.DiaTrabajado = (decimal)(turno1.PermanenceTime.HasValue ? 1 : 0);

                obj.VecesLLegadasTarde = (turno1LLegadaTardeMins > 0 ? 1 : 0) + (turno2LLegadaTardeMins > 0 ? 1 : 0);
                obj.VecesSalidasTemprano = (turno1SalidaTempranaMins > 0 ? 1 : 0) + (turno2SalidaTempranaMins > 0 ? 1 : 0);

                result.Add(obj);

                date = date.AddDays(1);
            }


            return result;
        }

        private string GetDiaSemana(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:      return "Lun";
                case DayOfWeek.Tuesday:     return "Mar";
                case DayOfWeek.Wednesday:   return "Mie";
                case DayOfWeek.Thursday:    return "Jue";
                case DayOfWeek.Friday:      return "Vie";
                case DayOfWeek.Saturday:    return "Sab";
                case DayOfWeek.Sunday:      return "Dom";
                default: return "";
            }
        }
    }
}
