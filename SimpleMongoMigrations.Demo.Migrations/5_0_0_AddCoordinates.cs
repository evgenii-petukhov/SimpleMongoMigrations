using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version(5)]
    [Name("Adds coodrinates for each entry of the City collection")]
    public class _5_0_0_AddCoordinates : ITransactionalMigration
    {
        public async Task UpAsync(
            IMongoDatabase database,
            CancellationToken cancellationToken)
        {
            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "London"),
                Builders<City>.Update.Set(x => x.Latitude, 51.507222m).Set(x => x.Longitude, -0.1275m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "Milan"),
                Builders<City>.Update.Set(x => x.Latitude, 45.466944m).Set(x => x.Longitude, 9.19m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "Barcelona"),
                Builders<City>.Update.Set(x => x.Latitude, 41.383333m).Set(x => x.Longitude, 2.183333m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "Berlin"),
                Builders<City>.Update.Set(x => x.Latitude, 52.52m).Set(x => x.Longitude, 13.405m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "Paris"),
                Builders<City>.Update.Set(x => x.Latitude, 48.856667m).Set(x => x.Longitude, 2.352222m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task UpAsync(
            IMongoDatabase database,
            IClientSessionHandle session,
            CancellationToken cancellationToken)
        {
            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "London"),
                Builders<City>.Update.Set(x => x.Latitude, 51.507222m).Set(x => x.Longitude, -0.1275m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Milan"),
                Builders<City>.Update.Set(x => x.Latitude, 45.466944m).Set(x => x.Longitude, 9.19m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Barcelona"),
                Builders<City>.Update.Set(x => x.Latitude, 41.383333m).Set(x => x.Longitude, 2.183333m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Berlin"),
                Builders<City>.Update.Set(x => x.Latitude, 52.52m).Set(x => x.Longitude, 13.405m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Paris"),
                Builders<City>.Update.Set(x => x.Latitude, 48.856667m).Set(x => x.Longitude, 2.352222m),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
