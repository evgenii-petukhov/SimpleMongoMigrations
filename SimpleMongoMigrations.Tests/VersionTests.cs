using FluentAssertions;
using SimpleMongoMigrations.Exceptions;

namespace SimpleMongoMigrations.Tests
{
    [TestFixture]
    public class VersionTests
    {
        [Test]
        public void Constructor_WithValidString_ShouldParseCorrectly()
        {
            var version = new Version("1.2.3");
            version.Major.Should().Be(1);
            version.Minor.Should().Be(2);
            version.Revision.Should().Be(3);
        }

        [Test]
        public void Constructor_WithValidString_MissingMinorAndRevision_ShouldDefaultToZero()
        {
            var version = new Version("5");
            version.Major.Should().Be(5);
            version.Minor.Should().Be(0);
            version.Revision.Should().Be(0);
        }

        [Test]
        public void Constructor_WithValidString_MissingRevision_ShouldDefaultToZero()
        {
            var version = new Version("2.4");
            version.Major.Should().Be(2);
            version.Minor.Should().Be(4);
            version.Revision.Should().Be(0);
        }

        [Test]
        public void Constructor_WithEmptyString_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version("");
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Constructor_WithNullString_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version(null);
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Constructor_WithTooManyParts_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version("1.2.3.4");
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Constructor_WithNonNumericMajor_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version("a.2.3");
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Constructor_WithNonNumericMinor_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version("1.b.3");
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Constructor_WithNonNumericRevision_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version("1.2.c");
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Constructor_WithEmptyParts_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version("1..3");
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Constructor_WithIntegers_ShouldSetProperties()
        {
            var version = new Version(7, 8, 9);
            version.Major.Should().Be(7);
            version.Minor.Should().Be(8);
            version.Revision.Should().Be(9);
        }

        [Test]
        public void Constructor_WithMaxIntegers_ShouldParseCorrectly()
        {
            var max = int.MaxValue;
            var version = new Version($"{max}.{max}.{max}");
            version.Major.Should().Be(max);
            version.Minor.Should().Be(max);
            version.Revision.Should().Be(max);
        }

        [Test]
        public void Constructor_WithLeadingZeros_ShouldParseCorrectly()
        {
            var version = new Version("01.02.03");
            version.Major.Should().Be(1);
            version.Minor.Should().Be(2);
            version.Revision.Should().Be(3);
        }

        [Test]
        public void Constructor_WithWhitespace_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version(" 1.2.3 ");
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Constructor_WithOnlySeparators_ShouldThrowInvalidVersionException()
        {
            Action act = () => new Version("..");
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Zero_ShouldReturnVersionWithAllZeros()
        {
            var zero = Version.Zero;
            zero.Major.Should().Be(0);
            zero.Minor.Should().Be(0);
            zero.Revision.Should().Be(0);
        }

        [Test]
        public void ImplicitConversion_FromString_ShouldWork()
        {
            Version version = "3.4.5";
            version.Major.Should().Be(3);
            version.Minor.Should().Be(4);
            version.Revision.Should().Be(5);
        }

        [Test]
        public void ImplicitConversion_ToString_ShouldWork()
        {
            var version = new Version(1, 2, 3);
            string str = version;
            str.Should().Be("1.2.3");
        }

        [Test]
        public void ImplicitConversion_FromInvalidString_ShouldThrowInvalidVersionException()
        {
            Action act = () => { Version v = "invalid"; };
            act.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void ToString_ShouldReturnCorrectFormat()
        {
            var version = new Version(10, 20, 30);
            version.ToString().Should().Be("10.20.30");
        }

        [Test]
        public void ToString_ShouldBeConsistentBetweenConstructors()
        {
            var fromString = new Version("4.5.6");
            var fromInts = new Version(4, 5, 6);
            fromString.ToString().Should().Be(fromInts.ToString());
        }

        [TestCase(1, 0, 0, 1, 0, 0, 0)]
        [TestCase(1, 2, 3, 1, 2, 3, 0)]
        [TestCase(2, 0, 0, 1, 0, 0, 1)]
        [TestCase(1, 3, 0, 1, 2, 0, 1)]
        [TestCase(1, 2, 4, 1, 2, 3, 1)]
        [TestCase(1, 2, 3, 2, 0, 0, -1)]
        [TestCase(1, 2, 3, 1, 3, 0, -1)]
        [TestCase(1, 2, 3, 1, 2, 4, -1)]
        public void CompareTo_ShouldReturnExpectedResult(
            int majorA, int minorA, int revA,
            int majorB, int minorB, int revB,
            int expected)
        {
            var a = new Version(majorA, minorA, revA);
            var b = new Version(majorB, minorB, revB);
            a.CompareTo(b).Should().Be(expected);
        }

        [Test]
        public void Equality_Operator_ShouldReturnTrueForEqualVersions()
        {
            var a = new Version(1, 2, 3);
            var b = new Version(1, 2, 3);
            (a == b).Should().BeTrue();
            a.Equals(b).Should().BeTrue();
            a.Equals((object)b).Should().BeTrue();
        }

        [Test]
        public void Inequality_Operator_ShouldReturnTrueForDifferentVersions()
        {
            var a = new Version(1, 2, 3);
            var b = new Version(1, 2, 4);
            (a != b).Should().BeTrue();
            a.Equals(b).Should().BeFalse();
            a.Equals((object)b).Should().BeFalse();
        }

        [Test]
        public void GreaterThan_Operator_ShouldWork()
        {
            var a = new Version(2, 0, 0);
            var b = new Version(1, 9, 9);
            (a > b).Should().BeTrue();
            (b > a).Should().BeFalse();
        }

        [Test]
        public void LessThan_Operator_ShouldWork()
        {
            var a = new Version(1, 0, 0);
            var b = new Version(1, 1, 0);
            (a < b).Should().BeTrue();
            (b < a).Should().BeFalse();
        }

        [Test]
        public void GreaterThanOrEqual_Operator_ShouldWork()
        {
            var a = new Version(1, 2, 3);
            var b = new Version(1, 2, 3);
            var c = new Version(1, 2, 2);
            (a >= b).Should().BeTrue();
            (a >= c).Should().BeTrue();
            (c >= a).Should().BeFalse();
        }

        [Test]
        public void LessThanOrEqual_Operator_ShouldWork()
        {
            var a = new Version(1, 2, 3);
            var b = new Version(1, 2, 3);
            var c = new Version(1, 2, 4);
            (a <= b).Should().BeTrue();
            (a <= c).Should().BeTrue();
            (c <= a).Should().BeFalse();
        }

        [Test]
        public void GetHashCode_ShouldBeEqualForEqualVersions()
        {
            var a = new Version(1, 2, 3);
            var b = new Version(1, 2, 3);
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Test]
        public void GetHashCode_ShouldBeDifferentForDifferentVersions()
        {
            var a = new Version(1, 2, 3);
            var b = new Version(1, 2, 4);
            a.GetHashCode().Should().NotBe(b.GetHashCode());
        }

        [Test]
        public void Equals_ObjectNull_ShouldThrowInvalidVersionException()
        {
            Action action = () => new Version(1, 2, 3).Equals(null);
            action.Should().Throw<InvalidVersionException>();
        }

        [Test]
        public void Equals_ObjectDifferentType_ShouldReturnTrue()
        {
            var a = new Version(1, 2, 3);
            a.Equals("1.2.3").Should().BeTrue();
        }
    }
}
