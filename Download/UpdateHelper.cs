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
    // TODO Comment
    public sealed class UpdateHelper
    {
        public const string TempFolderCleanupArgument = "--setonix-cleanup=";

        public Version LocalVersion { get; set; }

        public string VersionListUrl { get; set; }

        public VersionInfo CurrentVersion { get; private set; }

        public DirectoryInfo TemporaryFolder { get; private set; }

        public UpdateHelper(Version localVersion, string versionListUrl)
        {
            LocalVersion = localVersion;
            VersionListUrl = versionListUrl;
        }

        public bool CheckForUpdates()
        {
            FileInfo versionFile = new FileInfo(Path.GetTempFileName());
            try
            {
                if (!DownloadFile(VersionListUrl, versionFile.FullName))
                {
                    CurrentVersion = null;
                    throw new UpdateCheckException("Unable to download version information");
                }
                CurrentVersion = GetCurrentVersion(versionFile);
                if (CurrentVersion == null)
                    throw new UpdateCheckException("Unable to determine current version");
                return CurrentVersion.Version.IsNewerThan(LocalVersion);
            }
            catch (Exception e)
            {
                throw new UpdateCheckException("Unable to check for newer version", e);
            }
            finally
            {
                try
                {
                    versionFile?.Delete();
                }
                catch (Exception)
                { }
            }
        }

        public bool DownloadUpdate()
        {
            if (CurrentVersion == null)
                throw new InvalidOperationException("Current version has not been determined");

            FileInfo updateFile = new FileInfo(Path.GetTempFileName());
            try
            {
                if (!DownloadFile(CurrentVersion.Url, updateFile.FullName))
                    throw new UpdateException("Error downloading update from " + CurrentVersion.Url);

                TemporaryFolder = GetTempFolder("setonix_update");
                ZipFile.ExtractToDirectory(updateFile.FullName, TemporaryFolder.FullName);
                return true;
            }
            finally
            {
                try
                {
                    updateFile?.Delete();
                }
                catch (Exception)
                { }
            }
            return false;
        }

        public void StartUpdate(int processID, string executable)
        {
            StartUpdate(processID, executable, null);
        }

        public void StartUpdate(int processID, string executable, string[] args)
        {
            FileInfo updater = new FileInfo(TemporaryFolder + "\\setonix_updater.exe");
            if (!updater.Exists)
                throw new FileNotFoundException(updater.FullName);

            string arguments = processID.ToString() + " \"" + Path.GetDirectoryName(executable) + "\"";
            if (args != null)
                arguments += " " + args.ConcatenateAll();

            Process.Start(updater.FullName, arguments);
        }

        public static string[] HandleTempFolderCleanup(string[] args)
        {
            List<string> result = new List<string>();
            foreach (string arg in args)
                if (arg.ToLower().StartsWith(TempFolderCleanupArgument))
                    RemoveTempFolder(arg);
                else
                    result.Add(arg);
            return result.ToArray();
        }

        private static bool RemoveTempFolder(string arg)
        {
            try
            {
                string folder;
                if (arg.ToLower().StartsWith(TempFolderCleanupArgument))
                    folder = arg.Substring(TempFolderCleanupArgument.Length);
                else
                    return false;
                if (!folder.StartsWith(Path.GetTempPath()))
                    return false;
                DirectoryInfo di = new DirectoryInfo(folder);
                if (di.Exists)
                    di.Delete(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

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

        private bool DownloadFile(string url, string localFile)
        {
            const int BUFFER_SIZE = 2048;
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //request.UseBinary = true;
                //request.UsePassive = true;
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
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                fileStream.Close();
                httpStream.Close();
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private VersionInfo GetCurrentVersion(FileInfo versionFile)
        {
            List<VersionInfo> versions = new List<VersionInfo>();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(versionFile.FullName);
                foreach (XmlNode node in doc.SelectNodes("/versions/version"))
                {
                    string versionStr = node.GetAttributeValue("id", true);
                    string name = node.GetValue("name", "");
                    DateTime releaseDate = node.GetValue("release-date", DateTime.MinValue, "yyyy-MM-dd");
                    string url = node.GetValue("url", "");
                    Version version = Version.Parse(versionStr);
                    if (name.Length == 0 || releaseDate == DateTime.MinValue || url.Length == 0)
                        return null;
                    versions.Add(new VersionInfo(version, name, releaseDate, url));
                }
                VersionInfo newestByVersion = null;
                VersionInfo newestByDate = null;
                foreach (VersionInfo vi in versions)
                {
                    if (newestByVersion == null || vi.Version.IsNewerThan(newestByVersion.Version))
                        newestByVersion = vi;
                    if (newestByDate == null || vi.ReleaseDate.CompareTo(newestByDate.ReleaseDate) > 0)
                        newestByDate = vi;
                }
                if (newestByDate == newestByVersion)
                    return newestByVersion;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
