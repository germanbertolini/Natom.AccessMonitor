﻿using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Core.Biz.Managers;
using Natom.Extensions.Auth.Attributes;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.DataTable;
using Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Goals;
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
    public class GoalsController : BaseController
    {
        public GoalsController(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // POST: goals/list?encryptedId={encryptedId}&filter={filter}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string encryptedId, [FromQuery] string status = null)
        {
            try
            {
                if ((_accessToken.ClientId ?? 0) == 0)
                    throw new HandledException("El administrador de Natom solamente puede visualizar y administrar los accesos / porterías del cliente desde la aplicación de -Admin-");

                var placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedId));

                var manager = new GoalsManager(_serviceProvider);
                var goalsCount = await manager.ObtenerCountAsync(placeId);
                var goals = await manager.ObtenerDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, placeId, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<GoalDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<GoalDTO>
                    {
                        RecordsTotal = goalsCount,
                        RecordsFiltered = goals.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = goals.Select(goal => new GoalDTO().From(goal)).ToList()
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

        // POST: goals/list_actives
        [HttpPost]
        [ActionName("list_actives")]
        public async Task<IActionResult> PostListActivesAsync()
        {
            try
            {
                var manager = new GoalsManager(_serviceProvider);
                var goals = await manager.ObtenerActivasAsync(_accessToken.ClientId ?? -1);

                return Ok(new ApiResultDTO<List<GoalDTO>>
                {
                    Success = true,
                    Data = goals.Select(g => new GoalDTO().From(g)).ToList()
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

        // GET: goals/basics/data
        // GET: goals/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new GoalsManager(_serviceProvider);
                GoalDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var goalId = EncryptionService.Decrypt<int, Goal>(Uri.UnescapeDataString(encryptedId));
                    var goal = await manager.ObtenerAsync(goalId);
                    entity = new GoalDTO().From(goal);
                }

                var placesManager = new PlacesManager(_serviceProvider);
                var places = await placesManager.ObtenerActivasAsync(_accessToken.ClientId ?? -1);

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        places = places.Select(p => new PlaceDTO().From(p)).ToList()
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

        // POST: goals/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> PostSaveAsync([FromBody] GoalDTO goalDto)
        {
            try
            {
                if ((_accessToken.ClientId ?? 0) == 0)
                    throw new HandledException("El administrador de Natom solamente puede visualizar y administrar los accesos / porterías del cliente desde la aplicación de -Admin-");

                var manager = new GoalsManager(_serviceProvider);
                var goal = await manager.GuardarAsync(_accessToken.ClientId.Value, goalDto.ToModel());

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId.Value, goal.GoalId, nameof(Goal), string.IsNullOrEmpty(goalDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<GoalDTO>
                {
                    Success = true,
                    Data = new GoalDTO().From(goal)
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

        // DELETE: goals/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var goalId = EncryptionService.Decrypt<int, Goal>(Uri.UnescapeDataString(encryptedId));

                var manager = new GoalsManager(_serviceProvider);
                await manager.DesactivarAsync(goalId);

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, goalId, nameof(Goal), "Baja");

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

        // POST: goals/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        [TienePermiso(Permiso = "abm_places_goals")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var goalId = EncryptionService.Decrypt<int, Goal>(Uri.UnescapeDataString(encryptedId));

                var manager = new GoalsManager(_serviceProvider);
                await manager.ActivarAsync(goalId);

                await RegistrarAccionAsync(clienteId: _accessToken.ClientId ?? -1, goalId, nameof(Goal), "Alta");

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
