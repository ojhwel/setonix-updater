using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Extensions
{
    /// <summary>
    /// Thrown if a required XML argument is missing.
    /// </summary>
    public class MissingXmlAttributeException : Exception
    {
        /// <summary>
        /// The missing attribute.
        /// </summary>
        public readonly string Attribute;

        /// <summary>
        /// Thrown if a required XML argument is missing.
        /// </summary>
        public MissingXmlAttributeException()
        { }

        /// <summary>
        /// Thrown if a required XML argument is missing.
        /// </summary>
        /// <param name="attribute">The missing attribute.</param>
        public MissingXmlAttributeException(string attribute) => Attribute = attribute;
    }
}

