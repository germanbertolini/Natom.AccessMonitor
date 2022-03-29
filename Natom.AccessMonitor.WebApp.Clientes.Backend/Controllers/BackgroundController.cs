using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.Extensions.Auth.Services;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Background;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BackgroundController : BaseController
    {
        private readonly AuthService _authService;

        public BackgroundController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
        }

        
        // GET: background/resume
        [HttpGet]
        [ActionName("resume")]
        public async Task<IActionResult> GetResumeAsync()
        {
            try
            {
                int clienteId = _accessToken.ClientId ?? -1;

                var syncsManager = new SyncsManager(_serviceProvider);
                var unassignedDevices = await syncsManager.GetUnassignedDevicesByClientIdAsync(clienteId);
                var syncsTimes = await syncsManager.GetSynchronizerTimesByClienteAsync(clienteId);

                var placesManager = new PlacesManager(_serviceProvider);
                var placesWithoutHours = await placesManager.GetPlacesWithoutHoursAsync(clienteId);

                var clientesManager = new ClientesManager(_serviceProvider);
                var cliente = await clientesManager.ObtenerClienteAsync(clienteId);
                if (cliente == null)
                {
                    cliente = new Core.Biz.Entities.Models.Cliente
                    {
                        RazonSocial = "Natom",
                        NombreFantasia = "Natom",
                        RegisterAt = DateTime.Now
                    };
                }

                return Ok(new ApiResultDTO<ResumeDTO>
                {
                    Success = true,
                    Data = new ResumeDTO
                    {
                        CurrentYear = DateTime.Now.Year,
                        UnassignedDevices = unassignedDevices.Select(d => $"({d.DeviceId}) {d.DeviceName}").ToList(),
                        PlacesWithoutHours = placesWithoutHours.Select(p => p.Name).ToList(),
                        Organization = new OrganizationDTO().From(cliente),
                        SyncsTimes = syncsTimes.Select(s => new SyncTimeDTO().From(s)).ToList()
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
