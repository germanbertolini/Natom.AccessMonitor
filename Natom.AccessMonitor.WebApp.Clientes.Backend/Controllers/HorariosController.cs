using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.AccessMonitor.Services.Auth.Attributes;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.DataTable;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Horarios;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HorariosController : BaseController
    {
        public HorariosController(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // POST: horarios/list?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_places_horarios")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string encryptedId)
        {
            try
            {
                if ((_accessToken.ClientId ?? 0) == 0)
                    throw new HandledException("El administrador de Natom solamente puede visualizar y administrar los horarios y tolerancias del cliente desde la aplicación de -Admin-");

                var placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedId));

                var manager = new HorariosManager(_serviceProvider);
                var horariosCount = await manager.ObtenerCountAsync(placeId);
                var horarios = await manager.ObtenerDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, placeId);

                return Ok(new ApiResultDTO<DataTableResponseDTO<HorarioDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<HorarioDTO>
                    {
                        RecordsTotal = horariosCount,
                        RecordsFiltered = horarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = horarios.Select(goal => new HorarioDTO().From(goal)).ToList()
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

        // GET: horarios/basics/data
        // GET: horarios/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_places_horarios")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedPlaceId, [FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new HorariosManager(_serviceProvider);
                HorarioDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var horarioId = EncryptionService.Decrypt<int, ConfigTolerancia>(Uri.UnescapeDataString(encryptedId));
                    var horario = await manager.ObtenerAsync(horarioId);
                    entity = new HorarioDTO().From(horario);
                }
                else
                {
                    var placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedPlaceId));
                    var horario = await manager.ObtenerVigenteAsync(_accessToken.ClientId ?? -1, placeId);

                    //VALORES POR DEFAULT PRIMER CONFIGURACIÓN
                    if (horario == null)
                    {
                        horario = new ConfigTolerancia()
                        {
                            IngresoToleranciaMins = 10,
                            EgresoToleranciaMins = 10,
                            AlmuerzoHorarioDesde = new TimeSpan(12, 0, 0),
                            AlmuerzoHorarioHasta = new TimeSpan(15, 0, 0),
                            AlmuerzoTiempoLimiteMins = 65,
                            AplicaDesde = DateTime.Now.Date
                        };
                    }
                    horario.ConfigToleranciaId = 0;
                    entity = new HorarioDTO().From(horario);
                }

                return Ok(new ApiResultDTO<HorarioDTO>
                {
                    Success = true,
                    Data = entity
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

        // POST: horarios/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_places_horarios")]
        public async Task<IActionResult> PostSaveAsync([FromBody] HorarioDTO horarioDto)
        {
            try
            {
                if ((_accessToken.ClientId ?? 0) == 0)
                    throw new HandledException("El administrador de Natom solamente puede visualizar y administrar los horarios y tolerancias del cliente desde la aplicación de -Admin-");

                var manager = new HorariosManager(_serviceProvider);
                var horario = await manager.GuardarAsync(_accessToken.ClientId.Value, _accessToken.UserId.Value, horarioDto.ToModel(_accessToken.ClientId.Value, horarioDto));

                return Ok(new ApiResultDTO<HorarioDTO>
                {
                    Success = true,
                    Data = new HorarioDTO().From(horario)
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

        // GET: horarios/panorama/actual?encryptedPlaceId={encryptedPlaceId}
        [HttpGet]
        [ActionName("panorama/actual")]
        public async Task<IActionResult> GetPanoramaActualAsync([FromQuery] string encryptedPlaceId = null)
        {
            try
            {
                int? placeId = null;
                if (!string.IsNullOrEmpty(encryptedPlaceId))
                    placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedPlaceId));

                var manager = new HorariosManager(_serviceProvider);
                var panorama = manager.GetPanoramaActual(_accessToken.ClientId ?? -1, placeId);

                return Ok(new ApiResultDTO<PanoramaActualDTO>
                {
                    Success = true,
                    Data = new PanoramaActualDTO().From(panorama)
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

        // GET: horarios/panorama/porcentajes?encryptedPlaceId={encryptedPlaceId}
        [HttpGet]
        [ActionName("panorama/porcentajes")]
        public async Task<IActionResult> GetPanoramaPorcentajesAsync([FromQuery] string encryptedPlaceId = null)
        {
            try
            {
                int? placeId = null;
                if (!string.IsNullOrEmpty(encryptedPlaceId))
                    placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedPlaceId));

                var manager = new HorariosManager(_serviceProvider);
                var panorama = manager.GetPanoramaPorcentajes(_accessToken.ClientId ?? -1, placeId);

                var placesManager = new PlacesManager(_serviceProvider);
                var places = await placesManager.ObtenerActivasAsync(_accessToken.ClientId ?? -1);

                return Ok(new ApiResultDTO<PanoramaPorcentajesDTO>
                {
                    Success = true,
                    Data = new PanoramaPorcentajesDTO().From(panorama, places)
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
