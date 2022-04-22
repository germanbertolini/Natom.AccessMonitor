using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO;
using Natom.Extensions.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportesController : BaseController
    {
        public ReportesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // GET: reportes/listados/reporte-mensual-horas-trabajadas?desde={desde}&hasta={hasta}
        [HttpGet]
        [ActionName("listados/reporte-mensual-horas-trabajadas")]
        public async Task<IActionResult> GetReporteMensualHorasTrabajadasAsync([FromQuery] string desde, [FromQuery] string hasta)
        {
            try
            {
                DateTime _desde = DateTime.ParseExact(desde, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime _hasta = DateTime.ParseExact(hasta, "d/M/yyyy", CultureInfo.InvariantCulture);

                if (_hasta < _desde)
                    throw new HandledException("Fecha 'Desde' no puede ser mayor a fecha 'Hasta'");

                if ((_hasta - _desde).TotalDays > 35)
                    throw new HandledException("El rango de dias a consultar no puede ser mayor a 35 dias");

                var manager = new ReportingManager(_serviceProvider);
                var data = manager.GetDatosReporteMensualHorasTrabajadas(_accessToken.ClientId ?? -1, _desde, _hasta);

                string mimtype = "";
                int extension = 1;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                var path = Path.Combine(_hostingEnvironment.ContentRootPath, "Reporting", "ReporteMensualHorasTrabajadasReport.rdlc");
                var report = new LocalReport(path);
                report.AddDataSource("DataSet1", data);

                parameters.Add("Desde", _desde.ToString("dd/MM/yyyy"));
                parameters.Add("Hasta", _hasta.ToString("dd/MM/yyyy"));

                var result = report.Execute(RenderType.Pdf, extension, parameters, mimtype);
                return File(result.MainStream, "application/pdf");
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: reportes/listados/estadistica-asistencia?desde={desde}&hasta={hasta}
        [HttpGet]
        [ActionName("listados/estadistica-asistencia")]
        public async Task<IActionResult> GetEstadisticaAsistenciaAsync([FromQuery] string desde, [FromQuery] string hasta)
        {
            try
            {
                DateTime _desde = DateTime.ParseExact(desde, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime _hasta = DateTime.ParseExact(hasta, "d/M/yyyy", CultureInfo.InvariantCulture);

                if (_hasta < _desde)
                    throw new HandledException("Fecha 'Desde' no puede ser mayor a fecha 'Hasta'");

                var manager = new ReportingManager(_serviceProvider);
                var data = manager.GetDatosEstadisticaAsistencia(_accessToken.ClientId ?? -1, _desde, _hasta);

                string mimtype = "";
                int extension = 1;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                var path = Path.Combine(_hostingEnvironment.ContentRootPath, "Reporting", "EstadisticaAsistenciaReport.rdlc");
                var report = new LocalReport(path);
                report.AddDataSource("DataSet1", data);

                parameters.Add("Desde", _desde.ToString("dd/MM/yyyy"));
                parameters.Add("Hasta", _hasta.ToString("dd/MM/yyyy"));

                var result = report.Execute(RenderType.Pdf, extension, parameters, mimtype);
                return File(result.MainStream, "application/pdf");
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }
    }
}
