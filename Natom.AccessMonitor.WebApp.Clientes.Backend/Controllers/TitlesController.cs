using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.AccessMonitor.Services.Auth.Attributes;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.DataTable;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Titles;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TitlesController : BaseController
    {
        public TitlesController(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // POST: titles/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_titles")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new CargosManager(_serviceProvider);
                var titlesCount = await manager.ObtenerCountAsync(_accessToken.ClientId ?? -1);
                var titles = await manager.ObtenerDataTableAsync(_accessToken.ClientId ?? -1, request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<TitleDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<TitleDTO>
                    {
                        RecordsTotal = titlesCount,
                        RecordsFiltered = titles.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = titles.Select(title => new TitleDTO().From(title)).ToList()
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

        // GET: titles/basics/data
        // GET: titles/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_titles")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new CargosManager(_serviceProvider);
                TitleDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var cargo = await manager.ObtenerAsync(clienteId);
                    entity = new TitleDTO().From(cargo);
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

        // POST: titles/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_titles")]
        public async Task<IActionResult> PostSaveAsync([FromBody] TitleDTO titleDto)
        {
            try
            {
                var manager = new CargosManager(_serviceProvider);
                var title = await manager.GuardarAsync(titleDto.ToModel(_accessToken.ClientId ?? -1));

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, title.TitleId, nameof(Title), string.IsNullOrEmpty(titleDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<TitleDTO>
                {
                    Success = true,
                    Data = new TitleDTO().From(title)
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

        // DELETE: titles/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        [TienePermiso(Permiso = "abm_titles")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var titleId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new CargosManager(_serviceProvider);
                await manager.DesactivarAsync(titleId);

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, titleId, nameof(Title), "Baja");

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

        // POST: titles/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        [TienePermiso(Permiso = "abm_titles")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var titleId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new CargosManager(_serviceProvider);
                await manager.ActivarAsync(titleId);

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, titleId, nameof(Title), "Alta");

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
