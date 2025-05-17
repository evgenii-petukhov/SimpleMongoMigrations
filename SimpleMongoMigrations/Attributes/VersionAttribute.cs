using System;

namespace SimpleMongoMigrations.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class VersionAttribute : Attribute
    {
        public Version Version { get; }

        public VersionAttribute(string version)
        {
            Version = version;
        }
    }
}
