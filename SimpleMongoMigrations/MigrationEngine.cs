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
        private readonly Assembly _assembly;
        private readonly IMongoClient _externalClient;
        private readonly TransactionScope _transactionScope;

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
            _assembly = assembly;
            _externalClient = client;
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
                    var migrationRunner = new MigrationRunner(client, _databaseName, _assembly, _transactionScope);
                    await migrationRunner.RunAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                var migrationRunner = new MigrationRunner(_externalClient, _databaseName, _assembly, _transactionScope);
                await migrationRunner.RunAsync(cancellationToken).ConfigureAwait(false);
            }           
        }
    }
}
