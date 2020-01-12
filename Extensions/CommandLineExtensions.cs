using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Extensions
{
    public static class CommandLineExtensions
    {
        /// <summary>
        /// Returns all arguments as a single string, separated by spaces.
        /// </summary>
        /// <param name="args">The arguments to concatenate.</param>
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
