using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace SimpleMongoMigrations
{
    public class TransactionSupportChecker
    {
        private readonly Lazy<bool> _isTransactionSupported;

        public TransactionSupportChecker(IMongoClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            _isTransactionSupported = new Lazy<bool>(() => DetectTransactionSupport(client));
        }

        public bool IsTransactionSupported => _isTransactionSupported.Value;

        private bool DetectTransactionSupport(IMongoClient client)
        {
            // Get server information
            var isMasterCommand = new BsonDocument("ismaster", 1); // or "hello" in newer versions
            var result = client.GetDatabase("admin").RunCommand<BsonDocument>(isMasterCommand);

            // Check for replica set or sharded cluster
            bool isReplicaSet = result.Contains("setName");
            bool isMongos = result.Contains("msg") && result["msg"] == "isdbgrid"; // mongos = sharded cluster

            // Transactions are supported in replica sets (4.0+) and sharded clusters (4.2+)
            return isReplicaSet || isMongos;
        }
    }
}