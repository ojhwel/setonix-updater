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

        public readonly string ApplicationPath;

        public readonly string Language;

        public string[] AdditionalArguments;

        internal ArgumentResult(int callingProcessID, string applicationPath, string language)
        {
            IsValid = true;
            CallingProcessID = callingProcessID;
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
