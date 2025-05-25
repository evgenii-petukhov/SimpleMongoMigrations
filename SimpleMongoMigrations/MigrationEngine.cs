using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        private readonly MigrationScanner _migrationScanner;
        private readonly TransactionScope _transactionScope;

        static MigrationEngine()
        {
            BsonSerializer.TryRegisterSerializer(typeof(Version), new VerstionSerializer());
        }

        internal MigrationEngine(
            string connectionString,
            string databaseName,
            TransactionScope transactionScope,
            Assembly assembly)
            : this(databaseName, assembly, transactionScope)
        {
            _connectionString = connectionString;
        }

        public MigrationEngine(
            IMongoClient client,
            string databaseName,
            Assembly assembly,
            TransactionScope transactionScope = TransactionScope.None)
            : this(databaseName, assembly, transactionScope)
        {
            _externalClient = client;
        }

        private MigrationEngine(
            string databaseName,
            Assembly assembly,
            TransactionScope transactionScope)
        {
            _databaseName = databaseName;
            _transactionScope = transactionScope;
            _migrationScanner = new MigrationScanner(assembly);
        }

        /// <summary>
        /// Runs all pending migrations using the configured transaction scope.
        /// </summary>
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
                .OrderBy(g => g.Version)
                .Select(g => g.Type)
                .ToList();

            var transactionSupportChecker = new TransactionSupportChecker(client);

            if (transactionSupportChecker.IsTransactionSupported)
            {
                switch (_transactionScope)
                {
                    case TransactionScope.AllMigrations:
                        ApplyMigrationsInSingleTransaction(client, database, migrationRepository, migrationsToRun);
                        break;
                    case TransactionScope.SingleMigration:
                        ApplyMigrationsInSeparateTransactions(client, database, migrationRepository, migrationsToRun);
                        break;
                    case TransactionScope.None:
                    default:
                        ApplyMigrationsWithoutTransaction(database, migrationRepository, migrationsToRun);
                        break;
                }
            }
            else
            {
                ApplyMigrationsWithoutTransaction(database, migrationRepository, migrationsToRun);
            }
        }

        private void ApplyMigrationsInSingleTransaction(
            IMongoClient client,
            IMongoDatabase database,
            MigrationRepository migrationRepository,
            IEnumerable<Type> migrationsToRun)
        {
            using (var session = client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    foreach (var migrationType in migrationsToRun)
                    {
                        ApplyMigration(database, session, migrationRepository, migrationType);
                    }
                    session.CommitTransaction();
                }
                catch
                {
                    session.AbortTransaction();
                    throw;
                }
            }
        }

        private void ApplyMigrationsInSeparateTransactions(
            IMongoClient client,
            IMongoDatabase database,
            MigrationRepository migrationRepository,
            IEnumerable<Type> migrationsToRun)
        {
            foreach (var migrationType in migrationsToRun)
            {
                using (var session = client.StartSession())
                {
                    session.StartTransaction();
                    try
                    {
                        ApplyMigration(database, session, migrationRepository, migrationType);
                        session.CommitTransaction();
                    }
                    catch
                    {
                        session.AbortTransaction();
                        throw;
                    }
                }
            }
        }

        private void ApplyMigrationsWithoutTransaction(
            IMongoDatabase database,
            MigrationRepository migrationRepository,
            IEnumerable<Type> migrationsToRun)
        {
            foreach (var migrationType in migrationsToRun)
            {
                ApplyMigration(database, null, migrationRepository, migrationType);
            }
        }

        private void ApplyMigration(
            IMongoDatabase database,
            IClientSessionHandle session,
            MigrationRepository migrationRepository,
            Type migrationType)
        {
            var migration = (IMigration)Activator.CreateInstance(migrationType);
            migration.Up(database, session);
            migrationRepository.SaveMigration(
                session,
                migrationType.GetCustomAttribute<VersionAttribute>().Version,
                migrationType.GetCustomAttribute<NameAttribute>()?.Name);
        }
    }
}
