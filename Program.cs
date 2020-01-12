using SetonixUpdater.Extensions;
using SetonixUpdater.Manifest;
using SetonixUpdater.Resources;
using SetonixUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetonixUpdater
{
    /// <summary>
    /// The main class.
    /// </summary>
    static class Program
    {
        internal static Slogger Logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger = new Slogger(Path.GetTempPath(), "setonix_updater.log");

            // Check command line arguments
            ArgumentResult arguments = CheckArguments(args);
            if (!arguments.IsValid)
            {
                MessageBox.Show(string.Format(TextResources.ArgumentsMissing, GetDisplayCommandLine(args)),
                                TextResources.ErrorMsgBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            else
                Logger.Debug("Command line valid");

            string updatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.Trace("Preparing to read manifest");
            UpdateManifest manifest = null;
            try
            {
                manifest = UpdateManifest.FromFile(updatePath + "\\update.manifest");
                Logger.Trace("Manifest read");
            }
            catch (Exception e)
            {
                Logger.Error("Cannot read manifest: " + e.Message);
                MessageBox.Show(TextResources.InvalidManifest, TextResources.ErrorMsgBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            using (UpdateForm updateForm = new UpdateForm())
            {
                updateForm.Show(manifest.Strings["title"], manifest.Strings["wait"], manifest.Tasks.Count);
                Logger.Trace("Form shown");
                updateForm.SetFileName(TextResources.Preparing);
                Logger.Trace("'Preparing' shown");

                // Make sure the application has quit
                try
                {
                    Process appProcess = Process.GetProcessById(arguments.CallingProcessID);
                    Logger.Debug("Got process for ID " + arguments.CallingProcessID);
                    appProcess.CloseMainWindow();
                    while (!appProcess.HasExited)
                    {
                        if (!appProcess.WaitForExit(2500))
                        {
                            Logger.Debug("Process has not exited");
                            string message = string.Format(TextResources.PleaseClose, manifest.Strings["appname"]);
                            DialogResult dialogResult = MessageBox.Show(message, TextResources.ErrorMsgBoxTitle, MessageBoxButtons.RetryCancel);
                            if (dialogResult == DialogResult.Cancel)
                            {
                                Logger.Info("User cancelled");
                                MessageBox.Show(TextResources.Abort, TextResources.ErrorMsgBoxTitle);
                                Application.Exit();
                                return;
                            }
                            else
                                Logger.Debug("User waits");
                        }
                    }
                }
                catch (ArgumentException)
                {
                    // Nothing to do (the process was not found, which means the application is closed)
                    Logger.Trace("No process for ID " + arguments.CallingProcessID);
                }

                // Perform the update
                Logger.Trace("Creating updater");
                Updater updater = new Updater(manifest.Tasks, updatePath, Path.GetDirectoryName(arguments.ApplicationPath), updateForm.SetFileName);
                Logger.Trace("Updater created");
                try
                {
                    Logger.Trace("Performing updates");
                    updater.PerformUpdates();
                    Logger.Trace("Updates performed");
                }
                catch (UpdateException e)
                {
                    Logger.Error("Update error: " + e.Message);
                    MessageBox.Show(e.Message, TextResources.ErrorMsgBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                // Restart the application (our work here is done)
                string commandLine = arguments.GetAdditionalArgumentsAsString() + " " + Download.UpdateHelper.TempFolderCleanupArgument + updatePath;
                Logger.Info("Restarting application: " + arguments.ApplicationPath + " - " + commandLine);
                try
                {
                    Process.Start(arguments.ApplicationPath, commandLine);
                }
                catch (Exception e)
                {
                    Logger.Error("Unable to start application: " + e.Message);
                }
                Application.Exit();
            }
        }

        /// <summary>
        /// Returns the command line as a single string for display in an error message, i.e. return an "empty command line" text if the command line is empty.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        private static string GetDisplayCommandLine(string[] args)
        {
            bool anyText = false;
            foreach (string arg in args)
                if (!string.IsNullOrWhiteSpace(arg))
                {
                    anyText = true;
                    break;
                }
            if (anyText)
                return args.ConcatenateAll();
            else
                return TextResources.EmptyCommandLine;
        }

        /// <summary>
        /// Checks the command line arguments.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A new <c>ArgumentResult</c> instance.</returns>
        private static ArgumentResult CheckArguments(string[] args)
        {
            const int REQUIRED_ARGUMENTS = 2;

            bool ok = true;
            int callingProcessID = -1;
            string applicationPath = string.Empty;
            string language = string.Empty;
            int languageIndex = -1;

            // Find the optional language argument
            if (ok)
            {
                Logger.Trace("Looking for language argument");
                for (int i = 0; i < args.Length; i++)
                    if (args[i].Length >= 8 && (args[i][0] == '/' || args[i][0] == '-') && args[i].Substring(1, 5).ToLower() == "lang:")
                    {
                        language = args[i].Substring(6);
                        Logger.Debug("Found language argument \"" + args[i] + "\" in position " + i);
                        try
                        {
                            Logger.Trace("Language: " + language);
                            CultureInfo culture = CultureInfo.GetCultureInfo(language);
                            Logger.Debug("Found culture " + culture.ToString());
                            Logger.Trace("Setting UI culture");
                            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                            languageIndex = i;
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error getting culture for " + language);
                            ok = false;
                        }
                        break;
                    }
            }

            ok = ok && args.Length >= REQUIRED_ARGUMENTS;
            Logger.Debug("After required arguments check: " + ok);

            // Process ID
            ok = ok && int.TryParse(args[0], out callingProcessID);
            Logger.Debug("After process ID: " + ok);

            // Application path
            if (ok)
                try
                {
                    Logger.Trace("Getting application path");
                    FileInfo file = new FileInfo(args[1]);
                    Logger.Debug("Application file: " + file.FullName);
                    if (file.Exists)
                    {
                        applicationPath = args[1];
                        Logger.Debug("Application file exists");
                    }
                    else
                    {
                        ok = false;
                        Logger.Debug("Application file doesn't exist");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Error checking application path: " + e.Message);
                    ok = false;
                }

            // Return result
            if (ok)
            {
                Logger.Trace("Creating result");
                ArgumentResult result = new ArgumentResult(callingProcessID, applicationPath, language);

                // Additional argument passthrough
                Logger.Trace("Getting additional arguments");
                int addlArgsCount = args.Length - REQUIRED_ARGUMENTS - (languageIndex >= 0 ? 1 : 0);
                Logger.Debug(addlArgsCount + " additional arguments");
                if (addlArgsCount > 0)
                {
                    int a = 0;
                    string[] addlArgs = new string[addlArgsCount];
                    for (int i = 0; i < args.Length; i++)
                        if (i >= REQUIRED_ARGUMENTS && i != languageIndex)
                            addlArgs[a++] = args[i];
                    result.AdditionalArguments = addlArgs;
                    Logger.Debug("Additional arguments: " + addlArgs.ConcatenateAll());
                }
                return result;
            }
            else
            {
                Logger.Debug("Returning ArgumentResult.Invalid");
                return ArgumentResult.Invalid;
            }
        }
    }
}
