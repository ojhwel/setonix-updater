using Microsoft.VisualBasic.FileIO;
using SetonixUpdater.Manifest;
using SetonixUpdater.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater
{
    /// <summary>
    /// Performs the actual updating.
    /// </summary>
    internal class Updater
    {
        /// <summary>
        /// The tasks to perform.
        /// </summary>
        private readonly IEnumerable<IUpdateTask> tasks;

        /// <summary>
        /// The application path.
        /// </summary>
        private readonly string targetPath;

        /// <summary>
        /// The path with the update files.
        /// </summary>
        private readonly string sourcePath;

        /// <summary>
        /// The callback method on the UpdateForm.
        /// </summary>
        private readonly SetFileNameDelegate fileNameDelegate;

        /// <summary>
        /// Delegate for the callback method on the UpdateForm.
        /// </summary>
        internal delegate void SetFileNameDelegate(string fileName);

        /// <summary>
        /// Creates a new <c>Updater</c> instance.
        /// </summary>
        /// <param name="tasks">The tasks to perform.</param>
        /// <param name="sourcePath">The path with the update files.</param>
        /// <param name="targetPath">The application path.</param>
        /// <param name="fileNameDelegate">The callback method on the UpdateForm.</param>
        internal Updater(IEnumerable<IUpdateTask> tasks, string sourcePath, string targetPath, SetFileNameDelegate fileNameDelegate)
        {
            this.tasks = tasks;
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;
            this.fileNameDelegate = fileNameDelegate;
        }

        /// <summary>
        /// Performs the update, i.e. processes all the <see cref="tasks"/>.
        /// </summary>
        internal void PerformUpdates()
        {
            foreach (IUpdateTask task in tasks)
                if (task is UpdateFileTask fileTask)
                {
                    fileNameDelegate?.Invoke(fileTask.FileName);
                    PerformUpdate(fileTask);
                }
                else
                    throw new NotImplementedException(task.GetType().Name);
        }

        /// <summary>
        /// Performs an <c>UpdateFileTask</c>.
        /// </summary>
        /// <param name="task">The task to perform.</param>
        private void PerformUpdate(UpdateFileTask task)
        {
            FileInfo newFile = new FileInfo(sourcePath + "\\" + task.Path);
            if (!newFile.Exists)
                throw new UpdateException(string.Format(TextResources.NewFileNotFound, task.FileName, newFile.DirectoryName));
            FileInfo oldFile = new FileInfo(targetPath + "\\" + task.Path);
            if (oldFile.Exists)
                FileSystem.DeleteFile(oldFile.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            newFile.CopyTo(oldFile.FullName);
        }
    }
}
