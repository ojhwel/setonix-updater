using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Download
{
    /// <summary>
    /// Thrown if a version number string could not be parsed.
    /// </summary>
    public class VersionParseException : Exception
    {
        public VersionParseException()
        { }

        public VersionParseException(string message) : base(message)
        { }
    }
}
