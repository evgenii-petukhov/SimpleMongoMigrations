using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("4")]
    [Name("Adds a unique index by name")]
    public class _4_0_0_AddIndexByName : ITransactionalMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.GetCollection<City>(nameof(City)).Indexes.CreateOne(
                new CreateIndexModel<City>(
                    Builders<City>.IndexKeys.Ascending(x => x.Name),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }));
        }

        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            database.GetCollection<City>(nameof(City)).Indexes.CreateOne(
                session,
                new CreateIndexModel<City>(
                    Builders<City>.IndexKeys.Ascending(x => x.Name),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }));
        }
    }
}
