using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Download
{
    // TODO Comment
    class VersionParseException : Exception
    {
        public VersionParseException()
        { }

        public VersionParseException(string message) : base(message)
        { }
    }
}
