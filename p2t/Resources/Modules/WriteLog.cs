using System;
using System.IO;

namespace p2t.Resources.Modules
{
    class WriteLog
    {
        private readonly string _logPath;
        public WriteLog(bool printLogPath = true)
        {
            if (P2T.CommandLineArguments.LogEnabled)
            {
                if (!string.IsNullOrEmpty(P2T.Variables.Address))
                {
                    _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "p2t_" + P2T.Variables.LogStartTime + "_" + P2T.Variables.Address + ".log");
                }
                else
                {
                    _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "p2t_" + P2T.Variables.LogStartTime + ".log");
                }

                try
                {
                    File.AppendAllText(_logPath, "" + Environment.NewLine);
                    if (printLogPath)
                    {
                        Console.WriteLine($"Using the following file to log: {_logPath}");
                    }
                }
                catch
                {
                    Console.WriteLine($"Can't write log to file {_logPath}.");
                    _logPath = Path.Combine(Path.GetTempPath(), "p2t_" + P2T.Variables.LogStartTime + ".log");
                    Console.WriteLine($"Using the following file to log: {_logPath}");
                }
            }
        }

        public void Append(string text)
        {
            if (P2T.CommandLineArguments.LogEnabled)
            {
                try
                {
                    File.AppendAllText(_logPath, text + Environment.NewLine);
                }
                catch
                {
                    Console.WriteLine($"Can't write log to file {_logPath}");
                }
            }
        }
    }
}