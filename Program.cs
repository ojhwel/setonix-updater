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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check command line arguments
            ArgumentResult arguments = CheckArguments(args);
            if (!arguments.IsValid)
            {
                MessageBox.Show(string.Format(TextResources.ArgumentsMissing, GetDisplayCommandLine(args)), 
                                TextResources.ErrorMsgBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Read the manifest
            string updatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            UpdateManifest manifest = null;
            try
            {
                manifest = UpdateManifest.FromFile(updatePath + "\\update.manifest");
            }
            catch (Exception)
            {
                MessageBox.Show(TextResources.InvalidManifest, TextResources.ErrorMsgBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            using (UpdateForm updateForm = new UpdateForm())
            {
                updateForm.Show(manifest.Strings["title"], manifest.Strings["wait"], manifest.Tasks.Count);
                updateForm.SetFileName(TextResources.Preparing);

                // Make sure the application has quit
                try
                {
                    Process appProcess = Process.GetProcessById(arguments.CallingProcessID);
                    appProcess.CloseMainWindow();
                    while (!appProcess.HasExited)
                    {
                        if (!appProcess.WaitForExit(2500))
                        {
                            string message = string.Format(TextResources.PleaseClose, manifest.Strings["appname"]);
                            DialogResult dialogResult = MessageBox.Show(message, TextResources.ErrorMsgBoxTitle, MessageBoxButtons.RetryCancel);
                            if (dialogResult == DialogResult.Cancel)
                            {
                                MessageBox.Show(TextResources.Abort, TextResources.ErrorMsgBoxTitle);
                                Application.Exit();
                                return;
                            }
                        }
                    }
                }
                catch (ArgumentException)
                { 
                    // Nothing to do (the process was not found, which means the application is closed)
                }

                // Perform the update
                Updater updater = new Updater(manifest.Tasks, updatePath, Path.GetDirectoryName(arguments.ApplicationPath), updateForm.SetFileName);
                try
                {
                    updater.PerformUpdates();
                }
                catch (UpdateException e)
                {
                    MessageBox.Show(e.Message, TextResources.ErrorMsgBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                // Restart the application (our work here is done)
                Process.Start(arguments.ApplicationPath, arguments.GetAdditionalArgumentsAsString() + " " + 
                                                         Download.UpdateHelper.TempFolderCleanupArgument + updatePath);
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
                for (int i = 0; i < args.Length; i++)
                    if (args[i].Length >= 8 && (args[i][0] == '/' || args[i][0] == '-') && args[i].Substring(1, 5).ToLower() == "lang:")
                    {
                        language = args[i].Substring(6);
                        try
                        {
                            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language);
                            languageIndex = i;
                        }
                        catch (Exception)
                        {
                            ok = false;
                        }
                        break;
                    }
            }

            ok = ok && args.Length >= REQUIRED_ARGUMENTS;

            // Application path
            if (ok)
                try
                {
                    FileInfo file = new FileInfo(args[1]);
                    if (file.Exists)
                        applicationPath = args[1];
                    else
                        ok = false;
                }
                catch (Exception)
                {
                    ok = false;
                }

            // Return result
            if (ok)
            { 
                ArgumentResult result = new ArgumentResult(callingProcessID, applicationPath, language);

                // Additional argument passthrough
                int addlArgsCount = args.Length - REQUIRED_ARGUMENTS - (languageIndex >= 0 ? 1 : 0);
                if (addlArgsCount > 0)
                { 
                    int a = 0;
                    string[] addlArgs = new string[addlArgsCount];
                    for (int i = 0; i < args.Length; i++)
                        if (i >= REQUIRED_ARGUMENTS && i != languageIndex)
                            addlArgs[a++] = args[i];
                    result.AdditionalArguments = addlArgs;
                }
                return result;
            }
            else
                return ArgumentResult.Invalid;
        }
    }
}
