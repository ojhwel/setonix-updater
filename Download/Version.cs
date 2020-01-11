using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Download
{
    // TODO Comment
    public sealed class Version
    {
        public int Major { get; private set; }

        public int Minor { get; private set; }

        public int? Revision { get; private set; }

        public int? Build { get; private set; }

        public Version(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public Version(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public Version(int major, int minor, int revision, int build)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
            Build = build;
        }

        private Version()
        { }

        public static Version From(System.Version version) => new Version(version.Major, version.Minor, version.Revision, version.Build);

        public bool IsNewerThan(Version version)
        {
            if (this.Major != version.Major)
                return this.Major > version.Major;
            if (this.Minor != version.Minor)
                return this.Minor > version.Minor;
            if (this.Revision != version.Revision)
                return this.Revision > version.Revision || version.Revision == null;
            if (this.Build != version.Build)
                return this.Build > version.Build || version.Build == null;
            return false;
        }

        public override string ToString() => ToString(4);

        public string ToString(byte segments)
        {
            if (segments <= 0 || segments > 4)
                throw new ArgumentException();

            StringBuilder result = new StringBuilder(Major);
            if (segments >= 2 && Minor >= 0)
                result.Append(".").Append(Minor);
            if (segments >= 3 && Revision != null)
                result.Append(".").Append(Revision);
            if (segments == 4 && Build != null)
                result.Append(".").Append(Build);
            return result.ToString();
        }

        public static Version Parse(string versionNumber)
        {
            string[] segments = versionNumber.Split('.');
            if (segments.Length < 2 || segments.Length > 4)
                throw new VersionParseException();
            Version result = new Version();
            if (int.TryParse(segments[0], out int major))
                result.Major = major;
            else
                throw new VersionParseException();
            if (int.TryParse(segments[1], out int minor))
                result.Minor = minor;
            else
                throw new VersionParseException();
            if (segments.Length >= 3 && int.TryParse(segments[2], out int revision))
                result.Revision = revision;
            else
                throw new VersionParseException();
            if (segments.Length >= 4 && int.TryParse(segments[3], out int build))
                result.Build = build;
            else
                throw new VersionParseException();
            return result;
        }

        public static bool TryParse(string versionNumber, out Version result)
        {
            try
            {
                result = Parse(versionNumber);
                return true;
            }
            catch (VersionParseException)
            {
                result = null;
                return false;
            }
        }

        public static bool operator ==(Version v1, Version v2) => v1.Equals(v2);

        public static bool operator !=(Version v1, Version v2) => !v1.Equals(v2);

        public static bool operator >(Version v1, Version v2) => v1.IsNewerThan(v2);

        public static bool operator <(Version v1, Version v2) => v2.IsNewerThan(v1);

        public override bool Equals(object obj)
        {
            if (obj is Version version)
                return this.Major == version.Major &&
                       this.Minor == version.Minor &&
                       (this.Revision == version.Revision || (this.Revision == null && version.Revision == null)) &&
                       (this.Build == version.Build || (this.Build == null && version.Build == null));
            else
                return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 553019227;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + Revision.GetHashCode();
            hashCode = hashCode * -1521134295 + Build.GetHashCode();
            return hashCode;
        }
    }
}
