using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Extensions
{
    // TODO Comments
    public static class CommandLineExtensions
    {
        public static string ConcatenateAll(this string[] args)
        {
            StringBuilder result = new StringBuilder();
            foreach (string s in args)
            {
                result.Append(s);
                result.Append(" ");
            }
            return result.ToString().Trim();
        }
    }
}
