using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.Extensions.Auth.Attributes;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Autocomplete;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.DataTable;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Dockets;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Titles;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Places;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DocketsController : BaseController
    {
        public DocketsController(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // POST: dockets/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_dockets")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new LegajosManager(_serviceProvider);
                var titlesCount = await manager.ObtenerCountAsync(_accessToken.ClientId ?? -1);
                var titles = await manager.ObtenerDataTableAsync(_accessToken.ClientId ?? -1, request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<DocketListDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<DocketListDTO>
                    {
                        RecordsTotal = titlesCount,
                        RecordsFiltered = titles.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = titles.Select(title => new DocketListDTO().From(title)).ToList()
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

        // GET: dockets/basics/data
        // GET: dockets/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_dockets")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new LegajosManager(_serviceProvider);
                DocketDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var legajoId = EncryptionService.Decrypt<int, Docket>(Uri.UnescapeDataString(encryptedId));
                    var legajo = await manager.ObtenerAsync(legajoId);
                    entity = new DocketDTO().From(legajo);
                }

                var cargosManager = new CargosManager(_serviceProvider);
                var cargos = await cargosManager.ObtenerActivasAsync(_accessToken.ClientId ?? -1);

                var placesManager = new PlacesManager(_serviceProvider);
                var places = await placesManager.ObtenerActivasAsync(_accessToken.ClientId ?? -1);

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        cargos = cargos.Select(c => new TitleDTO().From(c)).ToList(),
                        places = places.Select(c => new PlaceDTO().From(c)).ToList()
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

        // POST: dockets/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_dockets")]
        public async Task<IActionResult> PostSaveAsync([FromBody] DocketDTO legajoDto)
        {
            try
            {
                var manager = new LegajosManager(_serviceProvider);
                var legajo = await manager.GuardarAsync(legajoDto.ToModel(_accessToken.ClientId ?? -1));

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, legajo.DocketId, nameof(Docket), string.IsNullOrEmpty(legajoDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<DocketDTO>
                {
                    Success = true,
                    Data = new DocketDTO().From(legajo)
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

        // DELETE: dockets/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        [TienePermiso(Permiso = "abm_dockets")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var legajoId = EncryptionService.Decrypt<int, Docket>(Uri.UnescapeDataString(encryptedId));

                var manager = new LegajosManager(_serviceProvider);
                await manager.DesactivarAsync(legajoId);

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, legajoId, nameof(Docket), "Baja");

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

        // GET: dockets/search?filter={filter}
        [HttpGet]
        [ActionName("search")]
        public async Task<IActionResult> GetSearchAsync([FromQuery] string filter = null)
        {
            try
            {
                var manager = new LegajosManager(_serviceProvider);
                var legajos = await manager.BuscarAsync(size: 20, filter);

                return Ok(new ApiResultDTO<AutocompleteResponseDTO<DocketDTO>>
                {
                    Success = true,
                    Data = new AutocompleteResponseDTO<DocketDTO>
                    {
                        Total = legajos.Count,
                        Results = legajos.Select(legajo => new DocketDTO().From(legajo)).ToList()
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

        // POST: dockets/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        [TienePermiso(Permiso = "abm_dockets")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var legajoId = EncryptionService.Decrypt<int, Docket>(Uri.UnescapeDataString(encryptedId));

                var manager = new LegajosManager(_serviceProvider);
                await manager.ActivarAsync(legajoId);

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, legajoId, nameof(Docket), "Alta");

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
