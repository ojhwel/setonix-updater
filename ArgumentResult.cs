using SetonixUpdater.Extensions;
using SetonixUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater
{
    // TODO Comment
    internal class ArgumentResult
    {
        public readonly bool IsValid;

        public readonly int CallingProcessID;

        public readonly System.IO.DirectoryInfo UpdatePath;

        public readonly string ApplicationPath;

        public readonly string Language;

        public string[] AdditionalArguments;

        internal ArgumentResult(int callingProcessID, System.IO.DirectoryInfo updatePath, string applicationPath, string language)
        {
            IsValid = true;
            CallingProcessID = callingProcessID;
            UpdatePath = updatePath;
            ApplicationPath = applicationPath;
            Language = language;
        }

        internal string GetAdditionalArgumentsAsString()
        {
            return AdditionalArguments.ConcatenateAll();
        }

        private ArgumentResult()
        {
            IsValid = false;
        }

        internal static ArgumentResult Invalid => new ArgumentResult();
    }
}
