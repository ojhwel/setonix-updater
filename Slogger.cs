using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetonixUpdater
{
    internal enum LogLevel 
    { 
        Error = 0, 
        Warn = 1, 
        Info = 2, 
        Debug = 3,
        Trace = 4,
        Undefined = 99
    };

    internal class Slogger
    {
        private string path;

        private string fileName;

        public LogLevel LogLevel = LogLevel.Undefined;

        internal Slogger(string path, string fileName)
        {
            this.path = path + (!path.EndsWith("\\") ? "\\" : "");
            this.fileName = fileName;
        }

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

        public void Error(string message) => Log(message, LogLevel.Error);
        public void Warn(string message) => Log(message, LogLevel.Warn);
        public void Info(string message) => Log(message, LogLevel.Info);
        public void Debug(string message) => Log(message, LogLevel.Debug);
        public void Trace(string message) => Log(message, LogLevel.Trace);


        private string GetFileName()
        {
            return path + Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("yyyy-MM-dd") + Path.GetExtension(fileName);
        }

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

        private LogLevel ParseLogLevel(string text)
        {
            switch (text.Trim().ToLower())
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
                    return "WHAT?";
            }
        }
    }
}
