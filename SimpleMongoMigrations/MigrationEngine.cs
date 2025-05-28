using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations
{
    /// <summary>
    /// Engine for running MongoDB migrations with optional transaction support.
    /// </summary>
    public class MigrationEngine
    {
        private readonly string _databaseName;
        private readonly string _connectionString;
        private readonly IMongoClient _externalClient;
        private readonly TransactionScope _transactionScope;
        private readonly MigrationScanner _migrationScanner;

        static MigrationEngine()
        {
            BsonSerializer.TryRegisterSerializer(typeof(Version), new VerstionSerializer());
        }

        internal MigrationEngine(
            string connectionString,
            string databaseName,
            TransactionScope transactionScope,
            Assembly assembly,
            IMongoClient client)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _transactionScope = transactionScope;
            _externalClient = client;
            _migrationScanner = new MigrationScanner(assembly);
        }

        /// <summary>
        /// Runs all pending migrations using the configured transaction scope.
        /// </summary>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (_externalClient == null)
            {
                using (var client = new MongoClient(_connectionString))
                {
                    await RunInternalAsync(client, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                await RunInternalAsync(_externalClient, cancellationToken).ConfigureAwait(false);
            }           
        }

        private async Task RunInternalAsync(IMongoClient client, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(_databaseName);
            var migrationRunner = new MigrationRunner(
                client,
                database,
                new MigrationRepository(database),
                _migrationScanner,
                new TransactionSupportChecker(client));
            await migrationRunner.RunAsync(_transactionScope, cancellationToken).ConfigureAwait(false);
        }
    }
}
