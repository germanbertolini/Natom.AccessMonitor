using Microsoft.Extensions.Configuration;
using Natom.AccessMonitor.Services.Configuration.Repository;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Services.Logger.Entities;
using Natom.AccessMonitor.Services.Logger.PackageConfig;
using Natom.AccessMonitor.Services.Logger.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Services.Logger.Services
{
    public class LoggerService
    {
        private readonly LoggerServiceConfig _config;
        private readonly ConfigurationService _configuration;
        private readonly DiscordService _discordService;

        private DateTime _lastExceptionOnDiscord;
        private DateTime _lastExceptionLogsOnDiscord;

        private List<Transaction> _transactions;
        private List<TransactionLog> _transactionLogs;

        public LoggerService(ConfigurationService configuration, DiscordService discordService, LoggerServiceConfig config)
        {
            _config = config;
            _configuration = configuration;
            _discordService = discordService;
            _transactions = new List<Transaction>();
            _transactionLogs = new List<TransactionLog>();
            _lastExceptionOnDiscord = DateTime.MinValue;
        }

        public async Task BulkInsertAsync()
        {
            var connectionString = await _configuration.GetValueAsync("ConnectionStrings.DbLogs");
            var repository = new LogRepository(connectionString);

            var toBulk = _transactions.Count < _config.BulkInsertSize ? _transactions.Count : _config.BulkInsertSize;
            var transactionsToCopy = _transactions.GetRange(0, toBulk);

            try
            {
                await repository.BulkInsertAsync(transactionsToCopy);

                //HACER BULK INSERT DE LOS TRANSACTIONS EXTRAIDOS
                _transactions.RemoveRange(0, transactionsToCopy.Count);
            }
            catch (Exception ex)
            {
                //ENVIAR EXCEPTION A DISCORD. HACER LOGICA PARA QUE LA EXCEPCION SE ENVIE CADA 30 MINUTOS
                if ((DateTime.Now - _lastExceptionOnDiscord).TotalMinutes >= 30)
                {
                    _= Task.Run(() => _discordService.LogExceptionAsync(ex, null, new { TransactionsToBulkCounter = _transactions.Count() }));
                    _lastExceptionOnDiscord = DateTime.Now;
                }
            }


            toBulk = _transactionLogs.Count < _config.BulkInsertSize ? _transactionLogs.Count : _config.BulkInsertSize;
            var transactionLogsToCopy = _transactionLogs.GetRange(0, toBulk);
            
            try
            {
                await repository.BulkInsertAsync(transactionLogsToCopy);

                //HACER BULK INSERT DE LOS TRANSACTIONS EXTRAIDOS
                _transactionLogs.RemoveRange(0, transactionLogsToCopy.Count);
            }
            catch (Exception ex)
            {
                //ENVIAR EXCEPTION A DISCORD. HACER LOGICA PARA QUE LA EXCEPCION SE ENVIE CADA 30 MINUTOS
                if ((DateTime.Now - _lastExceptionLogsOnDiscord).TotalMinutes >= 30)
                {
                    _ = Task.Run(() => _discordService.LogExceptionAsync(ex, null, new { TransactionsLogsToBulkCounter = _transactionLogs.Count() }));
                    _lastExceptionLogsOnDiscord = DateTime.Now;
                }
            }
        }

        public Transaction CreateTransaction(string scope, string lang, string ip, string urlRequested, string actionRequested, long? userId, string os, string appVersion, string hostName, int? port)
        {
            if (scope.Length > 20) scope = scope.Substring(0, 20);

            var trace = new Transaction()
            {
                IP = ip,
                TraceTransactionId = Guid.NewGuid().ToString("N"),
                Scope = scope,
                Lang = lang,
                DateTime = DateTime.Now,
                UrlRequested = urlRequested,
                ActionRequested = actionRequested,
                UserId = userId,
                OS = os,
                AppVersion = appVersion,
                HostName = hostName,
                Port = port
            };
            _transactions.Add(trace);

            return trace;
        }

        /// <summary>
        /// Loguea el Info + Opción de reporte a Discord
        /// </summary>
        public void LogInfo(string transactionId, string message, object data = null, bool logOnDiscord = false)
        {
            _transactionLogs.Add(new TransactionLog()
            {
                Type = "INFO",
                TraceTransactionId = transactionId,
                DateTime = DateTime.Now,
                Description = message,
                Data = (data == null) ? null : JsonConvert.SerializeObject(data)
            });

            if (logOnDiscord)
                Task.Run(() => _discordService.LogInfoAsync(message, transactionId, data));
        }

        /// <summary>
        /// Loguea el Warning
        /// </summary>
        public void LogWarning(string transactionId, string message, object data = null)
        {
            _transactionLogs.Add(new TransactionLog()
            {
                Type = "WARNI",
                TraceTransactionId = transactionId,
                DateTime = DateTime.Now,
                Description = message,
                Data = (data == null) ? null : JsonConvert.SerializeObject(data)
            });
        }

        /// <summary>
        /// Loguea el Error
        /// </summary>
        public void LogError(string transactionId, string message, object data = null)
        {
            _transactionLogs.Add(new TransactionLog()
            {
                Type = "ERROR",
                TraceTransactionId = transactionId,
                DateTime = DateTime.Now,
                Description = message,
                Data = (data == null) ? null : JsonConvert.SerializeObject(data)
            });
        }


        /// <summary>
        /// Loguea el Rechazo + Lo reporta en Discord
        /// </summary>
        public void LogBounce(string transactionId, string description, object accessToken)
        {
            _transactionLogs.Add(new TransactionLog()
            {
                Type = "INFO",
                TraceTransactionId = transactionId,
                DateTime = DateTime.Now,
                Description = description,
                Data = JsonConvert.SerializeObject(new { AccessToken = accessToken })
            });

            Task.Run(() => _discordService.LogInfoAsync(":no_entry: TRANSACCIÓN RECHAZADA :no_entry:", transactionId, new { AccessToken_Data = accessToken }));
        }

        /// <summary>
        /// Loguea el Exception + Lo reporta en Discord
        /// </summary>
        public void LogException(string transactionId, Exception ex, object transactionParams = null)
        {
            var realException = ex.InnerException?.InnerException ?? ex.InnerException ?? ex;
            var log = new TransactionLog()
            {
                Type = "EXCEP",
                TraceTransactionId = transactionId,
                DateTime = DateTime.Now,
                Description = "Exception has occurred",
                Data = JsonConvert.SerializeObject(new
                {
                    FullException = ex.ToString(),
                    TransactionParams = transactionParams
                })
            };
            _transactionLogs.Add(log);

            Task.Run(() => _discordService.LogExceptionAsync(realException, transactionId, transactionParams));
        }
    }
}
