using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var latestMigration = await _migrationRepository.GetMostRecentAppliedMigrationAsync(cancellationToken)
                .ConfigureAwait(false);
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

            var transactionSupported = await _transactionSupportChecker.IsTransactionSupportedAsync(cancellationToken)
                .ConfigureAwait(false);

            if (transactionSupported)
            {
                switch (_transactionScope)
                {
                    case TransactionScope.SingleTransaction:
                        await ApplyMigrationsInSingleTransactionAsync(migrationsToRun, cancellationToken)
                            .ConfigureAwait(false);
                        break;
                    case TransactionScope.PerMigration:
                        await ApplyMigrationsInSeparateTransactionsAsync(migrationsToRun, cancellationToken)
                            .ConfigureAwait(false);
                        break;
                    case TransactionScope.NoTransaction:
                    default:
                        await ApplyMigrationsWithoutTransactionAsync(migrationsToRun, cancellationToken)
                            .ConfigureAwait(false);
                        break;
                }
            }
            else
            {
                await ApplyMigrationsWithoutTransactionAsync(migrationsToRun, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private async Task ApplyMigrationsInSingleTransactionAsync(
            IEnumerable<Type> migrationsToRun,
            CancellationToken cancellationToken)
        {
            using (var session = await _client.StartSessionAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            {
                session.StartTransaction();
                try
                {
                    foreach (var migrationType in migrationsToRun)
                    {
                        await ApplyMigrationAsync(session, migrationType, cancellationToken)
                            .ConfigureAwait(false);
                    }
                    await session.CommitTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch
                {
                    await session.AbortTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);
                    throw;
                }
            }
        }

        private async Task ApplyMigrationsInSeparateTransactionsAsync(
            IEnumerable<Type> migrationsToRun,
            CancellationToken cancellationToken)
        {
            foreach (var migrationType in migrationsToRun)
            {
                using (var session = await _client.StartSessionAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false))
                {
                    session.StartTransaction();
                    try
                    {
                        await ApplyMigrationAsync(session, migrationType, cancellationToken)
                            .ConfigureAwait(false);
                        await session.CommitTransactionAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        await session.AbortTransactionAsync(cancellationToken)
                            .ConfigureAwait(false);
                        throw;
                    }
                }
            }
        }

        private async Task ApplyMigrationsWithoutTransactionAsync(
            IEnumerable<Type> migrationsToRun,
            CancellationToken cancellationToken)
        {
            foreach (var migrationType in migrationsToRun)
            {
                await ApplyMigrationAsync(null, migrationType, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private async Task ApplyMigrationAsync(
            IClientSessionHandle session,
            Type migrationType,
            CancellationToken cancellationToken)
        {
            var migration = (IMigration)Activator.CreateInstance(migrationType);
            if (session != null && migration is ITransactionalMigration transactionalMigration)
            {
                await transactionalMigration.UpAsync(_database, session, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await migration.UpAsync(_database, cancellationToken)
                    .ConfigureAwait(false);
            }
            await _migrationRepository.SaveMigrationAsync(
                session,
                migrationType.GetCustomAttribute<VersionAttribute>().Version,
                migrationType.GetCustomAttribute<NameAttribute>()?.Name,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
