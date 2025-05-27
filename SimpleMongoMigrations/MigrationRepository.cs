using MongoDB.Driver;
using SimpleMongoMigrations.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations
{
    internal class MigrationRepository
    {
        private readonly IMongoCollection<Migration> _migrationCollection;

        public MigrationRepository(IMongoDatabase database)
        {
            _migrationCollection = database.GetCollection<Migration>(MigrationConstants.MigrationCollectionName);
        }

        public Task<Migration> GetMostRecentAppliedMigrationAsync(CancellationToken cancellationToken)
        {
            return _migrationCollection
                .Find(Builders<Migration>.Filter.Eq(x => x.IsUp, true))
                .Sort(Builders<Migration>.Sort.Descending(x => x.Version))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task SaveMigrationAsync(
            IClientSessionHandle session,
            Version version, string name,
            CancellationToken cancellationToken)
        {
            return session == null
                ? SaveMigrationInternalAsync(version, name, cancellationToken)
                : SaveMigrationInternalAsync(session, version, name, cancellationToken);
        }

        private Task SaveMigrationInternalAsync(
            IClientSessionHandle session,
            Version version, string name,
            CancellationToken cancellationToken)
        {
            return _migrationCollection.InsertOneAsync(
                session,
                new Migration
                {
                    Name = name,
                    Version = version,
                    IsUp = true,
                    TimeStamp = DateTime.UtcNow
                }, cancellationToken: cancellationToken);
        }

        private Task SaveMigrationInternalAsync(
            Version version,
            string name,
            CancellationToken cancellationToken)
        {
            return _migrationCollection.InsertOneAsync(
                new Migration
                {
                    Name = name,
                    Version = version,
                    IsUp = true,
                    TimeStamp = DateTime.UtcNow
                }, cancellationToken: cancellationToken);
        }
    }
}
