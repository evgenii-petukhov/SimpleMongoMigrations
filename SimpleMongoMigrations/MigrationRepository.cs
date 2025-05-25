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

        public void SaveMigration(Version version, string name)
        {
            _migrationCollection.InsertOne(new Migration
            {
                Name = name,
                Version = version,
                IsUp = true,
                TimeStamp = DateTime.UtcNow
            });
        }
    }
}
