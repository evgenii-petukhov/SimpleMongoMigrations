using FluentAssertions;
using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using System.Reflection;
using IgnoreAttribute = SimpleMongoMigrations.Attributes.IgnoreAttribute;

namespace SimpleMongoMigrations.Tests
{
    [TestFixture]
    public class MigrationScannerTests
    {
        private Assembly _testAssembly;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testAssembly = typeof(MigrationScannerTests).Assembly;
        }

        [Test]
        public void Migrations_ShouldReturnTypesWithVersionAttribute_AndImplementIMigration()
        {
            var scanner = new MigrationScanner(_testAssembly);
            var migrations = scanner.Migrations;

            migrations.Should().Contain(typeof(TestMigration1));
            migrations.Should().Contain(typeof(TestMigration2));
        }

        [Test]
        public void Migrations_ShouldNotReturnAbstractClasses()
        {
            var scanner = new MigrationScanner(_testAssembly);
            var migrations = scanner.Migrations;

            migrations.Should().NotContain(typeof(AbstractMigration));
        }

        [Test]
        public void Migrations_ShouldNotReturnClassesWithoutVersionAttribute()
        {
            var scanner = new MigrationScanner(_testAssembly);
            var migrations = scanner.Migrations;

            migrations.Should().NotContain(typeof(NoVersionMigration));
        }

        [Test]
        public void Migrations_ShouldNotReturnClassesWithIgnoreAttribute()
        {
            var scanner = new MigrationScanner(_testAssembly);
            var migrations = scanner.Migrations;

            migrations.Should().NotContain(typeof(IgnoredMigration));
        }

        [Test]
        public void Migrations_ShouldReturnClassesOrderedByVersion()
        {
            var scanner = new MigrationScanner(_testAssembly);
            var migrations = scanner.Migrations;

            migrations.IndexOf(typeof(TestMigration1)).Should().BeLessThan(migrations.IndexOf(typeof(TestMigration2)));
        }

        [Test]
        public void Migrations_ShouldReturnEmptyList_WhenNoValidMigrations()
        {
            var scanner = new MigrationScanner(typeof(object).Assembly); // mscorlib, no migrations
            var migrations = scanner.Migrations;

            migrations.Should().BeEmpty();
        }

        [Version("1.0.0")]
        private class TestMigration1 : IMigration
        {
            public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        [Version("2.0.0")]
        private class TestMigration2 : IMigration
        {
            public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        [Version("3.0.0")]
        private abstract class AbstractMigration : IMigration
        {
            public abstract Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken);
        }

        private class NoVersionMigration : IMigration
        {
            public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        [Ignore]
        [Version("4.0.0")]
        private class IgnoredMigration : IMigration
        {
            public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
