using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater.Manifest
{
    /// <summary>
    /// Represents a file to be updated.
    /// </summary>
    public class UpdateFileTask : IUpdateTask
    {
        /// <summary>
        /// The relative path and name of the file to update.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// The name of the file to update.
        /// </summary>
        public string FileName => System.IO.Path.GetFileName(Path);

        /// <summary>
        /// Creates a new UpdateFileTask instance.
        /// </summary>
        /// <param name="path">The relative path of the file.</param>
        /// <param name="fileName">The name of the file.</param>
        internal UpdateFileTask(string path, string fileName)
        {
            if (path == "." || path.Length == 0)
                Path = fileName;
            else
                Path = path + "\\" + fileName;
        }

        public override string ToString()
        {
            return "UpdateFileTask: " + Path;
        }
    }
}
