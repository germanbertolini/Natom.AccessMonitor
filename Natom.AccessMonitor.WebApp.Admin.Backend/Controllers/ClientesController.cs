using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.AccessMonitor.Services.Auth.Attributes;
using Natom.AccessMonitor.Services.Auth.Services;
using Natom.AccessMonitor.WebApp.Admin.Backend.DTO;
using Natom.AccessMonitor.WebApp.Admin.Backend.DTO.Autocomplete;
using Natom.AccessMonitor.WebApp.Admin.Backend.DTO.Clientes;
using Natom.AccessMonitor.WebApp.Admin.Backend.DTO.DataTable;
using Natom.AccessMonitor.WebApp.Admin.Backend.DTO.Synchronizers;
using Natom.AccessMonitor.WebApp.Admin.Backend.DTO.Zonas;
using Natom.AccessMonitor.WebApp.Admin.Backend.Repositories;
using Natom.AccessMonitor.WebApp.Admin.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Admin.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ClientesController : BaseController
    {
        private readonly AuthService _authService;

        public ClientesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
        }

        // POST: clientes/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                var usuariosCount = await manager.ObtenerClientesCountAsync();
                var usuarios = await manager.ObtenerClientesDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<ClienteDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<ClienteDTO>
                    {
                        RecordsTotal = usuariosCount,
                        RecordsFiltered = usuarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new ClienteDTO().From(usuario)).ToList()
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

        // GET: clientes/basics/data
        // GET: clientes/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                ClienteDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var cliente = await manager.ObtenerClienteAsync(clienteId);
                    entity = new ClienteDTO().From(cliente);
                }

                var zonasManager = new ZonasManager(_serviceProvider);
                var zonas = await zonasManager.ObtenerZonasActivasAsync();

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        zonas = zonas.Select(zona => new ZonaDTO().From(zona))
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

        // POST: clientes/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> PostSaveAsync([FromBody] ClienteDTO clienteDto)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                var cliente = await manager.GuardarClienteAsync(clienteDto.ToModel());

                await RegistrarAccionAsync(clienteId: 0, cliente.ClienteId, nameof(Cliente), string.IsNullOrEmpty(clienteDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<ClienteDTO>
                {
                    Success = true,
                    Data = new ClienteDTO().From(cliente)
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

        // GET: clientes/search?filter={filter}
        [HttpGet]
        [ActionName("search")]
        public async Task<IActionResult> GetSearchAsync([FromQuery] string filter = null)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                var clientes = await manager.BuscarClientesAsync(size: 20, filter);

                return Ok(new ApiResultDTO<AutocompleteResponseDTO<ClienteDTO>>
                {
                    Success = true,
                    Data = new AutocompleteResponseDTO<ClienteDTO>
                    {
                        Total = clientes.Count,
                        Results = clientes.Select(cliente => new ClienteDTO().From(cliente)).ToList()
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

        // DELETE: clientes/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new ClientesManager(_serviceProvider);
                await manager.DesactivarClienteAsync(clienteId);

                await RegistrarAccionAsync(clienteId: 0, clienteId, nameof(Cliente), "Baja");

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

        // POST: clientes/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new ClientesManager(_serviceProvider);
                await manager.ActivarClienteAsync(clienteId);

                await RegistrarAccionAsync(clienteId: 0, clienteId, nameof(Cliente), "Alta");

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

        // POST: clientes/syncs/list?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("syncs/list")]
        [TienePermiso(Permiso = "abm_clientes_dispositivos")]
        public async Task<IActionResult> PostListSincronizadoresAsync([FromBody] DataTableRequestDTO request, [FromQuery] string encryptedId = null)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var repository = new SynchronizerRepository(_configurationService);
                var synchronizers = await repository.ListByClienteAsync(clienteId, request.Search.Value, request.Start, request.Length);

                return Ok(new ApiResultDTO<DataTableResponseDTO<SyncDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<SyncDTO>
                    {
                        RecordsTotal = synchronizers.FirstOrDefault()?.TotalRegistros ?? 0,
                        RecordsFiltered = synchronizers.FirstOrDefault()?.TotalFiltrados ?? 0,
                        Records = synchronizers.Select(sync => new SyncDTO().From(sync)).ToList()
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

        // DELETE: clientes/syncs/delete?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("syncs/delete")]
        [TienePermiso(Permiso = "abm_clientes_dispositivos")]
        public async Task<IActionResult> DeleteSyncAsync([FromQuery] string encryptedId)
        {
            try
            {
                var syncInstanceId = EncryptionService.Decrypt<string>(Uri.UnescapeDataString(encryptedId));

                var manager = new SynchronizerRepository(_configurationService);
                var tokens = await manager.BajaAsync(syncInstanceId);

                tokens.ForEach(async (token) => await _authService.DestroyTokenAsync(token));

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

        // POST: clientes/syncs/devices/list?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("syncs/devices/list")]
        [TienePermiso(Permiso = "abm_clientes_dispositivos")]
        public async Task<IActionResult> PostListDispositivosAsync([FromBody] DataTableRequestDTO request, [FromQuery] string encryptedId = null)
        {
            try
            {
                var syncInstanceId = EncryptionService.Decrypt<string>(Uri.UnescapeDataString(encryptedId));

                var repository = new SynchronizerRepository(_configurationService);
                var devices = await repository.ListDevicesBySyncAsync(syncInstanceId, request.Search.Value, request.Start, request.Length);

                return Ok(new ApiResultDTO<DataTableResponseDTO<DeviceDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<DeviceDTO>
                    {
                        RecordsTotal = devices.FirstOrDefault()?.TotalRegistros ?? 0,
                        RecordsFiltered = devices.FirstOrDefault()?.TotalFiltrados ?? 0,
                        Records = devices.Select(dev => new DeviceDTO().From(dev)).ToList()
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
