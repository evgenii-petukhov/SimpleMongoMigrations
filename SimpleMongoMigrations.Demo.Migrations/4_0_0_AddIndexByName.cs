using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("4")]
    [Name("Adds a unique index by name")]
    public class _4_0_0_AddIndexByName : ITransactionalMigration
    {
        public Task UpAsync(
            IMongoDatabase database,
            CancellationToken cancellationToken)
        {
            return database.GetCollection<City>(nameof(City)).Indexes.CreateOneAsync(
                new CreateIndexModel<City>(
                    Builders<City>.IndexKeys.Ascending(x => x.Name),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }),
                cancellationToken: cancellationToken);
        }

        public Task UpAsync(
            IMongoDatabase database,
            IClientSessionHandle session,
            CancellationToken cancellationToken)
        {
            return database.GetCollection<City>(nameof(City)).Indexes.CreateOneAsync(
                session,
                new CreateIndexModel<City>(
                    Builders<City>.IndexKeys.Ascending(x => x.Name),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }),
                cancellationToken: cancellationToken);
        }
    }
}
