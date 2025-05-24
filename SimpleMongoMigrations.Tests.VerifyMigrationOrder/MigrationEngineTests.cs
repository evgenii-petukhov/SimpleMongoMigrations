using FluentAssertions;
using Mongo2Go;
using MongoDB.Driver;
using SimpleMongoMigrations.Models;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder
{
    [TestFixture]
    public class MigrationEngineTests
    {
        private const string TestDbName = "TestDB";

        private MongoDbRunner _runner;
        private MongoClient _client;
        private IMongoCollection<Migration> _migrationCollection;
        private IMongoDatabase _database;
        private MigrationEngine _migrationEngine;

        [SetUp]
        public void SetUp()
        {
            _runner = MongoDbRunner.Start();
            _client = new MongoClient(_runner.ConnectionString);
            _database = _client.GetDatabase(TestDbName);
            _migrationCollection = _database.GetCollection<Migration>(MigrationConstants.MigrationCollectionName);
            _migrationEngine = new MigrationEngine(_client, TestDbName, typeof(Person).Assembly);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _runner.Dispose();
        }

        [Test]
        public void Run_AppliesMigrationsInCorrectOrderAndIgnoresUnversioned()
        {
            // Arrange

            // Act
            _migrationEngine.Run();

            // Assert
            _migrationCollection
                .CountDocuments(Builders<Migration>.Filter.Empty)
                .Should()
                .Be(6);

            _migrationCollection
                .Find(Builders<Migration>.Filter.Empty)
                .ToList()
                .Should()
                .BeEquivalentTo(new List<Migration>
                {
                    new() {
                        Name = "Adds default data",
                        Version = new Version("1.0.0"),
                        IsUp = true,
                    },
                    new() {
                        Name = "Adds age",
                        Version = new Version("2.0.0"),
                        IsUp = true,
                    },
                    new() {
                        Name = null,
                        Version = new Version("3.0.0"),
                        IsUp = true,
                    },
                    new() {
                        Name = "Adds preferences",
                        Version = new Version("6.0.0"),
                        IsUp = true,
                    },
                    new() {
                        Name = "Adds another person",
                        Version = new Version("7.0.0"),
                        IsUp = true,
                    },
                    new() {
                        Name = "Sets status",
                        Version = new Version("8.0.0"),
                        IsUp = true,
                    },
                }, options => options.Excluding(x => x.Id).Excluding(x => x.TimeStamp).ComparingByMembers<Person>().WithStrictOrdering());

            var personsCollection = _database.GetCollection<Person>(nameof(Person));
            personsCollection
                .Find(Builders<Person>.Filter.Empty)
                .ToList()
                .Select(p => p.Data)
                .Should()
                .BeEquivalentTo(
                [
                    "Olivia, 28 y.o., from Paris, loves cars, married",
                    "Lucas, married"
                ]);
        }

        [Test]
        public void Run_AppliesOnlyMissingVersionedMigrations_WhenSomeAlreadyApplied()
        {
            var migrationEngine = new MigrationEngine(_client, TestDbName, typeof(Person).Assembly);
            // Arrange
            _migrationCollection
                .InsertMany(
                [
                    new() {
                        Name = "Adds default data",
                        Version = new Version("1.0.0"),
                        IsUp = true,
                    },
                    new() {
                        Name = "Adds age",
                        Version = new Version("2.0.0"),
                        IsUp = true,
                    },
                ]);

            var a = _migrationCollection
                .Find(Builders<Migration>.Filter.Empty)
                .ToList();

            var personsCollection = _database.GetCollection<Person>(nameof(Person));
            personsCollection
                .InsertOne(new Person
                {
                    Data = "Olivia, 28 y.o., "
                });

            // Act

            migrationEngine.Run();

            // Assert
            _migrationCollection
                .CountDocuments(Builders<Migration>.Filter.Empty)
                .Should()
                .Be(6);

            personsCollection
                .Find(Builders<Person>.Filter.Empty)
                .ToList()
                .Select(p => p.Data)
                .Should()
                .BeEquivalentTo(
                [
                    "Olivia, 28 y.o., from Paris, loves cars, married",
                    "Lucas, married"
                ]);
        }
    }
}
