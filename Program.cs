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
    // TODO Comment
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

            ArgumentResult arguments = CheckArguments(args);
            if (!arguments.IsValid)
            {
                MessageBox.Show(string.Format(TextResources.ArgumentsMissing, GetDisplayCommandLine(args)), 
                                TextResources.ErrorMsgBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            UpdateManifest manifest = null;
            try
            {
                manifest = UpdateManifest.FromFile(arguments.UpdatePath + "\\update.manifest");
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

                Updater updater = new Updater(manifest, arguments.UpdatePath.FullName, updateForm.SetFileName);
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

                Process.Start(arguments.ApplicationPath, arguments.GetAdditionalArgumentsAsString() + " " + 
                                                         Download.UpdateHelper.TempFolderCleanupArgument + arguments.UpdatePath);
                Application.Exit();
            }
        }

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

        private static ArgumentResult CheckArguments(string[] args)
        {
            const int REQUIRED_ARGUMENTS = 3;

            bool ok = true;
            int callingProcessID = -1;
            DirectoryInfo updatePath = null;
            string applicationPath = string.Empty;
            string language = string.Empty;
            int languageIndex = -1;

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
                // Note that if "/lang:xx" is not included, that's OK and language is an empty string
            }

            ok = ok && args.Length >= REQUIRED_ARGUMENTS;
            ok = ok && int.TryParse(args[0], out callingProcessID);
            if (ok)
                try
                {
                    updatePath = new DirectoryInfo(args[1]);
                    ok = updatePath.Exists;
                }
                catch (Exception)
                {
                    ok = false;
                }
            if (ok)
                try
                {
                    FileInfo file = new FileInfo(args[2]);
                    if (file.Exists)
                        applicationPath = args[2];
                    else
                        ok = false;
                }
                catch (Exception)
                {
                    ok = false;
                }
            if (ok)
            { 
                ArgumentResult result = new ArgumentResult(callingProcessID, updatePath, applicationPath, language);

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
