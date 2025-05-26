using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version(5)]
    [Name("Adds coodrinates for each entry of the City collection")]
    public class _5_0_0_AddCoordinates : ITransactionalMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "London"),
                Builders<City>.Update.Set(x => x.Latitude, 51.507222m).Set(x => x.Longitude, -0.1275m));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "Milan"),
                Builders<City>.Update.Set(x => x.Latitude, 45.466944m).Set(x => x.Longitude, 9.19m));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "Barcelona"),
                Builders<City>.Update.Set(x => x.Latitude, 41.383333m).Set(x => x.Longitude, 2.183333m));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "Berlin"),
                Builders<City>.Update.Set(x => x.Latitude, 52.52m).Set(x => x.Longitude, 13.405m));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "Paris"),
                Builders<City>.Update.Set(x => x.Latitude, 48.856667m).Set(x => x.Longitude, 2.352222m));
        }

        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "London"),
                Builders<City>.Update.Set(x => x.Latitude, 51.507222m).Set(x => x.Longitude, -0.1275m));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Milan"),
                Builders<City>.Update.Set(x => x.Latitude, 45.466944m).Set(x => x.Longitude, 9.19m));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Barcelona"),
                Builders<City>.Update.Set(x => x.Latitude, 41.383333m).Set(x => x.Longitude, 2.183333m));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Berlin"),
                Builders<City>.Update.Set(x => x.Latitude, 52.52m).Set(x => x.Longitude, 13.405m));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Paris"),
                Builders<City>.Update.Set(x => x.Latitude, 48.856667m).Set(x => x.Longitude, 2.352222m));
        }
    }
}
