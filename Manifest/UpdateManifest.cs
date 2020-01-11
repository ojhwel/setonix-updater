using SetonixUpdater.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SetonixUpdater.Manifest
{
    /// <summary>
    /// Represents the update manifest.
    /// </summary>
    public class UpdateManifest
    {
        #region Private properties

        /// <summary>
        /// The tasks to perform. Private, mutable list.
        /// </summary>
        private readonly List<IUpdateTask> tasks = new List<IUpdateTask>();

        #endregion

        #region Public properties

        /// <summary>
        /// Update-specific or application-specific strings in the manifest.
        /// </summary>
        public readonly LocalizedStringCollection Strings = new LocalizedStringCollection();

        /// <summary>
        /// The tasks to perform.
        /// </summary>
        public IList<IUpdateTask> Tasks => tasks;

        #endregion

        #region Private constructor

        /// <summary>
        /// Creates a new UpdateManifest instance. Private.
        /// </summary>
        private UpdateManifest()
        { }

        #endregion

        #region Factory method

        /// <summary>
        /// Returns a new UpdateManifest instance representing the contents of the manifest file.
        /// </summary>
        /// <param name="manifestFile">The manifest file to read.</param>
        /// <exception cref="MissingXmlAttributeException" />
        public static UpdateManifest FromFile(string manifestFile)
        {
            UpdateManifest result = new UpdateManifest();
            XmlDocument doc = new XmlDocument();
            doc.Load(manifestFile);

            foreach (XmlNode folderNode in doc.SelectNodes("/manifest/folder"))
            {
                string folderPath = folderNode.GetAttributeValue("path", true);
                foreach (XmlNode fileNode in folderNode.SelectNodes("file"))
                {
                    string fileName = fileNode.GetAttributeValue("name", true);
                    result.AddTask(new UpdateFileTask(folderPath, fileName));
                }
            }

            foreach (XmlNode messageNode in doc.SelectNodes("/manifest/messages/message"))
            {
                string language = messageNode.GetAttributeValue("language", true);
                string key = messageNode.GetAttributeValue("key", true);
                string text = messageNode.InnerText.Trim();
                result.Strings.Add(language, key, text);
            }

            return result;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Adds a task to the task list.
        /// </summary>
        /// <param name="task">The task to add.</param>
        internal void AddTask(IUpdateTask task)
        {
            tasks.Add(task);
        }

        #endregion

        public override string ToString()
        {
            return "UpdateManifest: " + tasks.Count + " task(s)";
        }
    }
}
