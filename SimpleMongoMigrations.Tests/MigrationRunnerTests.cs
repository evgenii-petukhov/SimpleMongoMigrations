﻿using FluentAssertions;
using MongoDB.Driver;
using Moq;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Models;

namespace SimpleMongoMigrations.Tests
{
    [TestFixture]
    public class MigrationRunnerTests
    {
        private Mock<IMongoClient> _clientMock;
        private Mock<IMongoDatabase> _databaseMock;
        private Mock<IMigrationRepository> _migrationRepositoryMock;
        private Mock<IMigrationScanner> _migrationScannerMock;
        private Mock<ITransactionSupportChecker> _transactionSupportCheckerMock;
        private MigrationRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _clientMock = new Mock<IMongoClient>();
            _databaseMock = new Mock<IMongoDatabase>();
            _migrationRepositoryMock = new Mock<IMigrationRepository>();
            _migrationScannerMock = new Mock<IMigrationScanner>();
            _transactionSupportCheckerMock = new Mock<ITransactionSupportChecker>();

            _runner = new MigrationRunner(
                _clientMock.Object,
                _databaseMock.Object,
                _migrationRepositoryMock.Object,
                _migrationScannerMock.Object,
                _transactionSupportCheckerMock.Object
            );
        }

        [Test]
        public async Task RunAsync_Should_ApplyTransactionalMigrations_When_SingleTransactionAndTransactionSupported()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _migrationRepositoryMock
                .Setup(x => x.GetMostRecentAppliedMigrationAsync(cancellationToken))
                .ReturnsAsync((Migration)null!);

            _transactionSupportCheckerMock
                .Setup(x => x.IsTransactionSupportedAsync(cancellationToken))
                .ReturnsAsync(true);

            var sessionMock = new Mock<IClientSessionHandle>();
            _clientMock
                .Setup(x => x.StartSessionAsync(null, cancellationToken))
                .ReturnsAsync(sessionMock.Object);

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([typeof(TransactionalMigration1), typeof(TransactionalMigration2)]);

            // Act
            await _runner.RunAsync(TransactionScope.SingleTransaction, cancellationToken);

            // Assert
            sessionMock.Verify(
                x => x.StartTransaction(It.IsAny<TransactionOptions>()),
                Times.Once);
            sessionMock.Verify(
                x => x.CommitTransactionAsync(cancellationToken),
                Times.Once);
            sessionMock.Verify(
                x => x.AbortTransactionAsync(cancellationToken),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(sessionMock.Object, 1, nameof(TransactionalMigration1), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(sessionMock.Object, 2, nameof(TransactionalMigration2), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
            _transactionSupportCheckerMock.Verify(
                x => x.IsTransactionSupportedAsync(cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task RunAsync_Should_ApplyTransactionalMigrations_When_TransactionPerMigrationAndTransactionSupported()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _migrationRepositoryMock
                .Setup(x => x.GetMostRecentAppliedMigrationAsync(cancellationToken))
                .ReturnsAsync((Migration)null!);

            _transactionSupportCheckerMock
                .Setup(x => x.IsTransactionSupportedAsync(cancellationToken))
                .ReturnsAsync(true);

            var sessionMock = new Mock<IClientSessionHandle>();
            _clientMock
                .Setup(x => x.StartSessionAsync(null, cancellationToken))
                .ReturnsAsync(sessionMock.Object);

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([typeof(TransactionalMigration1), typeof(TransactionalMigration2)]);

            // Act
            await _runner.RunAsync(TransactionScope.PerMigration, cancellationToken);

            // Assert
            sessionMock.Verify(
                x => x.StartTransaction(It.IsAny<TransactionOptions>()),
                Times.Exactly(2));
            sessionMock.Verify(
                x => x.CommitTransactionAsync(cancellationToken),
                Times.Exactly(2));
            sessionMock.Verify(
                x => x.AbortTransactionAsync(cancellationToken),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(sessionMock.Object, 1, nameof(TransactionalMigration1), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(sessionMock.Object, 2, nameof(TransactionalMigration2), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
            _transactionSupportCheckerMock.Verify(
                x => x.IsTransactionSupportedAsync(cancellationToken),
                Times.Once);
        }

        [Test]
        [TestCase(TransactionScope.SingleTransaction)]
        [TestCase(TransactionScope.PerMigration)]
        [TestCase(TransactionScope.NoTransaction)]
        public async Task RunAsync_Should_ApplyTransactionalMigrations_When_TransactionNotSupported(TransactionScope transactionScope)
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _migrationRepositoryMock
                .Setup(x => x.GetMostRecentAppliedMigrationAsync(cancellationToken))
                .ReturnsAsync((Migration)null!);

            _transactionSupportCheckerMock
                .Setup(x => x.IsTransactionSupportedAsync(cancellationToken))
                .ReturnsAsync(false);

            var sessionMock = new Mock<IClientSessionHandle>();
            _clientMock
                .Setup(x => x.StartSessionAsync(null, cancellationToken))
                .ReturnsAsync(sessionMock.Object);

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([typeof(TransactionalMigration1), typeof(TransactionalMigration2)]);

            // Act
            await _runner.RunAsync(transactionScope, cancellationToken);

            // Assert
            sessionMock.Verify(
                x => x.StartTransaction(It.IsAny<TransactionOptions>()),
                Times.Never);
            sessionMock.Verify(
                x => x.CommitTransactionAsync(cancellationToken),
                Times.Never);
            sessionMock.Verify(
                x => x.AbortTransactionAsync(cancellationToken),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(1, nameof(TransactionalMigration1), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(2, nameof(TransactionalMigration2), cancellationToken),
                Times.Once);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
            _transactionSupportCheckerMock.Verify(
                x => x.IsTransactionSupportedAsync(cancellationToken),
                transactionScope == TransactionScope.NoTransaction ? Times.Never : Times.Once);
        }

        [Test]
        public async Task RunAsync_Should_ApplyMigrations_When_SingleTransactionAndTransactionSupported()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _migrationRepositoryMock
                .Setup(x => x.GetMostRecentAppliedMigrationAsync(cancellationToken))
                .ReturnsAsync((Migration)null!);

            _transactionSupportCheckerMock
                .Setup(x => x.IsTransactionSupportedAsync(cancellationToken))
                .ReturnsAsync(true);

            var sessionMock = new Mock<IClientSessionHandle>();
            _clientMock
                .Setup(x => x.StartSessionAsync(null, cancellationToken))
                .ReturnsAsync(sessionMock.Object);

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([typeof(Migration1), typeof(Migration2)]);

            // Act
            await _runner.RunAsync(TransactionScope.SingleTransaction, cancellationToken);

            // Assert
            sessionMock.Verify(
                x => x.StartTransaction(It.IsAny<TransactionOptions>()),
                Times.Once);
            sessionMock.Verify(
                x => x.CommitTransactionAsync(cancellationToken),
                Times.Once);
            sessionMock.Verify(
                x => x.AbortTransactionAsync(cancellationToken),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(1, nameof(Migration1), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(2, nameof(Migration2), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
            _transactionSupportCheckerMock.Verify(
                x => x.IsTransactionSupportedAsync(cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task RunAsync_Should_ApplyMigrations_When_TransactionPerMigrationAndTransactionSupported()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _migrationRepositoryMock
                .Setup(x => x.GetMostRecentAppliedMigrationAsync(cancellationToken))
                .ReturnsAsync((Migration)null!);

            _transactionSupportCheckerMock
                .Setup(x => x.IsTransactionSupportedAsync(cancellationToken))
                .ReturnsAsync(true);

            var sessionMock = new Mock<IClientSessionHandle>();
            _clientMock
                .Setup(x => x.StartSessionAsync(null, cancellationToken))
                .ReturnsAsync(sessionMock.Object);

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([typeof(Migration1), typeof(Migration2)]);

            // Act
            await _runner.RunAsync(TransactionScope.PerMigration, cancellationToken);

            // Assert
            sessionMock.Verify(
                x => x.StartTransaction(It.IsAny<TransactionOptions>()),
                Times.Exactly(2));
            sessionMock.Verify(
                x => x.CommitTransactionAsync(cancellationToken),
                Times.Exactly(2));
            sessionMock.Verify(
                x => x.AbortTransactionAsync(cancellationToken),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(1, nameof(Migration1), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(2, nameof(Migration2), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
            _transactionSupportCheckerMock.Verify(
                x => x.IsTransactionSupportedAsync(cancellationToken),
                Times.Once);
        }

        [Test]
        [TestCase(TransactionScope.SingleTransaction)]
        [TestCase(TransactionScope.PerMigration)]
        [TestCase(TransactionScope.NoTransaction)]
        public async Task RunAsync_Should_ApplyMigrations_When_TransactionNotSupported(TransactionScope transactionScope)
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _migrationRepositoryMock
                .Setup(x => x.GetMostRecentAppliedMigrationAsync(cancellationToken))
                .ReturnsAsync((Migration)null!);

            _transactionSupportCheckerMock
                .Setup(x => x.IsTransactionSupportedAsync(cancellationToken))
                .ReturnsAsync(false);

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([typeof(Migration1), typeof(Migration2)]);

            // Act
            await _runner.RunAsync(transactionScope, cancellationToken);

            // Assert
            _clientMock
                .Verify(x => x.StartSessionAsync(It.IsAny<ClientSessionOptions>(), It.IsAny<CancellationToken>()), Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(1, nameof(Migration1), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(2, nameof(Migration2), cancellationToken),
                Times.Once);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
            _transactionSupportCheckerMock.Verify(
                x => x.IsTransactionSupportedAsync(cancellationToken),
                transactionScope == TransactionScope.NoTransaction ? Times.Never : Times.Once);
        }

        [Test]
        public async Task RunAsync_Should_NotApplyMigrations_If_AlreadyUpToDate()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _migrationRepositoryMock
                .Setup(x => x.GetMostRecentAppliedMigrationAsync(cancellationToken))
                .ReturnsAsync(new Migration
                {
                    Version = new Version(2)
                });

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([typeof(Migration1), typeof(Migration2)]);

            // Act
            await _runner.RunAsync(TransactionScope.NoTransaction, cancellationToken);

            // Assert
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
        }

        [Test]
        public async Task RunAsync_Should_NotApplyMigrations_If_ThereAreNoMigrations()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([]);

            // Act
            await _runner.RunAsync(TransactionScope.NoTransaction, cancellationToken);

            // Assert
            _transactionSupportCheckerMock.Verify(
                x => x.IsTransactionSupportedAsync(cancellationToken),
                Times.Never);
            _clientMock.Verify(
                x => x.StartSessionAsync(It.IsAny<ClientSessionOptions>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
        }

        [Test]
        public async Task RunAsync_Should_RollbackTransaction_When_ExceptionThrown()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _migrationRepositoryMock
                .Setup(x => x.GetMostRecentAppliedMigrationAsync(cancellationToken))
                .ReturnsAsync((Migration)null!);

            _transactionSupportCheckerMock
                .Setup(x => x.IsTransactionSupportedAsync(cancellationToken))
                .ReturnsAsync(true);

            var sessionMock = new Mock<IClientSessionHandle>();
            _clientMock
                .Setup(x => x.StartSessionAsync(null, cancellationToken))
                .ReturnsAsync(sessionMock.Object);

            _migrationScannerMock
                .Setup(x => x.Migrations)
                .Returns([typeof(TransactionalMigration1), typeof(TransactionalMigration2)]);

            _migrationRepositoryMock
                .Setup(x => x.SaveMigrationAsync(sessionMock.Object, 2, nameof(TransactionalMigration2), cancellationToken))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            Func<Task> action = () => _runner.RunAsync(TransactionScope.SingleTransaction, cancellationToken);

            await action.Should().ThrowAsync<Exception>().WithMessage("Test exception");

            // Assert
            sessionMock.Verify(
                x => x.StartTransaction(It.IsAny<TransactionOptions>()),
                Times.Once);
            sessionMock.Verify(
                x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Never);
            sessionMock.Verify(
                x => x.AbortTransactionAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.GetMostRecentAppliedMigrationAsync(cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(sessionMock.Object, 1, nameof(TransactionalMigration1), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(sessionMock.Object, 2, nameof(TransactionalMigration2), cancellationToken),
                Times.Once);
            _migrationRepositoryMock.Verify(
                x => x.SaveMigrationAsync(It.IsAny<Version>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _migrationScannerMock.Verify(
                x => x.Migrations,
                Times.Once);
            _transactionSupportCheckerMock.Verify(
                x => x.IsTransactionSupportedAsync(cancellationToken),
                Times.Once);
        }

        [Version(1)]
        [Name(nameof(Migration1))]
        private class Migration1 : IMigration
        {
            public Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        [Version(2)]
        [Name(nameof(Migration2))]
        private class Migration2 : IMigration
        {
            public Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        [Version(1)]
        [Name(nameof(TransactionalMigration1))]
        private class TransactionalMigration1 : ITransactionalMigration
        {
            public Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken) => Task.CompletedTask;
            public Task UpAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        [Version(2)]
        [Name(nameof(TransactionalMigration2))]
        private class TransactionalMigration2 : ITransactionalMigration
        {
            public Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken) => Task.CompletedTask;
            public Task UpAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken) => Task.CompletedTask;
        }
    }
}
