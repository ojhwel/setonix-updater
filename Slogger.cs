using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater
{
    /// <summary>
    /// 
    /// </summary>
    internal enum LogLevel 
    { 
        None = -1,
        Error = 0, 
        Warn = 1, 
        Info = 2, 
        Debug = 3,
        Trace = 4,
        Undefined = 99
    };

    /// <summary>
    /// A simple logger with only .NET Framework dependencies.
    /// </summary>
    internal class Slogger
    {
        /// <summary>
        /// The path to write the log to.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// The log file name.
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// The current log level.
        /// <para/>
        /// Default value: Error, unless there is a "setonix_loglevel.&lt;loglevel&gt;" in the system temp folder which.
        /// </summary>
        public LogLevel LogLevel = LogLevel.Undefined;

        /// <summary>
        /// Creates a new Slogger instance.
        /// </summary>
        /// <param name="path">The path to write the log to.</param>
        /// <param name="fileName">The log file name.</param>
        internal Slogger(string path, string fileName)
        {
            this.path = path + (!path.EndsWith("\\") ? "\\" : "");
            this.fileName = fileName;
        }

        /// <summary>
        /// Writes a log message at the specified log level, depending on the current <c>LogLevel</c>.
        /// <para/>
        /// Please note the more convenient logging methods <see cref="Error(string)"/>, <see cref="Warn(string)"/>, <see cref="Info(string)"/>,
        /// <see cref="Debug(string)"/>, and <see cref="Trace(string)"/>.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="logLevel">The log level. THe message is only written if the current <c>LogLevel</c> is at least this.</param>
        public void Log(string message, LogLevel logLevel)
        {
            if (LogLevel == LogLevel.Undefined)
                AutoDetermineLogLevel();

            if (logLevel <= LogLevel)
            {
                StreamWriter wr = null;
                try
                {
                    wr = new StreamWriter(GetFileName(), true);
                    wr.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff") + "  " + GetLogLevel(logLevel) + "  " + message);
                    wr.Flush();
                }
                catch (Exception)
                { }
                finally
                {
                    wr?.Close();
                }
            }
        }

        /// <summary>
        /// Writes an Error log message.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Error(string message) => Log(message, LogLevel.Error);

        /// <summary>
        /// Write a Warning log message.
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message) => Log(message, LogLevel.Warn);

        /// <summary>
        /// Writes an Info log message.
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message) => Log(message, LogLevel.Info);
        
        /// <summary>
        /// Writes a Debug log message.
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message) => Log(message, LogLevel.Debug);
        
        /// <summary>
        /// Writes a Trace log message.
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message) => Log(message, LogLevel.Trace);

        #region Private methods

        /// <summary>
        /// Creates the log file name based on the current date.
        /// </summary>
        /// <returns></returns>
        private string GetFileName()
        {
            return path + Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("yyyy-MM-dd") + Path.GetExtension(fileName);
        }

        /// <summary>
        /// Sets the <c>LogLevel</c> based on the presence of a log level file in the system temp folder. If no such file is present, Error is set. If multiple
        /// such files exist, the most verbose one is set.
        /// </summary>
        private void AutoDetermineLogLevel()
        {
            DirectoryInfo tempDir = new DirectoryInfo(Path.GetTempPath());
            LogLevel logLevel = LogLevel.Error;
            foreach (FileInfo file in tempDir.GetFiles("setonix_loglevel.*"))
            {
                LogLevel l = ParseLogLevel(Path.GetExtension(file.Name));
                if (l != LogLevel.Undefined && l > logLevel)
                    logLevel = l;
            }
            LogLevel = logLevel;
        }

        /// <summary>
        /// Translates the file extension into a <c>LogLevel</c>.
        /// </summary>
        /// <param name="extension">The log level file extension.</param>
        /// <returns>The log level repesenting the file extension, or <c>LogLevel.Undefined</c> if the extenstion is not an expected value.</returns>
        private LogLevel ParseLogLevel(string extension)
        {
            switch (extension.Trim().ToLower())
            {
                case ".error":
                    return LogLevel.Error;
                case ".warn":
                    return LogLevel.Warn;
                case ".info":
                    return LogLevel.Info;
                case ".debug":
                    return LogLevel.Debug;
                case ".trace":
                    return LogLevel.Trace;
                default:
                    return LogLevel.Undefined;
            }
            
        }

        /// <summary>
        /// Translates the log level into a string to write into the log file.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        private string GetLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Error:
                    return "ERROR";
                case LogLevel.Warn:
                    return "WARN ";
                case LogLevel.Info:
                    return "INFO ";
                case LogLevel.Debug:
                    return "DEBUG";
                case LogLevel.Trace:
                    return "TRACE";
                default:
                    return "XXXXX";
            }
        }

        #endregion
    }
}
