using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Download
{
    /// <summary>
    /// Represents an application version downloadable from the internet.
    /// </summary>
    public sealed class VersionInfo
    {
        /// <summary>
        /// The version number as a string.
        /// </summary>
        public string VersionNumber => Version?.ToString() ?? "";

        /// <summary>
        /// The version number.
        /// </summary>
        public Version Version { get; internal set; }

        /// <summary>
        /// The name of the version.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The release date of the version.
        /// </summary>
        public DateTime ReleaseDate { get; internal set; }

        /// <summary>
        /// The URL the version can be downloaded from.
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// Creates a new <c>VersionInfo</c> instance.
        /// </summary>
        internal VersionInfo()
        { }

        /// <summary>
        /// Creates a new <c>VersionInfo</c> instance.
        /// </summary>
        /// <param name="version">The version number.</param>
        /// <param name="name">The name of the version.</param>
        /// <param name="releaseDate">The release date of the version.</param>
        /// <param name="url">The URL the version can be downloaded from.</param>
        internal VersionInfo(Version version, string name, DateTime releaseDate, string url)
        {
            Version = version;
            Name = name;
            ReleaseDate = releaseDate;
            Url = url;
        }
    }
}
