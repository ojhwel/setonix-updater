using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Download
{
    /// <summary>
    /// Represents a software version.
    /// </summary>
    // TODO This could do with a unit test
    public sealed class Version
    {
        #region Public properties

        /// <summary>
        /// The major version, i.e. the "1" in "1.2.34.56789".
        /// </summary>
        public ushort Major { get; private set; }

        /// <summary>
        /// The minor version, i.e. the "2" in "1.2.34.56789".
        /// </summary>
        public ushort Minor { get; private set; }

        /// <summary>
        /// The revision, i.e. the "34" in "1.2.34.56789". Can be <c>null</c>.
        /// </summary>
        public ushort? Revision { get; private set; }

        /// <summary>
        /// The build number, i.e. the "56789" in "1.2.34.56789". Can be <c>null</c>.
        /// </summary>
        public ushort? Build { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Version</c> instance.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        public Version(ushort major, ushort minor)
        {
            Major = major;
            Minor = minor;
        }

        /// <summary>
        /// Creates a new <c>Version</c> instance.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="revision">The revision.</param>
        public Version(ushort major, ushort minor, ushort revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        /// <summary>
        /// Creates a new <c>Version</c> instance.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="build">The build number.</param>
        public Version(ushort major, ushort minor, ushort revision, ushort build)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
            Build = build;
        }

        /// <summary>
        /// Private. Creates a new <c>Version</c> instance without any version information.
        /// </summary>
        private Version()
        { }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a new <c>Version</c> instance from the string representation of a version number. Major and minor version are required, revision and build
        /// number are optional.
        /// </summary>
        /// <param name="versionNumber">The version number string to parse.</param>
        /// <returns>A new <c>Version</c> instance from the version number string.</returns>
        /// <exception cref="VersionParseException">Thrown if the string could not be parsed as a version.</exception>
        public static Version Parse(string versionNumber)
        {
            string[] segments = versionNumber.Split('.');
            if (segments.Length < 2 || segments.Length > 4)
                throw new VersionParseException();
            Version result = new Version();
            if (ushort.TryParse(segments[0], out ushort major))
                result.Major = major;
            else
                throw new VersionParseException();
            if (ushort.TryParse(segments[1], out ushort minor))
                result.Minor = minor;
            else
                throw new VersionParseException();
            if (segments.Length >= 3 && ushort.TryParse(segments[2], out ushort revision))
                result.Revision = revision;
            else
                throw new VersionParseException();
            if (segments.Length >= 4 && ushort.TryParse(segments[3], out ushort build))
                result.Build = build;
            else
                throw new VersionParseException();
            return result;
        }

        /// <summary>
        /// Tries to convert the string representation of a version number into a <c>Version</c> equivalent.  Major and minor version are required, revision 
        /// and build number are optional.
        /// </summary>
        /// <param name="versionNumber">The version number string to parse.</param>
        /// <param name="result">If the parsing succeeded, a new <c>Version</c> instance from the version number string. <c>null</c> if parsing failed.</param>
        /// <returns><c>true</c> if the parsing succeeded and <c>result</c> contains a corresponding <c>Version</c> instance, otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Creates a new <c>Version</c> instance from a <see cref="System.Version"/>.
        /// </summary>
        /// <param name="version">The <c>System.Version</c> instance.</param>
        /// <returns>A new <c>Version</c> instance from the <see cref="System.Version"/>.</returns>
        public static Version From(System.Version version) => new Version((ushort)version.Major, (ushort)version.Minor, (ushort)version.Revision, (ushort)version.Build);

        #endregion

        #region Overloaded operators, Equals() and GetHashCode()

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

        #endregion

        #region Public methods

        /// <summary>
        /// Returns whether the version represented by this instance is newer than the <c>version</c> parameter.
        /// </summary>
        /// <param name="version">The version to compare to.</param>
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

        /// <summary>
        /// Returns the string representation of the version as major.minor.revision.build.
        /// </summary>
        public override string ToString() => ToString(4);

        /// <summary>
        /// Returns the string representation of the version with the specified number of segments.
        /// <list type="bullet">
        ///     <item>2: major.minor</item>
        ///     <item>3: major.minor.revision</item>
        ///     <item>4: major.minor.revision.build</item>
        /// </list>
        /// </summary>
        /// <param name="segments">The number of version number segments to return, between 2 and 4.</param>
        /// <exception cref="ArgumentException">Thrown if a number of segments less than 2 or greater than 4 was passed.</exception>
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

        #endregion
    }
}
