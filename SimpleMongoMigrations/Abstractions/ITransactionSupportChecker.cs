using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Abstractions
{
    internal interface ITransactionSupportChecker
    {
        /// <summary>
        /// Asynchronously checks whether the connected MongoDB server supports transactions.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if transactions are supported; otherwise, <c>false</c>.</returns>
        Task<bool> IsTransactionSupportedAsync(CancellationToken cancellationToken);
    }
}
