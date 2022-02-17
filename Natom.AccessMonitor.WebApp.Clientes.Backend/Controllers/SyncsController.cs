using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.AccessMonitor.Services.Auth.Attributes;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.DataTable;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Syncs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SyncsController : BaseController
    {
        public SyncsController(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // POST: syncs/devices/list
        [HttpPost]
        [ActionName("devices/list")]
        [TienePermiso(Permiso = "admin_sync")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request)
        {
            try
            {
                if ((_accessToken.ClientId ?? 0) == 0)
                    throw new HandledException("El administrador de Natom solamente puede visualizar y administrar los dispositivos del cliente desde la aplicación de -Admin-");

                var repository = new SyncsManager(_serviceProvider);
                var devices = await repository.ListDevicesByClienteAsync(request.Search.Value, request.Start, request.Length, _accessToken.ClientId ?? -1);

                return Ok(new ApiResultDTO<DataTableResponseDTO<DeviceDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<DeviceDTO>
                    {
                        RecordsTotal = devices.FirstOrDefault()?.TotalRegistros ?? 0,
                        RecordsFiltered = devices.FirstOrDefault()?.TotalFiltrados ?? 0,
                        Records = devices.Select(device => new DeviceDTO().From(device)).ToList()
                    }
                });
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
