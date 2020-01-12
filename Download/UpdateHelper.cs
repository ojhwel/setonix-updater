using SetonixUpdater.Extensions;
using SetonixUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SetonixUpdater.Download
{
    /// <summary>
    /// Provides functionality for an application to check for, download, and install updates.
    /// <para/>
    /// Basic usage (see each method for more details):
    /// <list type="number">
    ///     <item>Create an instance of <c>UpdateHelper</c></item>
    ///     <item>Call <c>CheckForUpdates()</c> to see if there is a newer version available</item>
    ///     <item>If there is a newer version, call <c>DownloadUpdate()</c> to download it and unpack the archive into a temporary folder</item>
    ///     <item>Call <c>StartUpdate()</c> and quit your application</item>
    ///     <item>(SetonixUpdater will update your application and restart it)</item>
    ///     <item>Be ready to handle cleanup of the temporary in your application by passing the args parameter through <c>HandleTempFolderCleanup()</c></item>
    /// </list>
    /// </summary>
    public class UpdateHelper
    {
        private static Slogger Logger;

        #region Public properties

        /// <summary>
        /// The command line argument that is passed back to the calling application on restart, providing the temporary folder from the update, allowing
        /// cleanup. This is used by <see cref="HandleTempFolderCleanup(string[])"/>.
        /// </summary>
        public const string TempFolderCleanupArgument = "--setonix-cleanup=";

        /// <summary>
        /// Gets or sets the version of the application that is currently installed. This is normally set at instanciation.
        /// </summary>
        public Version LocalVersion { get; set; }

        /// <summary>
        /// Gets or sets the HTTP URL of the version list file. This is normally set at instanciation.
        /// </summary>
        public string VersionListUrl { get; set; }

        /// <summary>
        /// Gets the most current version of the application as stated in the version list file. Prior to calling <see cref="CheckForUpdates()"/>, this is <c>null</c>.
        /// </summary>
        public VersionInfo CurrentVersion { get; private set; }

        /// <summary>
        /// Gets the temporary directory into which the update archive has been decompressed after calling <see cref="DownloadUpdate()"/>.
        /// </summary>
        public DirectoryInfo TemporaryFolder { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>UpdateHelper</c> instance.
        /// </summary>
        /// <param name="localVersion">The currently installed version of the application to check against.</param>
        /// <param name="versionListUrl">The HTTP URL of the version list file.</param>
        public UpdateHelper(Version localVersion, string versionListUrl)
        {
            CreateLogger();
            LocalVersion = localVersion;
            VersionListUrl = versionListUrl;
            Logger.Debug("UpdateHelper created with local version \"" + localVersion + "\" and URL \"" + versionListUrl + "\"");
        }

        /// <summary>
        /// Creates a new <c>UpdateHelper</c> instance.
        /// </summary>
        /// <param name="localVersion">The currently installed version of the application to check against.</param>
        /// <param name="versionListUrl">The HTTP URL of the version list file.</param>
        /// <exception cref="VersionParseException">Thrown if the <c>localVersion</c> parameter cannot be parsed into a <see cref="Version"/></exception>
        public UpdateHelper(string localVersion, string versionListUrl)
        {
            CreateLogger();
            Logger.Debug("Local version argument: " + localVersion);
            LocalVersion = Version.Parse(localVersion);
            VersionListUrl = versionListUrl;
            Logger.Debug("UpdateHelper created with local version \"" + localVersion + "\" and URL \"" + versionListUrl + "\"");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Checks whether a newer version than the one currently installed is in the version list file. If successful, sets the <see cref="CurrentVersion"/> property.
        /// </summary>
        /// <returns><c>true</c> if there is a newer version, otherwise <c>false</c>.</returns>
        /// <exception cref="UpdateCheckException">Thrown if the version list file could not be read.</exception>
        public bool CheckForUpdates()
        {
            string tempFileName = Path.GetTempFileName();
            Logger.Debug("Version list temp file name: " + tempFileName);
            FileInfo versionFile = new FileInfo(tempFileName);
            try
            {
                if (!DownloadFile(VersionListUrl, versionFile.FullName))
                {
                    CurrentVersion = null;
                    Logger.Error("DownloadFile returned false");
                    throw new UpdateCheckException("Unable to download version information");
                }
                CurrentVersion = GetCurrentVersion(versionFile);
                if (CurrentVersion == null)
                {
                    Logger.Error("Current version is null");
                    throw new UpdateCheckException("Unable to determine current version");
                }
                Logger.Debug("Current version is: " + CurrentVersion.Version);
                return CurrentVersion.Version.IsNewerThan(LocalVersion);
            }
            catch (Exception e)
            {
                throw new UpdateCheckException("Unable to check for newer version: " + e.Message, e);
            }
            finally
            {
                try
                {
                    Logger.Trace("Deleting temp file");
                    versionFile?.Delete();
                }
                catch (Exception e)
                {
                    Logger.Info("Error deleting temp file: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Downloads the update file and unpacks it into a newly created temporary folder, to which the <see cref="TemporaryFolder"/> property is set on
        /// success. Requires <see cref="CheckForUpdates"/> to have been called previously.
        /// </summary>
        /// <returns><c>true</c> if the update was successfully downloaded, otherwise <c>false</c></returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="CheckForUpdates"/> has not been successfully called previously.</exception>
        /// <exception cref="UpdateException">Thrown if the update archive could not be downloaded.</exception>
        public bool DownloadUpdate()
        {
            if (CurrentVersion == null)
            {
                Logger.Error("CurrentVersion not set");
                throw new InvalidOperationException("Current version has not been determined");
            }

            string tempFileName = Path.GetTempFileName();
            Logger.Debug("ZIP temp file name: " + tempFileName);
            FileInfo updateFile = new FileInfo(tempFileName);
            try
            {
                // TODO Having these exceptions could be useful
                if (!DownloadFile(CurrentVersion.Url, updateFile.FullName))
                {
                    Logger.Error("DownloadFile returned false");
                    throw new UpdateException("Error downloading update from " + CurrentVersion.Url);
                }

                TemporaryFolder = GetTempFolder("setonix_update");
                Logger.Debug("Unzipping to: " + TemporaryFolder);
                ZipFile.ExtractToDirectory(updateFile.FullName, TemporaryFolder.FullName);
                Logger.Trace("Unzipping finished");
                return true;
            }
            catch (UpdateException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                Logger.Error("Error downloading/unzippung update: " + e.Message);
                return false;
            }
            finally
            {
                try
                {
                    Logger.Trace("Deleting temp file");
                    updateFile?.Delete();
                }
                catch (Exception e)
                {
                    Logger.Info("Error deleting temp file: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Starts the update after <see cref="DownloadUpdate"/> has been called successfully. The calling application should quit immediately afterwards.
        /// </summary>
        /// <param name="processID">The process ID of the calling application, to allow checking if it is still running before starting the update. Unless 
        /// there is reason not to, pass <c>System.Diagnostics.Process.GetCurrentProcess().Id</c> for this.</param>
        /// <param name="executable">The full path to the application fo restartung it after the update.</param>
        public void StartUpdate(int processID, string executable)
        {
            StartUpdate(processID, executable, null);
        }

        /// <summary>
        /// Starts the update after <see cref="DownloadUpdate"/> has been called successfully. The calling application should quit immediately afterwards.
        /// </summary>
        /// <param name="processID">The process ID of the calling application, to allow checking if it is still running before starting the update. Unless 
        /// there is reason not to, pass <c>System.Diagnostics.Process.GetCurrentProcess().Id</c> for this.</param>
        /// <param name="executable">The full path to the application fo restartung it after the update.</param>
        /// <param name="args">Any command line arguments that should be passed to the application on restart. An extra argument will be added to allow
        /// cleaning up the temporary files after the update; see <see cref="HandleTempFolderCleanup(string[])"/>.</param>
        /// <exception cref="FileNotFoundException">Thrown if the updater executable (<c>setonix_updater.exe</c>) does not exist in the temporary folder
        /// with the archive contents (see <see cref="DownloadUpdate"/>).</exception>
        public void StartUpdate(int processID, string executable, string[] args)
        {
            FileInfo updater = new FileInfo(TemporaryFolder + "\\setonix_updater.exe");
            if (!updater.Exists)
            {
                Logger.Error("Updater doesn't exist: " + updater.FullName);
                throw new FileNotFoundException(updater.FullName);
            }
            else
                Logger.Debug("Updater found: " + updater.FullName);

            string arguments = processID.ToString() + " \"" + executable + "\"";
            if (args != null)
                arguments += " " + args.ConcatenateAll();
            Logger.Debug("Command line: \"" + arguments + "\"");

            try
            {
                Logger.Trace("Starting updater");
                Process.Start(updater.FullName, arguments);
            }
            catch (Exception e)
            {
                Logger.Error("Unable to start updater: " + e.Message);
            }
        }

        /// <summary>
        /// Checks if the <seealso cref="TempFolderCleanupArgument">temporary folder cleanup argument</seealso> is among the command line arguments passed,
        /// and deletes that folder, returning all other arguments unchanged.
        /// <para/>
        /// In order to remove the temporary files from an update, the calling application should have this line before any action is taken on the command line
        /// arguments:
        /// <code>args = SetonixUpdater.Download.UpdateHelpet.HandleTempFolderCleanup(args);</code>
        /// </summary>
        /// <param name="args"></param>
        /// <returns>The command line arguments passed into the methods, minus the temporary folder cleanup argument if it was present.</returns>
        public static string[] HandleTempFolderCleanup(string[] args)
        {
            CreateLogger();
            Logger.Trace("Entering HandleTempFolderCleanup");
            List<string> result = new List<string>();
            foreach (string arg in args)
                if (arg.ToLower().StartsWith(TempFolderCleanupArgument))
                {
                    Logger.Debug("Found cleanup argument: \"" + arg + "\"");
                    RemoveTempFolder(arg);
                }
                else
                    result.Add(arg);
            return result.ToArray();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Removes the specified temporary folder and its contents, but only if the path is in the system temp path.
        /// </summary>
        /// <param name="path">The path to delete.</param>
        /// <returns><c>true</c> of the path was deleted, otherwise <c>false</c>.</returns>
        private static bool RemoveTempFolder(string path)
        {
            Logger?.Debug("Removing temp folder: \"" + path + "\"");
            try
            {
                string folder;
                if (path.ToLower().StartsWith(TempFolderCleanupArgument))
                    folder = path.Substring(TempFolderCleanupArgument.Length);
                else
                {
                    Logger?.Error("Not a cleanup argument: \"" + path + "\"");
                    return false;
                }
                if (!folder.StartsWith(Path.GetTempPath()))
                {
                    Logger?.Warn("Temp folder is not under temp path: " + path);
                    return false;
                }

                DirectoryInfo di = new DirectoryInfo(folder);
                if (di.Exists)
                {
                    Logger?.Trace("Deleting temp folder");
                    di.Delete(true);
                }
                else
                    Logger?.Info("Temp folder does not exist");
                return true;
            }
            catch (Exception e)
            {
                Logger?.Error("Error deleting temp folder: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Returns the path of a newly created temporary folder in the system temp path.  If a temporary folder of the specified name already exists, a
        /// number will be added to it.
        /// </summary>
        /// <param name="folderName">The name of the temporary folder to create.</param>
        /// <returns>A <c>DirectoryInfo</c> instance of the new temporary folder.</returns>
        private DirectoryInfo GetTempFolder(string folderName)
        {
            DirectoryInfo result;
            int i = 0;
            bool success = false;
            do
            {
                string path = Path.GetTempPath() + "\\" + folderName + (i > 0 ? "_" + i.ToString() : "");
                result = new DirectoryInfo(path);
                if (!result.Exists)
                    try
                    { 
                        result.Create();
                        success = true;
                    }
                    catch (IOException)
                    {
                        success = false;
                    }
                i++;
            }
            while (!success);
            if (success)
                return result;
            else
                throw new IOException();
        }

        /// <summary>
        /// Downloads the file at the URL and writes it to the local disk as the local file name. If the file already exists, it is overwritten.
        /// </summary>
        /// <param name="url">The HTTP URL of the file to download.</param>
        /// <param name="localFile">The full path of the local file to write.</param>
        /// <returns><c>true</c> if the file was downloaded, otherwise <c>false</c>.</returns>
        /// TODO This should not swallow its exceptions
        private bool DownloadFile(string url, string localFile)
        {
            const int BUFFER_SIZE = 2048;
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.KeepAlive = true;
                request.Method = WebRequestMethods.Http.Get; 
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;

                Stream httpStream = response.GetResponseStream();
                FileStream fileStream = new FileStream(localFile, FileMode.Create);
                byte[] byteBuffer = new byte[BUFFER_SIZE];
                int bytesRead = httpStream.Read(byteBuffer, 0, BUFFER_SIZE);
                try
                {
                    while (bytesRead > 0)
                    {
                        fileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = httpStream.Read(byteBuffer, 0, BUFFER_SIZE);
                    }
                }
                catch (Exception e) 
                {
                    Logger.Error("Error reading download stream: " + e.Message);
                }
                fileStream.Close();
                httpStream.Close();
                response.Close();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("Error downloading file: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Returns the current version from the specified version file.
        /// </summary>
        /// <param name="versionFile">The version file.</param>
        /// <returns>A <see cref="VersionInfo"/> instance describing the current version, or <c>null</c> if that version could not be definitely 
        /// determined.</returns>
        /// TODO This should not swallow its exceptions
        private VersionInfo GetCurrentVersion(FileInfo versionFile)
        {
            List<VersionInfo> versions = new List<VersionInfo>();
            XmlDocument doc = new XmlDocument();
            try
            {
                Logger.Trace("Loading version file");
                doc.Load(versionFile.FullName);
                foreach (XmlNode node in doc.SelectNodes("/versions/version"))
                {
                    Logger.Trace("Found version");
                    string versionStr = node.GetAttributeValue("id", true);
                    Logger.Debug("Version ID: " + versionStr);
                    string name = node.GetValue("name", "");
                    Logger.Debug("Name: " + name);
                    DateTime releaseDate = node.GetValue("release-date", DateTime.MinValue, "yyyy-MM-dd");
                    Logger.Debug("Release date: " + releaseDate);
                    string url = node.GetValue("url", "");
                    Logger.Debug("URL: " + url);
                    Version version = Version.Parse(versionStr);
                    if (name.Length == 0 || releaseDate == DateTime.MinValue || url.Length == 0)
                    {
                        Logger.Warn("Invalid version: name=\"" + name + "\", releaseDate=\"" + releaseDate + "\", url=\"" + url + "\"");
                        return null;
                    }
                    Logger.Trace("Adding version");
                    versions.Add(new VersionInfo(version, name, releaseDate, url));
                }

                VersionInfo highestVersion = null;
                DateTime newestDate = DateTime.MinValue;
                foreach (VersionInfo vi in versions)
                {
                    if (highestVersion == null || vi.Version.IsNewerThan(highestVersion.Version))
                        highestVersion = vi;
                    if (newestDate == null || vi.ReleaseDate > newestDate)
                        newestDate = vi.ReleaseDate;
                }
                Logger.Debug("Highest version: " + highestVersion.Version + " (date: " + highestVersion.ReleaseDate + ")");
                Logger.Debug("Newest date: " + newestDate);
                if (newestDate == highestVersion.ReleaseDate)
                    return highestVersion;
                else
                    return null;
            }
            catch (Exception e)
            {
                Logger.Error("Error reading version file: " + e.Message);
                return null;
            }
        }

        private static void CreateLogger()
        {
            Logger = new Slogger(Path.GetTempPath(), "setonix_updater.log");
        }

        #endregion
    }
}
