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

        private readonly List<Migration> _migrations =
        [
            new() {
                Name = "Adds default data",
                Version = new Version("1.0.0"),
                IsUp = true
            },
            new() {
                Name = "Adds age",
                Version = new Version("2.0.0"),
                IsUp = true
            },
            new() {
                Name = null,
                Version = new Version("3.0.0"),
                IsUp = true
            },
            new() {
                Name = "Adds preferences",
                Version = new Version("6.0.0"),
                IsUp = true
            },
            new() {
                Name = "Adds another person",
                Version = new Version("7.0.0"),
                IsUp = true
            },
            new() {
                Name = "Sets status",
                Version = new Version("8.0.0"),
                IsUp = true
            }
        ];

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
            _migrationEngine = MigrationEngineBuilder
                .Create()
                .WithDatabase(TestDbName)
                .WithAssembly(typeof(Person).Assembly)
                .WithClient(_client)
                .Build();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _runner.Dispose();
        }

        [Test]
        public async Task RunAsync_AppliesMigrationsInCorrectOrderAndIgnoresUnversioned()
        {
            // Arrange

            // Act
            await _migrationEngine.RunAsync(default);

            // Assert
            _migrationCollection
                .CountDocuments(Builders<Migration>.Filter.Empty)
                .Should()
                .Be(6);

            _migrationCollection
                .Find(Builders<Migration>.Filter.Empty)
                .ToList()
                .Should()
                .BeEquivalentTo(_migrations, options => options.Excluding(x => x.Id).Excluding(x => x.TimeStamp).ComparingByMembers<Person>().WithStrictOrdering());

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
        public async Task RunAsync_AppliesOnlyMissingVersionedMigrations_WhenSomeAlreadyApplied()
        {
            // Arrange
            _migrationCollection.InsertMany(_migrations.Take(2).Select(m =>
            {
                m.TimeStamp = new DateTime(2020, 1, 1);
                return m;
            }));

            var personsCollection = _database.GetCollection<Person>(nameof(Person));
            personsCollection.InsertOne(new Person
            {
                Data = "Olivia, 28 y.o., "
            });

            // Act

            await _migrationEngine.RunAsync(default);

            // Assert
            _migrationCollection
                .CountDocuments(Builders<Migration>.Filter.Empty)
                .Should()
                .Be(6);

            _migrationCollection
                .Find(Builders<Migration>.Filter.Empty)
                .ToList()
                .Should()
                .BeEquivalentTo(_migrations.Select((m, i) =>
                {
                    if (i < 2)
                    {
                        m.TimeStamp = new DateTime(2020, 1, 1);
                    }
                    
                    return m;
                }), options => options.Excluding(x => x.Id).Excluding(x => x.TimeStamp).ComparingByMembers<Person>().WithStrictOrdering());

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
        public async Task RunAsync_DoesNotReapplyMigrations_WhenAllAreAlreadyApplied()
        {
            // Arrange
            var appliedMigrations = _migrations
                .Select(m =>
                {
                    m.TimeStamp = new DateTime(2020, 1, 1);
                    return m;
                })
                .ToList();

            _migrationCollection.InsertMany(appliedMigrations);

            var a = _migrationCollection
                .Find(Builders<Migration>.Filter.Empty)
                .ToList();

            var personsCollection = _database.GetCollection<Person>(nameof(Person));
            personsCollection
                .InsertMany(
                [
                    new Person
                    {
                        Data = "Olivia, 28 y.o., from Paris, loves cars, married, no change",
                    },
                    new Person
                    {
                        Data = "Lucas, married, no change"
                    }
                ]);

            // Act

            await _migrationEngine.RunAsync(default);

            // Assert
            _migrationCollection
                .CountDocuments(Builders<Migration>.Filter.Empty)
                .Should()
                .Be(6);

            _migrationCollection
                .Find(Builders<Migration>.Filter.Empty)
                .ToList()
                .Should()
                .BeEquivalentTo(appliedMigrations, options => options.Excluding(x => x.Id).Excluding(x => x.TimeStamp).ComparingByMembers<Person>().WithStrictOrdering());

            personsCollection
                .Find(Builders<Person>.Filter.Empty)
                .ToList()
                .Select(p => p.Data)
                .Should()
                .BeEquivalentTo(
                [
                    "Olivia, 28 y.o., from Paris, loves cars, married, no change",
                    "Lucas, married, no change"
                ]);
        }
    }
}
