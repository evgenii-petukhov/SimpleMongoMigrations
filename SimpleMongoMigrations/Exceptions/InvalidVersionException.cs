using System;

namespace SimpleMongoMigrations.Exceptions
{
    public class InvalidVersionException : Exception
    {
        public InvalidVersionException(string version)
            : base(string.Format("Invalid value: {0}", version))
        { }
    }
}
