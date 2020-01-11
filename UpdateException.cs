using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater
{
    // TODO Comment
    internal class UpdateException : Exception
    {
        public UpdateException(string message) 
            : base(message)
        { }

        public UpdateException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}
