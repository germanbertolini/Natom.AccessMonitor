using Dapper;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker.Extensions
{
    public static class RetryableRepositoryExtensions
    {
        public static List<int> retryableSqlErrors = new List<int>() {
            -2,    //Timeout
            1204,  //NoLock
            1205,  //Deadlock
            30053  //WordbreakerTimeout
        };
        public static List<int> retryableSystemErrors = new List<int>() {
            0x102,  //Timeout expired
            0x121   //Semaphore timeout expired
        };

        /// <summary>
        /// Author: German Bertolini
        /// Date: 22/09/2021
        /// Ejecuta un command aplicando la política de reintentos
        /// </summary>
        public async static Task<int> RetryableExecuteAsync(this IDbConnection cnn, CommandDefinition command)
                                            => await BuildRetryPolicy<int>().ExecuteAsync(() => cnn.ExecuteAsync(command));

        /// <summary>
        /// Author: German Bertolini
        /// Date: 22/09/2021
        /// Ejecuta un command aplicando la política de reintentos
        /// </summary>
        public async static Task<int> RetryableExecuteAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
                                            => await BuildRetryPolicy<int>().ExecuteAsync(() => cnn.ExecuteAsync(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken)));


        /// <summary>
        /// Author: German Bertolini
        /// Date: 29/09/2021
        /// Ejecuta un command aplicando la política de reintentos
        /// </summary>
        public async static Task<IEnumerable<dynamic>> RetryableQueryAsync(this IDbConnection cnn, CommandDefinition commandDefinition)
                                            => await BuildRetryPolicy<IEnumerable<dynamic>>().ExecuteAsync(() => cnn.QueryAsync(commandDefinition));

        /// <summary>
        /// Author: German Bertolini
        /// Date: 29/09/2021
        /// Ejecuta un command aplicando la política de reintentos
        /// </summary>
        public async static Task<IEnumerable<TResult>> RetryableQueryAsync<TResult>(this IDbConnection cnn, CommandDefinition commandDefinition)
                                            => await BuildRetryPolicy<IEnumerable<TResult>>().ExecuteAsync(() => cnn.QueryAsync<TResult>(commandDefinition));

        /// <summary>
        /// Author: German Bertolini
        /// Date: 22/09/2021
        /// Ejecuta un command aplicando la política de reintentos
        /// </summary>
        public async static Task<IEnumerable<TResult>> RetryableQueryAsync<TResult>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
                                            => await BuildRetryPolicy<IEnumerable<TResult>>().ExecuteAsync(() => cnn.QueryAsync<TResult>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken)));


        /// <summary>
        /// Author: German Bertolini
        /// Date: 22/09/2021
        /// Crea una Policy para los reintentos por Excepción
        /// </summary>
        private static AsyncRetryPolicy<TResult> BuildRetryPolicy<TResult>()
                                            => Policy<TResult>
                                                .Handle<SqlException>(ShouldRetryOn)
                                                .Or<TimeoutException>()
                                                .OrInner<Win32Exception>(ShouldRetryOn)
                                                .WaitAndRetryAsync(
                                                retryCount: 5,
                                                delayForRetry => TimeSpan.FromMilliseconds(500)
                                            );

        private static bool ShouldRetryOn(SqlException ex)
        {
            foreach (SqlError err in ex.Errors)
                return retryableSqlErrors.Any(errNro => errNro.Equals(err.Number));
            return false;
        }

        private static bool ShouldRetryOn(Win32Exception ex)
        {
            return retryableSystemErrors.Any(errNro => errNro.Equals(ex.NativeErrorCode));
        }
    }
}
