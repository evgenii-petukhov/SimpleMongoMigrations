using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;
using System.Collections.Generic;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("1.0.0")]
    [Name("Populates the City collection with sample data")]
    public class _1_0_0_AddDefaultData : IMigration
    {
        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            database.GetCollection<City>(nameof(City)).InsertMany(
                //session, // Uncomment this line if you want to use a session for the insert operation
                new List<City>
                {
                    new City
                    {
                        Name = "London",
                        Country = "England"
                    },
                    new City
                    {
                        Name = "Milan",
                        Country = "Italy"
                    },
                    new City
                    {
                        Name = "Barcelona",
                        Country = "Spain"
                    },
                    new City
                    {
                        Name = "Berlin",
                        Country = "Germany"
                    },
                    new City
                    {
                        Name = "Paris",
                        Country = "France"
                    }
                });
        }
    }
}
