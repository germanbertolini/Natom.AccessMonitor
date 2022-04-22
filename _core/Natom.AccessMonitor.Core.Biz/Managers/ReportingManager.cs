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
        public ReportingManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

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
    }
}
