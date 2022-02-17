using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.AccessMonitor.Services.Auth.Attributes;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.DataTable;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Places;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PlacesController : BaseController
    {
        public PlacesController(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // POST: places/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new PlacesManager(_serviceProvider);
                var placesCount = await manager.ObtenerCountAsync(_accessToken.ClientId ?? -1);
                var places = await manager.ObtenerDataTableAsync(_accessToken.ClientId ?? -1, request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<PlaceDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<PlaceDTO>
                    {
                        RecordsTotal = placesCount,
                        RecordsFiltered = places.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = places.Select(place => new PlaceDTO().From(place)).ToList()
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

        // GET: places/basics/data
        // GET: places/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new PlacesManager(_serviceProvider);
                PlaceDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var cargo = await manager.ObtenerAsync(clienteId);
                    entity = new PlaceDTO().From(cargo);
                }

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity
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

        // POST: places/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> PostSaveAsync([FromBody] PlaceDTO placeDto)
        {
            try
            {
                var manager = new PlacesManager(_serviceProvider);
                var place = await manager.GuardarAsync(placeDto.ToModel(_accessToken.ClientId ?? -1));

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, place.PlaceId, nameof(Place), string.IsNullOrEmpty(placeDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<PlaceDTO>
                {
                    Success = true,
                    Data = new PlaceDTO().From(place)
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

        // DELETE: places/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var placeId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new PlacesManager(_serviceProvider);
                await manager.DesactivarAsync(placeId);

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, placeId, nameof(Place), "Baja");

                return Ok(new ApiResultDTO
                {
                    Success = true
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

        // POST: places/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var placeId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new PlacesManager(_serviceProvider);
                await manager.ActivarAsync(placeId);

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, placeId, nameof(Place), "Alta");

                return Ok(new ApiResultDTO
                {
                    Success = true
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
