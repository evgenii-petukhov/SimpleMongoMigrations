using FluentAssertions;
using Mongo2Go;
using MongoDB.Driver;
using SimpleMongoMigrations.Models;

namespace SimpleMongoMigrations.Tests.NoMigrations
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
            _migrationEngine = new MigrationEngine(_client, TestDbName, typeof(object).Assembly);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _runner.Dispose();
        }

        [Test]
        public void Run_ShouldNotThrow_WhenNoMigrations()
        {
            // Arrange

            // Act
            _migrationEngine.Run();

            // Assert
            _migrationCollection.CountDocuments(Builders<Migration>.Filter.Empty).Should().Be(0);
        }
    }
}
