using System;
using System.Collections.Generic;

namespace SimpleMongoMigrations.Abstractions
{
    internal interface IMigrationScanner
    {
        List<Type> Migrations { get; }
    }
}
