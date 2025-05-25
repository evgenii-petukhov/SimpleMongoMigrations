using MongoDB.Driver;
using SimpleMongoMigrations.Models;
using System;

namespace SimpleMongoMigrations
{
    public class MigrationRepository
    {
        private readonly IMongoCollection<Migration> _migrationCollection;

        public MigrationRepository(IMongoDatabase database)
        {
            _migrationCollection = database.GetCollection<Migration>(MigrationConstants.MigrationCollectionName);
        }

        public Migration GetMostRecentAppliedMigration()
        {
            return _migrationCollection
                .Find(Builders<Migration>.Filter.Eq(x => x.IsUp, true))
                .Sort(Builders<Migration>.Sort.Descending(x => x.Version))
                .FirstOrDefault();
        }

        public void SaveMigration(IClientSessionHandle session, Version version, string name)
        {
            if (session == null)
            {
                SaveMigrationInternal(version, name);
            }
            else
            {
                SaveMigrationInternal(session, version, name);
            }
        }

        private void SaveMigrationInternal(IClientSessionHandle session, Version version, string name)
        {
            _migrationCollection.InsertOne(
                session,
                new Migration
                {
                    Name = name,
                    Version = version,
                    IsUp = true,
                    TimeStamp = DateTime.UtcNow
                });
        }

        private void SaveMigrationInternal(Version version, string name)
        {
            _migrationCollection.InsertOne(
                new Migration
                {
                    Name = name,
                    Version = version,
                    IsUp = true,
                    TimeStamp = DateTime.UtcNow
                });
        }
    }
}
