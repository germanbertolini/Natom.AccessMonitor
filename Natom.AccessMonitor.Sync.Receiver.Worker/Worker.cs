using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Services.MQ.Entities;
using Natom.AccessMonitor.Services.MQ.Exceptions;
using Natom.AccessMonitor.Services.MQ.WorkerUtilities;
using Natom.AccessMonitor.Services.MQ.WorkerUtilities.Config;
using Natom.AccessMonitor.Sync.Entities.DTO;
using Natom.AccessMonitor.Sync.Receiver.Worker.Repository;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker
{
    public class Worker : CycleWorkerMQ<Worker, MessageMQ, WorkerMQConfig>
    {
        private readonly ConfigurationService _configuration;

        public Worker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _configuration = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
        }

        /// <summary>
        /// Evento que es llamado por cada mensaje leído.
        /// </summary>
        public override async Task OnReadMessageAsync(ReadMessageMQ<MessageMQ> readMessage, CancellationToken cancellationToken, DateTimeOffset executionTime)
        {
            try
            {
                MessageMQ message = readMessage.Content;
                switch (message.Topic)
                {
                    case "DataBlockToBulkInsert":
                        await DataBlockToBulkInsertAsync(message, cancellationToken);
                        break;
                }

            }
            catch (Exception ex)
            {
                bool isSqlException = ex is SqlException;
                bool isDuplicatedPK = isSqlException && ((SqlException)ex).Number.Equals(2627);
                if (isDuplicatedPK)
                    throw ex;   //LOS DUPLICADOS LOS MANDAMOS A LA COLA DE ERROR
                else
                    throw new RetryableException(ex, delayMiliseconds: 10000);
            }
            finally
            {
                if (cancellationToken.IsCancellationRequested)
                    readMessage.AbortRemovingFromQueue();
            }
        }

        private async Task DataBlockToBulkInsertAsync(MessageMQ message, CancellationToken cancellationToken)
        {
            var lastSyncRegistered = message.CreationDateTime;
            var dataBlock = JsonConvert.DeserializeObject<DataBlockForSyncDto>(message.Message);

            var movementsRepository = new MovementsRepository(_configuration);
            await movementsRepository.BulkInsertAsync(message.ProducerInfo.ClientId, message.ProducerInfo.SyncInstanceId, lastSyncRegistered: message.CreationDateTime, dataBlock);

            var synchronizerRepository = new SynchronizerRepository(_configuration);
            await synchronizerRepository.UpdateLastSyncAsync(lastSyncRegistered, message.ProducerInfo.SyncInstanceId);
            await synchronizerRepository.AddOrUpdateDeviceInfoAsync(message.ProducerInfo.SyncInstanceId, dataBlock.DeviceId, dataBlock.DeviceName,
                                                                    dataBlock.LastConfigurationAt, dataBlock.DeviceSerialNumber, dataBlock.DeviceModel,
                                                                    dataBlock.DeviceBrand, dataBlock.DeviceDateTimeFormat, dataBlock.DeviceFirmwareVersion);
        }
    }
}
