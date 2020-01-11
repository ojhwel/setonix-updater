using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Download
{
    // TODO Comment
    public sealed class VersionInfo
    {
        public string VersionNumber => Version?.ToString() ?? "";

        public Version Version { get; internal set; }

        public string Name { get; internal set; }

        public DateTime ReleaseDate { get; internal set; }

        public string Url { get; internal set; }

        internal VersionInfo()
        { }

        internal VersionInfo(Version version, string name, DateTime releaseDate, string url)
        {
            Version = version;
            Name = name;
            ReleaseDate = releaseDate;
            Url = url;
        }
    }
}
