using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;

namespace SimpleMongoMigrations
{
    public class MigrationEngine
    {
        private readonly string _databaseName;
        private readonly string _connectionString;
        private readonly IMongoClient _externalClient;
        private readonly MigrationScanner _migrationScanner;

        static MigrationEngine()
        {
            BsonSerializer.TryRegisterSerializer(typeof(Version), new VerstionSerializer());
        }

        [Obsolete("Direct instantiation is deprecated. Please use MigrationEngineBuilder for configuring and creating MigrationEngine instances.")]
        public MigrationEngine(
            string connectionString,
            string databaseName,
            Assembly assembly) : this(databaseName, assembly)
        {
            _connectionString = connectionString;
        }

        public MigrationEngine(
            IMongoClient client,
            string databaseName,
            Assembly assembly) : this(databaseName, assembly)
        {
            _externalClient = client;
        }

        private MigrationEngine(
            string databaseName,
            Assembly assembly)
        {
            _databaseName = databaseName;
            _migrationScanner = new MigrationScanner(assembly);
        }

        public void Run()
        {
            if (_externalClient == null)
            {
                using (var client = new MongoClient(_connectionString))
                {
                    RunInternal(client);
                }
            }
            else
            {
                RunInternal(_externalClient);
            }
        }

        private void RunInternal(IMongoClient client)
        {
            var database = client.GetDatabase(_databaseName);
            var migrationRepository = new MigrationRepository(database);
            var latestMigration = migrationRepository.GetMostRecentAppliedMigration();
            var latestVersion = latestMigration?.Version ?? Version.Zero;

            var migrationsToRun = _migrationScanner.Migrations
                .Select(migration => new
                {
                    Type = migration,
                    migration.GetCustomAttribute<VersionAttribute>().Version
                })
                .Where(g => g.Version > latestVersion)
                .ToList();

            foreach (var migrationType in migrationsToRun.OrderBy(g => g.Version).Select(g => g.Type))
            {
                var migration = (IMigration)Activator.CreateInstance(migrationType);
                migration.Up(database);
                migrationRepository.SaveMigration(
                    migrationType.GetCustomAttribute<VersionAttribute>().Version,
                    migrationType.GetCustomAttribute<NameAttribute>()?.Name);
            }
        }
    }
}
