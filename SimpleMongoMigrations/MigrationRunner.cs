using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleMongoMigrations
{
    public class MigrationRunner
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly TransactionScope _transactionScope;
        private readonly MigrationRepository _migrationRepository;
        private readonly MigrationScanner _migrationScanner;
        private readonly TransactionSupportChecker _transactionSupportChecker;

        public MigrationRunner(
            IMongoClient client,
            string databaseName,
            Assembly assembly,
            TransactionScope transactionScope)
        {
            _client = client;
            _transactionScope = transactionScope;
            _database = _client.GetDatabase(databaseName);
            _migrationRepository = new MigrationRepository(_database);
            _migrationScanner = new MigrationScanner(assembly);
            _transactionSupportChecker = new TransactionSupportChecker(_client);
        }

        public void Run()
        {
            var latestMigration = _migrationRepository.GetMostRecentAppliedMigration();
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

            if (_transactionSupportChecker.IsTransactionSupported)
            {
                switch (_transactionScope)
                {
                    case TransactionScope.AllMigrations:
                        ApplyMigrationsInSingleTransaction(migrationsToRun);
                        break;
                    case TransactionScope.SingleMigration:
                        ApplyMigrationsInSeparateTransactions(migrationsToRun);
                        break;
                    case TransactionScope.None:
                    default:
                        ApplyMigrationsWithoutTransaction(migrationsToRun);
                        break;
                }
            }
            else
            {
                ApplyMigrationsWithoutTransaction(migrationsToRun);
            }
        }

        private void ApplyMigrationsInSingleTransaction(IEnumerable<Type> migrationsToRun)
        {
            using (var session = _client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    foreach (var migrationType in migrationsToRun)
                    {
                        ApplyMigration(session, migrationType);
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

        private void ApplyMigrationsInSeparateTransactions(IEnumerable<Type> migrationsToRun)
        {
            foreach (var migrationType in migrationsToRun)
            {
                using (var session = _client.StartSession())
                {
                    session.StartTransaction();
                    try
                    {
                        ApplyMigration(session, migrationType);
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

        private void ApplyMigrationsWithoutTransaction(IEnumerable<Type> migrationsToRun)
        {
            foreach (var migrationType in migrationsToRun)
            {
                ApplyMigration(null, migrationType);
            }
        }

        private void ApplyMigration(IClientSessionHandle session, Type migrationType)
        {
            var migration = (IMigration)Activator.CreateInstance(migrationType);
            migration.Up(_database, session);
            _migrationRepository.SaveMigration(
                session,
                migrationType.GetCustomAttribute<VersionAttribute>().Version,
                migrationType.GetCustomAttribute<NameAttribute>()?.Name);
        }
    }
}
