using MongoDB.Bson;
using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations
{
    /// <summary>
    /// Checks and caches whether the connected MongoDB server supports transactions, asynchronously.
    /// </summary>
    internal class TransactionSupportChecker : ITransactionSupportChecker
    {
        private readonly IMongoClient _client;

        public TransactionSupportChecker(IMongoClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Asynchronously checks whether the connected MongoDB server supports transactions.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if transactions are supported; otherwise, <c>false</c>.</returns>
        public async Task<bool> IsTransactionSupportedAsync(CancellationToken cancellationToken)
        {
            // Get server information
            var isMasterCommand = new BsonDocument("ismaster", 1); // or "hello" in newer versions
            var result = await _client.GetDatabase("admin")
                .RunCommandAsync<BsonDocument>(isMasterCommand, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            // Check for replica set or sharded cluster
            bool isReplicaSet = result.Contains("setName");
            bool isMongos = result.Contains("msg") && result["msg"] == "isdbgrid"; // mongos = sharded cluster

            // Transactions are supported in replica sets (4.0+) and sharded clusters (4.2+)
            return isReplicaSet || isMongos;
        }
    }
}