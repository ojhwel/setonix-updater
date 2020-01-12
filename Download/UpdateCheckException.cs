using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Download
{
    /// <summary>
    ///  Thrown for error encountered during checking for an updated version.
    /// </summary>
    public sealed class UpdateCheckException : Exception
    {
        public UpdateCheckException()
        { }

        public UpdateCheckException(string message) : base(message)
        { }

        public UpdateCheckException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}
