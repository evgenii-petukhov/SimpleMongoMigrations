using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties


// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.

[assembly: Guid("0dcbe9e5-2159-438e-96e6-4063a3666769")]
[assembly: InternalsVisibleTo("SimpleMongoMigrations.Tests")]
[assembly: InternalsVisibleTo("SimpleMongoMigrations.Tests.NoMigrations")]
[assembly: InternalsVisibleTo("SimpleMongoMigrations.Tests.VerifyMigrationOrder")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]