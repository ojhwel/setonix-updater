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
    // TODO Comments
    internal class Updater
    {
        private readonly IEnumerable<IUpdateTask> tasks;
        private readonly string targetPath;
        private readonly string sourcePath;
        private readonly SetFileNameDelegate fileNameDelegate;

        internal delegate void SetFileNameDelegate(string fileName);

        internal Updater(IEnumerable<IUpdateTask> tasks, string sourcePath, string targetPath, SetFileNameDelegate fileNameDelegate)
        {
            this.tasks = tasks;
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;
            this.fileNameDelegate = fileNameDelegate;
        }

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
