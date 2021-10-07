using Natom.AccessMonitor.Services.MQ.Entities;
using Natom.AccessMonitor.Services.MQ.Exceptions;
using Natom.AccessMonitor.Services.MQ.WorkerUtilities;
using Natom.AccessMonitor.Services.MQ.WorkerUtilities.Config;
using Natom.AccessMonitor.Sync.Entities.DTO;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker
{
    public class Worker : CycleWorkerMQ<Worker, MessageMQ, WorkerMQConfig>
    {
        public Worker(IServiceProvider serviceProvider) : base(serviceProvider)
        {

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

        private Task DataBlockToBulkInsertAsync(MessageMQ message, CancellationToken cancellationToken)
        {
            var lastSyncRegistered = message.CreationDateTime;
            var dataBlock = JsonConvert.DeserializeObject<DataBlockForSyncDto>(message.Message);
            throw new NotImplementedException();
        }
    }
}
