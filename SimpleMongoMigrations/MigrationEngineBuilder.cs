using System.Reflection;

namespace SimpleMongoMigrations
{
    public class MigrationEngineBuilder
    {
        private string _connectionString;
        private string _databaseName;
        private Assembly _assembly;

        public static MigrationEngineBuilder Create() => new MigrationEngineBuilder();

        public MigrationEngineBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public MigrationEngineBuilder WithDatabase(string databaseName)
        {
            _databaseName = databaseName;
            return this;
        }

        public MigrationEngineBuilder WithAssembly(Assembly assembly)
        {
            _assembly = assembly;
            return this;
        }

        public MigrationEngine Build()
        {
            return new MigrationEngine(_connectionString, _databaseName, _assembly);
        }
    }
}
