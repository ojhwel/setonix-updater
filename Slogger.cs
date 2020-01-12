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
        Trace = 4
    };

    internal class Slogger
    {
        private string path;

        private string fileName;

        public LogLevel LogLevel = LogLevel.Error;

        internal Slogger(string path, string fileName)
        {
            this.path = path + (!path.EndsWith("\\") ? "\\" : "");
            this.fileName = fileName;
        }

        public void Log(string message, LogLevel logLevel)
        {
            if (logLevel <= LogLevel)
            {
                StreamWriter wr = null;
                try
                {
                    wr = new StreamWriter(GetFileName());
                    wr.WriteLine(DateTime.Now.ToString("hh:mm:ss:fff") + "  " + GetLogLevel(logLevel) + "  " + message);
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
            return path + Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "." + Path.GetExtension(fileName);
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
