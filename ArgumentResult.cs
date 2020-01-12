using SetonixUpdater.Extensions;
using SetonixUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater
{
    /// <summary>
    /// Represents the command line.
    /// </summary>
    internal class ArgumentResult
    {
        /// <summary>
        /// Whether the command line arguments are valid.
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// The process ID of the calling application.
        /// </summary>
        public readonly int CallingProcessID;

        /// <summary>
        /// The full path of the application executable to restart after updating.
        /// </summary>
        public readonly string ApplicationPath;

        /// <summary>
        /// The UI language.
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// All other, unchecked arguments.
        /// </summary>
        public string[] AdditionalArguments;

        /// <summary>
        /// Represents an invalid command line. Read-only.
        /// </summary>
        internal static ArgumentResult Invalid => new ArgumentResult();

        

        /// <summary>
        /// Creates a new <c>ArgumentResult</c> instance.
        /// </summary>
        /// <param name="callingProcessID">The process ID of the calling application.</param>
        /// <param name="applicationPath">The full path of the application executable to restart after updating.</param>
        /// <param name="language">The UI language.</param>
        internal ArgumentResult(int callingProcessID, string applicationPath, string language)
        {
            IsValid = true;
            CallingProcessID = callingProcessID;
            ApplicationPath = applicationPath;
            Language = language;
        }

        /// <summary>
        /// Returns the <seealso cref="AdditionalArguments">additional arguments</seealso> as a string.
        /// </summary>
        internal string GetAdditionalArgumentsAsString()
        {
            if (AdditionalArguments != null)
                return AdditionalArguments.ConcatenateAll();
            else
                return string.Empty;
        }

        /// <summary>
        /// Private. Creates a new <c>ArgumentResult</c> instance without any data. Only used for <see cref="Invalid"/>.
        /// </summary>
        private ArgumentResult()
        {
            IsValid = false;
        }
    }
}
