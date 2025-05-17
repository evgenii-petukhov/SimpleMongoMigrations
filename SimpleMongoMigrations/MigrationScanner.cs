using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Abstractions;

namespace SimpleMongoMigrations
{
    public class MigrationScanner
    {
        private readonly Lazy<List<Type>> _migrationTypes;

        public MigrationScanner(Assembly assembly)
        {
            _migrationTypes = new Lazy<List<Type>>(() => GetValidMigrations(assembly));
        }

        public List<Type> Migrations => _migrationTypes.Value;

        private List<Type> GetValidMigrations(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type =>
                    typeof(IMigration).IsAssignableFrom(type) &&
                    !type.IsAbstract &&
                    type.IsClass &&
                    type.GetCustomAttribute<IgnoreAttribute>() == null &&
                    type.GetCustomAttribute<VersionAttribute>() != null)
                .OrderBy(type => type.GetCustomAttribute<VersionAttribute>()?.Version)
                .ToList();
        }
    }
}
