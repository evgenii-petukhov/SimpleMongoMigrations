using System.Reflection;

namespace SimpleMongoMigrations
{
    /// <summary>
    /// Provides a fluent API for configuring and creating <see cref="MigrationEngine"/> instances.
    /// </summary>
    public class MigrationEngineBuilder
    {
        private string _connectionString;
        private string _databaseName;
        private TransactionScope _transactionScope;
        private Assembly _assembly;

        /// <summary>
        /// Creates a new instance of <see cref="MigrationEngineBuilder"/>.
        /// </summary>
        public static MigrationEngineBuilder Create() => new MigrationEngineBuilder();

        /// <summary>
        /// Sets the MongoDB connection string to be used by the migration engine.
        /// </summary>
        /// <param name="connectionString">The MongoDB connection string.</param>
        /// <returns>The current <see cref="MigrationEngineBuilder"/> instance.</returns>
        public MigrationEngineBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        /// <summary>
        /// Sets the name of the MongoDB database to be used by the migration engine.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns>The current <see cref="MigrationEngineBuilder"/> instance.</returns>
        public MigrationEngineBuilder WithDatabase(string databaseName)
        {
            _databaseName = databaseName;
            return this;
        }

        /// <summary>
        /// Sets the assembly containing migration classes.
        /// </summary>
        /// <param name="assembly">The assembly to scan for migrations.</param>
        /// <returns>The current <see cref="MigrationEngineBuilder"/> instance.</returns>
        public MigrationEngineBuilder WithAssembly(Assembly assembly)
        {
            _assembly = assembly;
            return this;
        }

        /// <summary>
        /// Sets the transaction scope to be used when applying migrations.
        /// </summary>
        /// <param name="transactionScope">The desired <see cref="TransactionScope"/>.</param>
        /// <returns>The current <see cref="MigrationEngineBuilder"/> instance.</returns>
        public MigrationEngineBuilder WithTransactionScope(TransactionScope transactionScope)
        {
            _transactionScope = transactionScope;
            return this;
        }

        /// <summary>
        /// Builds a configured <see cref="MigrationEngine"/> instance.
        /// </summary>
        /// <returns>A new <see cref="MigrationEngine"/> instance.</returns>
        public MigrationEngine Build()
        {
            return new MigrationEngine(_connectionString, _databaseName, _transactionScope, _assembly);
        }
    }
}