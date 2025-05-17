using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.ConsoleAppNet6.Models;

namespace SimpleMongoMigrations.Tests.Migrations
{
    [Version("0.0.3")]
    public class _003_AddIndexByName : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.GetCollection<City>(nameof(City)).Indexes.CreateOne(new CreateIndexModel<City>(
                Builders<City>.IndexKeys.Ascending(x => x.Name),
                new CreateIndexOptions
                {
                    Unique = true
                }));
        }
    }
}
