﻿using FluentAssertions;
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
            _migrationEngine = MigrationEngineBuilder
                .Create()
                .WithDatabase(TestDbName)
                .WithAssembly(typeof(object).Assembly)
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
        public async Task RunAsync_ShouldNotThrow_WhenNoMigrations()
        {
            // Arrange

            // Act
            await _migrationEngine.RunAsync(default);

            // Assert
            _migrationCollection.CountDocuments(Builders<Migration>.Filter.Empty).Should().Be(0);
        }
    }
}
