using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Services.Logger.Services;
using Natom.AccessMonitor.Services.MQ.WorkerUtilities.Config;
using Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Models;
using Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Results;
using Natom.AccessMonitor.Sync.Receiver.Worker.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker
{
    public class MovementsProcessorWorker : BackgroundService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly LoggerService _loggerService;
        private readonly ConfigurationService _configuration;

        protected WorkerConfig _workerConfig { get; private set; }
        protected DateTimeOffset _firedAt { get; private set; }
        


        public MovementsProcessorWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));
            _configuration = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));

            var configuration = (IConfiguration)serviceProvider.GetService(typeof(IConfiguration));

            //WORKER CONFIG
            _workerConfig = configuration.GetSection("MovementsProcessorWorker").Get<WorkerConfig>();
        }

        /// <summary>
        /// Ejecución del Worker
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _loggerService.LogInfo(transactionId: "", $"Worker {_workerConfig.InstanceName} iniciado.", GetTransactionInfoForWorker(), logOnDiscord: true);

            await Task.Delay(1000);

            while (!stoppingToken.IsCancellationRequested)
            {
                _firedAt = DateTimeOffset.Now;

                try
                {
                    Parallel.For(fromInclusive: 0,
                                    toExclusive: _workerConfig.Process.Threads,
                                    iThread => DoWorkAsync(iThread + 1, stoppingToken, _firedAt).Wait());
                }
                catch (Exception ex)
                {
                    _loggerService.LogException(transactionId: "", ex, GetTransactionInfoForWorker());
                }

                //AGUARDAMOS A LA PRÓXIMA EJECUCIÓN
                int waitForDelay = _firedAt.Millisecond + (_workerConfig.Process.MinIntervalMS) - DateTimeOffset.Now.Millisecond;
                if (waitForDelay < 0) waitForDelay = 0;
                await Task.Delay(waitForDelay, stoppingToken);
            }

            _loggerService.LogInfo(transactionId: "", $"Worker {_workerConfig.InstanceName} detenido.", GetTransactionInfoForWorker(), logOnDiscord: true);
        }

        /// <summary>
        /// Realiza la tarea del Worker
        /// </summary>
        private async Task DoWorkAsync(int threadNumber, CancellationToken stoppingToken, DateTimeOffset executionTime)
        {
            var transactionId = executionTime.ToString("yyyyMMddHHmmss");
            var clientesRepository = new ClientesRepository(_configuration);
            var movementsRepository = new MovementsRepository(_configuration);

            int procesadosConMovimientosTerminanEnError = 0, procesadosConMovimientosCorrectamente = 0, noProcesadosPorActivarseCancellationToken = 0;

            var clientes = await clientesRepository.GetListAsync();

            //LOGUEAMOS INICIO DEL PROCESO
            _loggerService.LogInfo(transactionId, "Inicio proceso MovementsProcessorWorker", new { clientesPorProcesar = clientes.Count });

            foreach (var cliente in clientes)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                _loggerService.LogInfo(transactionId, "Inicio lectura movimientos pendientes de procesar", new { ClienteId = cliente.ClienteId, ClienteNombre = cliente.EsEmpresa ? cliente.RazonSocial : (cliente.Nombre + " " + cliente.Apellido) });
                var toProcess = await movementsRepository.GetPendingToProcessByClientAsync(cliente.ClienteId);
                _loggerService.LogInfo(transactionId, "Fin lectura movimientos pendientes de procesar", new { movementsCount = toProcess.Movements.Count, docketRangesCount = toProcess.DocketsRanges.Count, lastInOutCount = toProcess.LastInOut?.Count ?? 0 });

                await Task.Delay(100, stoppingToken);


                if (toProcess.Movements.Count == 0)
                    continue;

                var movementsToUpdate = new List<MovementProcessed>();
                var movementsToAdd = new List<MovementProcessed>();
                var movementIdsProcessed = new List<long>();

                
                try
                {
                    //LOGICA DE MOVIMIENTOS!
                    _loggerService.LogInfo(transactionId, "Inicio procesamiento de movimientos pendientes de procesar");
                    var movementsByDocket = toProcess.Movements.GroupBy(m => new { m.DocketNumber, m.DocketId, m.PlaceId }, (k, v) => new { GroupInfo = k, Movements = v.OrderBy(o => o.DateTime) });
                    foreach (var data in movementsByDocket)
                    {
                        var docketId = data.GroupInfo.DocketId;
                        var docketNumber = data.GroupInfo.DocketNumber;
                        var placeId = data.GroupInfo.PlaceId;
                        var movements = data.Movements;
                        var turnosOriginal = toProcess.DocketsRanges.Where(r => r.DocketId == docketId).OrderBy(r => r.DayOfWeek).ThenBy(r => r.From).ToList();
                        var horarioMedianoche = turnosOriginal.FirstOrDefault(h => h.From.Hours > h.To.Hours);
                        var lastInOut = toProcess.LastInOut.FirstOrDefault(l => l.DocketId == docketId);

                        if (movements.Count() > 0)
                        {
                            DateTime? lastMovimiento = lastInOut?.Out ?? lastInOut?.In;
                            var movementsByHourRangeAndJornada = new Dictionary<DateTime, Dictionary<spMovementsProcessorSelectByClientDetailDocketRangesResult, List<spMovementsProcessorSelectByClientDetailMovementsResult>>>();

                            foreach (var movement in movements)
                            {
                                //SI FICHO DOS VECES SEGUIDAS, AVANZAMOS AL SIGUIENTE
                                if (lastMovimiento != null && (movement.DateTime - lastMovimiento).Value.TotalMinutes <= 2)
                                    continue;

                                lastMovimiento = movement.DateTime;

                                //DETERMINAMOS LA JORNADA
                                var jornada = movement.DateTime.Date;
                                if (horarioMedianoche != null && movement.DateTime.Hour >= 0 && movement.DateTime.Hour < horarioMedianoche.To.Hours + 5)
                                    jornada = movement.DateTime.AddDays(-1).Date;

                                //DETERMINAMOS EL TURNO
                                var dayOfWeek = (int)jornada.DayOfWeek;
                                if (dayOfWeek == 0) dayOfWeek = 7;

                                var turnos = turnosOriginal.Where(t => t.DayOfWeek.Equals(dayOfWeek)).ToList();
                                var turnosUmbral = turnosOriginal.Where(t => t.DayOfWeek.Equals(dayOfWeek)).Select(t => new spMovementsProcessorSelectByClientDetailDocketRangesResult { From = t.From, To = t.To }).ToList();

                                if (turnosUmbral.Count == 2)
                                {
                                    var diff = (turnosUmbral.Last().From - turnosUmbral.First().To).TotalMinutes;
                                    var mitadMinutos = Convert.ToInt32(diff / 2);
                                    turnosUmbral[0].To = turnosUmbral[0].To.Add(TimeSpan.FromMinutes(mitadMinutos));
                                    turnosUmbral[1].From = turnosUmbral[1].From.Add(TimeSpan.FromMinutes(-mitadMinutos));
                                }

                                var turno = turnos.First();
                                if (turnosUmbral.Count == 2)
                                {
                                    var indexTurnoMedianoche = turnosUmbral.FindIndex(t => t.From > t.To);
                                    if (indexTurnoMedianoche == -1 && movement.DateTime.TimeOfDay >= turnosUmbral[1].From && movement.DateTime.TimeOfDay <= turnosUmbral[1].From.Add(TimeSpan.FromHours(5)))
                                        turno = turnos[1];
                                    else if (indexTurnoMedianoche == 0 && (movement.DateTime.TimeOfDay >= turnosUmbral[0].From.Add(TimeSpan.FromHours(-3)) || movement.DateTime.TimeOfDay <= turnosUmbral[0].To))
                                        turno = turnos[0];
                                    else if (indexTurnoMedianoche == 1 && (movement.DateTime.TimeOfDay >= turnosUmbral[1].From || movement.DateTime.TimeOfDay <= turnosUmbral[1].To.Add(TimeSpan.FromHours(5))))
                                        turno = turnos[1];
                                }

                                //LO AGREGAMOS A LA COLECCIÓN AGRUPADOS POR JORNADA Y TURNO
                                if (!movementsByHourRangeAndJornada.ContainsKey(jornada))
                                    movementsByHourRangeAndJornada.Add(jornada, new Dictionary<spMovementsProcessorSelectByClientDetailDocketRangesResult, List<spMovementsProcessorSelectByClientDetailMovementsResult>>());

                                if (!movementsByHourRangeAndJornada[jornada].ContainsKey(turno))
                                    movementsByHourRangeAndJornada[jornada].Add(turno, new List<spMovementsProcessorSelectByClientDetailMovementsResult>());

                                movementsByHourRangeAndJornada[jornada][turno].Add(movement);
                            }

                            //AHORA PREPARAMOS LOS DATOS PARA IMPACTARLOS EN "_PROCESSED"
                            foreach (var jornada in movementsByHourRangeAndJornada.Keys)
                            {
                                foreach (var turno in movementsByHourRangeAndJornada[jornada].Keys)
                                {
                                    bool outWasEstimated = false;
                                    var turnoMovs = movementsByHourRangeAndJornada[jornada][turno];
                                    var minMovement = turnoMovs.First(m => m.DateTime.Equals(turnoMovs.Min(mov => mov.DateTime)));
                                    var maxMovement = turnoMovs.First(m => m.DateTime.Equals(turnoMovs.Max(mov => mov.DateTime)));

                                    if (minMovement.DateTime.Equals(maxMovement.DateTime))
                                        maxMovement = null;

                                    var turnoDurationMins = turno.To > turno.From
                                                                    ? (turno.To - turno.From).TotalMinutes
                                                                    : ((TimeSpan.FromHours(24) - turno.From) + turno.To).TotalMinutes;


                                    //SI YA TIENE UN PROCESSED GRABADO EN LA BASE DE DATOS PARA EL MISMO TURNO Y JORNADA LE ACTUALIZAMOS EL HORARIO DE SALIDA
                                    if (lastInOut != null && lastInOut.Date.Date.Equals(jornada) && lastInOut.ExpectedIn.Equals(turno.From))
                                    {
                                        var finalMov = (maxMovement ?? minMovement);
                                        lastInOut.Out = finalMov.DateTime;
                                        lastInOut.OutGoalId = finalMov.GoalId;
                                        lastInOut.OutPlaceId = finalMov.PlaceId;
                                        lastInOut.OutDeviceId = finalMov.DeviceId;
                                        lastInOut.OutDeviceMovementType = finalMov.MovementType;
                                        lastInOut.OutWasEstimated = outWasEstimated;
                                        lastInOut.OutProcessedAt = DateTime.Now;
                                        lastInOut.PermanenceTime = (finalMov.DateTime - lastInOut.In);

                                        if (lastInOut.PermanenceTime.Value.TotalDays >= 1)
                                            lastInOut.PermanenceTime = null;

                                        movementsToUpdate.Add(lastInOut);
                                    }

                                    //SI ES UN PROCESSED TOTALMENTE NUEVO
                                    else
                                    {
                                        if (maxMovement == null)
                                        {
                                            //PRIMERO CALCULAMOS LA HORA SALIDA PROMEDIO POR SI NO LA LLEGO A FICHAR
                                            var fechaHoraLimite = new DateTime(jornada.Year, jornada.Month, jornada.Day, turno.From.Hours, turno.From.Minutes, 0);
                                            fechaHoraLimite = fechaHoraLimite.AddMinutes(turnoDurationMins);

                                            TimeSpan? outPromedio = null;

                                            //SI ESTA SINCRONIZANDO MAS DE TRES JORNADAS PRIMERO INTENTAMOS TOMAR EL PROMEDIO DE LOS ULTIMAS (EN LO POSIBLE 5) ANTERIORES A LA JORNADA QUE SE ESTÁ PROCESANDO
                                            if (movementsByHourRangeAndJornada.Keys.Count >= 3)
                                            {
                                                var ultimasSalidas = movementsToUpdate.Where(m => m.DocketId == minMovement.DocketId).ToList();
                                                ultimasSalidas.AddRange(movementsToAdd.Where(m => m.DocketId == minMovement.DocketId).ToList());
                                                ultimasSalidas = ultimasSalidas.Where(m => m.Out.HasValue && m.Out.Value < minMovement.DateTime && m.ExpectedIn == turno.From && m.ExpectedOut == turno.To).OrderByDescending(m => m.Out.Value).ToList();
                                                if (ultimasSalidas.Count > 5)
                                                    ultimasSalidas = ultimasSalidas.Take(5).ToList();
                                                if (ultimasSalidas.Count() > 2)
                                                {
                                                    var promedioMins = Convert.ToInt32(ultimasSalidas.Sum(s => s.Out.Value.TimeOfDay.TotalMinutes) / ultimasSalidas.Count());
                                                    if (promedioMins > 0)
                                                        outPromedio = TimeSpan.FromMinutes(promedioMins);
                                                }
                                            }

                                            //SI NO QUEDA OTRA, ENTONCES TOMAMOS EL PROMEDIO DE LA BASE DE DATOS
                                            if (outPromedio == null)
                                                outPromedio = (await movementsRepository.GetOutPromedioAsync(cliente.ClienteId, minMovement.DocketId))?.OutPromedio;

                                            if (outPromedio != null)
                                            {
                                                outWasEstimated = true;
                                                maxMovement = new spMovementsProcessorSelectByClientDetailMovementsResult
                                                {
                                                    DeviceId = minMovement.DeviceId,
                                                    DateTime = new DateTime(fechaHoraLimite.Year, fechaHoraLimite.Month, fechaHoraLimite.Day, outPromedio.Value.Hours, outPromedio.Value.Minutes, outPromedio.Value.Seconds),
                                                    DocketNumber = minMovement.DocketNumber,
                                                    MovementType = null,
                                                    GoalId = minMovement.GoalId,
                                                    PlaceId = minMovement.PlaceId,
                                                    DocketId = minMovement.DocketId
                                                };
                                            }
                                        }

                                        //POR ULTIMO LO AGREGAMOS A LA COLECCIÓN DE NUEVOS MOVIMIENTOS
                                        var newMovement = new MovementProcessed()
                                        {
                                            Date = jornada,
                                            DocketNumber = docketNumber,
                                            DocketId = docketId,
                                            In = minMovement.DateTime,
                                            ExpectedPlaceId = minMovement.ExpectedPlaceId,
                                            ExpectedIn = turno.From,
                                            InGoalId = minMovement.GoalId,
                                            InPlaceId = minMovement.PlaceId,
                                            InDeviceId = minMovement.DeviceId,
                                            InDeviceMovementType = minMovement.MovementType,
                                            InWasEstimated = false,
                                            InProcessedAt = DateTime.Now,

                                            ExpectedOut = turno.To,
                                            Out = maxMovement?.DateTime,
                                            OutGoalId = maxMovement?.GoalId,
                                            OutPlaceId = maxMovement?.PlaceId,
                                            OutDeviceId = maxMovement?.DeviceId,
                                            OutDeviceMovementType = maxMovement?.MovementType,
                                            OutWasEstimated = outWasEstimated,
                                            OutProcessedAt = maxMovement != null ? DateTime.Now : null,

                                            PermanenceTime = maxMovement != null ? (maxMovement.DateTime - minMovement.DateTime) : null
                                        };

                                        if (newMovement.PermanenceTime != null && newMovement.PermanenceTime.Value.TotalDays >= 1)
                                            newMovement.PermanenceTime = null;

                                        movementsToAdd.Add(newMovement);
                                    }
                                }
                            }

                            movementIdsProcessed.AddRange(movements.Select(m => m.MovementId).ToList());
                        }
                    }
                    _loggerService.LogInfo(transactionId, "Fin procesamiento de movimientos pendientes de procesar", new { movementsToUpdateCount = movementsToUpdate.Count, movementsToAddCount = movementsToAdd.Count, movementsProcessedCount = movementIdsProcessed.Count });

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        int cancellationTokenSeconds = 30;
                        string proceso = "";
                        try
                        {
                            //ACTUALIZAMOS EN BASE DE DATOS
                            proceso = "ProcessedUpdateAsync";
                            _loggerService.LogInfo(transactionId, "Inicio ProcessedUpdateAsync", new { cancellationTokenSeconds, ClienteId = cliente.ClienteId, movementsToUpdateCount = movementsToUpdate.Count });
                            var cancellationTokenUpdate = new CancellationTokenSource(TimeSpan.FromSeconds(cancellationTokenSeconds)).Token;
                            await movementsRepository.ProcessedUpdateAsync(cliente.ClienteId, movementsToUpdate, cancellationTokenUpdate);
                            _loggerService.LogInfo(transactionId, "Fin ProcessedUpdateAsync");
                            await Task.Delay(200);

                            //INSERTAMOS EN BASE DE DATOS
                            proceso = "ProcessedInsertAsync";
                            _loggerService.LogInfo(transactionId, "Inicio ProcessedInsertAsync", new { cancellationTokenSeconds, ClienteId = cliente.ClienteId, movementsToAddCount = movementsToAdd.Count });
                            var cancellationTokenInsert = new CancellationTokenSource(TimeSpan.FromSeconds(cancellationTokenSeconds)).Token;
                            await movementsRepository.ProcessedInsertAsync(cliente.ClienteId, movementsToAdd, cancellationTokenInsert);
                            _loggerService.LogInfo(transactionId, "Fin ProcessedInsertAsync");
                            await Task.Delay(200);

                            //MARCAMOS COMO PROCESADO
                            proceso = "MarkAsProcessedAsync";
                            _loggerService.LogInfo(transactionId, "Inicio MarkAsProcessedAsync", new { cancellationTokenSeconds, ClienteId = cliente.ClienteId, movementsProcessedCount = movementIdsProcessed.Count });
                            await movementsRepository.MarkAsProcessedAsync(cliente.ClienteId, movementIdsProcessed);
                            _loggerService.LogInfo(transactionId, "Fin MarkAsProcessedAsync");
                            await Task.Delay(200);

                            procesadosConMovimientosCorrectamente++;
                        }
                        catch (TaskCanceledException ex)
                        {
                            noProcesadosPorActivarseCancellationToken++;
                            throw new Exception($"El proceso '{proceso}' ha sido finalizado por activarse el CancellationToken de {cancellationTokenSeconds} segundos.");
                        }                        
                    }

                    //LOGUEAR FIN DE PROCESAMIENTO PARA EL <CLIENTID> Y CUANTOS REGISTROS SE ACTUALIZARON Y CUANTOS SE INSERTARON
                    //LOGUEAR ARRIBA CUANDO COMIENZA EL PROCESAMIENTO PARA EL <CLIENTID>
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("ha sido finalizado por activarse el CancellationToken"))
                        procesadosConMovimientosTerminanEnError++;

                    _loggerService.LogException(transactionId, ex, new
                    {
                        RunningInfo = new
                        {
                            ThreadNumber = threadNumber,
                            ExecutionTime = executionTime,
                            StoppingTokenActivated = stoppingToken.IsCancellationRequested
                        },
                        ClienteId = cliente.ClienteId,
                        ClienteNombre = cliente.EsEmpresa ? cliente.RazonSocial : (cliente.Nombre + " " + cliente.Apellido),
                        MovementsToUpdateCount = movementsToUpdate.Count,
                        MovementsToAddCount = movementsToAdd.Count,
                        MovementsIdsProcessed = movementIdsProcessed
                    });
                }
                _loggerService.LogInfo(transactionId, "Fin procesamiento de movimientos pendientes de procesar");
            }

            //LOGUEAMOS FIN DEL PROCESO
            _loggerService.LogInfo(transactionId, "Fin proceso MovementsProcessorWorker", new { clientesPorProcesar = clientes.Count, procesadosConMovimientosCorrectamente, procesadosConMovimientosTerminanEnError, noProcesadosPorActivarseCancellationToken });
        }


        /// <summary>
        /// Date: 14/06/2021
        /// Retorna un diccionario con datos de la instancia del Worker para persistirlo en el Logger como 'ExtraFields'
        /// </summary>
        protected object GetTransactionInfoForWorker()
        {
            var transactionInfo = new Dictionary<string, object>
            {
                ["WorkerName"] = _workerConfig.Name,
                ["WorkerInstance"] = _workerConfig.InstanceName,
                ["WorkerFullName"] = typeof(MovementsProcessorWorker).FullName
            };

            return transactionInfo;
        }
    }
}
