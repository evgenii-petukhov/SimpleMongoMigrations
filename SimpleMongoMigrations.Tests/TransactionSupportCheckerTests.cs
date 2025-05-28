using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace SimpleMongoMigrations.Tests
{
    [TestFixture]
    public class TransactionSupportCheckerTests
    {
        private Mock<IMongoClient> _clientMock;
        private Mock<IMongoDatabase> _dbMock;

        [SetUp]
        public void SetUp()
        {
            _clientMock = new Mock<IMongoClient>();
            _dbMock = new Mock<IMongoDatabase>();
        }

        [Test]
        public async Task IsTransactionSupportedAsync_ReturnsTrue_WhenReplicaSet()
        {
            // Arrange
            var resultDoc = new BsonDocument { { "setName", "rs0" } };
            _clientMock
                .Setup(x => x.GetDatabase("admin", null))
                .Returns(_dbMock.Object);
            _dbMock
                .Setup(x => x.RunCommandAsync(It.IsAny<Command<BsonDocument>>(), null, default))
                .ReturnsAsync(resultDoc);

            var checker = new TransactionSupportChecker(_clientMock.Object);

            // Act
            var supported = await checker.IsTransactionSupportedAsync(CancellationToken.None);

            // Assert
            supported.Should().BeTrue();
        }

        [Test]
        public async Task IsTransactionSupportedAsync_ReturnsTrue_WhenMongos()
        {
            // Arrange
            var resultDoc = new BsonDocument { { "msg", "isdbgrid" } };
            _clientMock
                .Setup(x => x.GetDatabase("admin", null))
                .Returns(_dbMock.Object);
            _dbMock
                .Setup(x => x.RunCommandAsync(It.IsAny<Command<BsonDocument>>(), null, default))
                .ReturnsAsync(resultDoc);

            var checker = new TransactionSupportChecker(_clientMock.Object);

            // Act
            var supported = await checker.IsTransactionSupportedAsync(CancellationToken.None);

            // Assert
            supported.Should().BeTrue();
        }

        [Test]
        public async Task IsTransactionSupportedAsync_ReturnsFalse_WhenNeitherReplicaSetNorMongos()
        {
            // Arrange
            var resultDoc = new BsonDocument { { "ok", 1 } };
            _clientMock
                .Setup(x => x.GetDatabase("admin", null))
                .Returns(_dbMock.Object);
            _dbMock
                .Setup(x => x.RunCommandAsync(It.IsAny<Command<BsonDocument>>(), null, default))
                .ReturnsAsync(resultDoc);

            var checker = new TransactionSupportChecker(_clientMock.Object);

            // Act
            var supported = await checker.IsTransactionSupportedAsync(CancellationToken.None);

            // Assert
            supported.Should().BeFalse();
        }

        [Test]
        public async Task IsTransactionSupportedAsync_ReturnsTrue_WhenBothReplicaSetAndMongos()
        {
            // Arrange
            var resultDoc = new BsonDocument { { "setName", "rs0" }, { "msg", "isdbgrid" } };
            _clientMock
                .Setup(x => x.GetDatabase("admin", null))
                .Returns(_dbMock.Object);
            _dbMock
                .Setup(x => x.RunCommandAsync(It.IsAny<Command<BsonDocument>>(), null, default))
                .ReturnsAsync(resultDoc);

            var checker = new TransactionSupportChecker(_clientMock.Object);

            // Act
            var supported = await checker.IsTransactionSupportedAsync(CancellationToken.None);

            // Assert
            supported.Should().BeTrue();
        }

        [Test]
        public void IsTransactionSupportedAsync_Throws_WhenRunCommandFails()
        {
            // Arrange
            _clientMock
                .Setup(x => x.GetDatabase("admin", null))
                .Returns(_dbMock.Object);
            _dbMock
                .Setup(x => x.RunCommandAsync(It.IsAny<Command<BsonDocument>>(), null, default))
                .ThrowsAsync(new MongoException("fail"));

            var checker = new TransactionSupportChecker(_clientMock.Object);

            // Act
            var act = () => checker.IsTransactionSupportedAsync(CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<MongoException>();
        }
    }
}
