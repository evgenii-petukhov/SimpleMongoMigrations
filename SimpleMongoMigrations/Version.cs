using SimpleMongoMigrations.Exceptions;
using System;

namespace SimpleMongoMigrations
{
    public struct Version : IComparable<Version>
    {
        private const char VERSION_SPLITTER = '.';
        private const int MAX_LENGTH = 3;
        public readonly int Major;
        public readonly int Minor;
        public readonly int Revision;

        public static Version Zero => new Version(0, 0, 0);

        public Version(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new InvalidVersionException(version);
            }

            var parts = version.Split(VERSION_SPLITTER);

            if (parts.Length > MAX_LENGTH)
            {
                throw new VersionStringTooLongException(version);
            }

            Major = parts.Length > 0 && int.TryParse(parts[0], out var major)
                ? major
                : throw new InvalidVersionException(parts[0]);
            Minor = parts.Length > 1 && int.TryParse(parts[1], out var minor) 
                ? minor
                : 0;
            Revision = parts.Length > 2 && int.TryParse(parts[2], out var revision)
                ? revision
                : 0;
        }


        public Version(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public static implicit operator Version(string version)
        {
            return new Version(version);
        }

        public static implicit operator string(Version version)
        {
            return version.ToString();
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Revision}";
        }

        public int CompareTo(Version other)
        {
            if (Equals(other))
            {
                return 0;
            }

            return this > other ? 1 : -1;
        }

        public static bool operator ==(Version a, Version b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }

        public static bool operator >(Version a, Version b)
        {
            return a.Major > b.Major
                || (a.Major == b.Major && a.Minor > b.Minor)
                || (a.Major == b.Major && a.Minor == b.Minor && a.Revision > b.Revision);
        }

        public static bool operator <(Version a, Version b)
        {
            return a != b && !(a > b);
        }

        public static bool operator <=(Version a, Version b)
        {
            return a == b || a < b;
        }

        public static bool operator >=(Version a, Version b)
        {
            return a == b || a > b;
        }

        public bool Equals(Version other)
        {
            return other.Major == Major && other.Minor == Minor && other.Revision == Revision;
        }

        public override bool Equals(object obj)
        {
            return !(obj is null) && obj.GetType() == typeof(Version) && Equals((Version)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Major;
                result = (result * 397) ^ Minor;
                result = (result * 397) ^ Revision;
                return result;
            }
        }
    }
}
