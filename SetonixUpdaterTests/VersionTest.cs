using NUnit.Framework;
using SetonixUpdater.Download;

namespace SetonixUpdaterTests
{
    public class Tests
    {
        [Test]
        public void Properties()
        {
            Version v = new Version(19, 9, 4, 26933);
            Assert.AreEqual(19, v.Major);
            Assert.AreEqual(9, v.Minor);
            Assert.AreEqual(4, v.Revision);
            Assert.AreEqual(26933, v.Build);
        }

        [Test]
        public new void ToString()
        {
            Version v = new Version(5, 0);
            Assert.AreEqual("5.0", v.ToString(2));
            Assert.AreEqual("5.0", v.ToString(3));
            Assert.AreEqual("5.0", v.ToString(4));
            Assert.AreEqual("5.0", v.ToString());

            v = new Version(3, 5, 1);
            Assert.AreEqual("3.5", v.ToString(2));
            Assert.AreEqual("3.5.1", v.ToString(3));
            Assert.AreEqual("3.5.1", v.ToString(4));
            Assert.AreEqual("3.5.1", v.ToString());

            v = new Version(19, 9, 4, 26933);
            Assert.AreEqual("19.9", v.ToString(2));
            Assert.AreEqual("19.9.4", v.ToString(3));
            Assert.AreEqual("19.9.4.26933", v.ToString(4));
            Assert.AreEqual("19.9.4.26933", v.ToString());
        }

        [Test]
        public void Parse()
        {
            Version v = Version.Parse("5.7");
            Assert.AreEqual(5, v.Major);
            Assert.AreEqual(7, v.Minor);
            Assert.IsNull(v.Revision);
            Assert.IsNull(v.Build);

            v = Version.Parse("12.0.8");
            Assert.AreEqual(12, v.Major);
            Assert.AreEqual(0, v.Minor);
            Assert.AreEqual(8, v.Revision);
            Assert.IsNull(v.Build);

            v = Version.Parse("12.0.8.4711");
            Assert.AreEqual(12, v.Major);
            Assert.AreEqual(0, v.Minor);
            Assert.AreEqual(8, v.Revision);
            Assert.AreEqual(4711, v.Build);

            Assert.Throws<VersionParseException>(() => Version.Parse("14"));
            Assert.Throws<VersionParseException>(() => Version.Parse("7."));
            Assert.Throws<VersionParseException>(() => Version.Parse(".0"));
            Assert.Throws<VersionParseException>(() => Version.Parse(""));
            Assert.Throws<VersionParseException>(() => Version.Parse(".."));
            Assert.Throws<VersionParseException>(() => Version.Parse("15.0."));
            Assert.Throws<VersionParseException>(() => Version.Parse("15.0.3."));
            Assert.Throws<VersionParseException>(() => Version.Parse(".0.6"));
        }
        
        [Test]
        public void TryParse()
        {
            Version.TryParse("18.4.3", out Version v);
            Assert.AreEqual("18.4.3", v.ToString());

            Assert.IsFalse(Version.TryParse(null, out Version v2));
            Assert.IsNull(v2);
        }

        [Test]
        public void Operators()
        {
            Version v1 = Version.Parse("3.1.2");
            Version v2 = new Version(3, 1, 2);
            Version v3 = new Version(3, 1, 3);
            Assert.IsTrue(v1 == v2);
            Assert.IsFalse(v1 == v3);
            Assert.IsTrue(v3 > v2);
            Assert.IsTrue(v1 < v3);

            Assert.IsTrue(v1.Equals(v2));
        }

        [Test]
        public void NewerThan()
        {
            Version v1 = Version.Parse("3.1.2");
            Version v2 = new Version(3, 1, 2);
            Version v3 = new Version(3, 1, 3);
            Assert.IsFalse(v1.IsNewerThan(v2));
            Assert.IsTrue(v3.IsNewerThan(v2));
            Assert.IsFalse(v1.IsNewerThan(v3));

            Assert.IsTrue(v1.Equals(v2));
        }
    }
}