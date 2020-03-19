using System;
using System.Runtime.InteropServices;
using p2t.Resources;
using p2t.Resources.DataTypes;

namespace p2t
{
    class P2T
    {
        #region GLOBAL VARS
        static HandlerRoutine _consoleHandler;
        #endregion
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);
        // A delegate type to be used as the handler routine for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes ctrlType);
        public static Variables Variables = new Variables();
        public static CommandLineArguments CommandLineArguments = new CommandLineArguments();
        public static Statistic Statistic = new Statistic();

        public enum CtrlTypes
        {
            CtrlCEvent = 0,
            CtrlBreakEvent = 1,
            CtrlCloseEvent = 2,
            CtrlLogoffEvent = 5,
            CtrlShutdownEvent = 6
        }

        static void Main (string[] args)
        {
            if (args.Length == 0)
            {
                DisplayHelp();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Application is running without any arguments. Starting an interactive mode.");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Please enter parameters following this step-by-step procedure:");
                Console.ResetColor();
                Console.WriteLine();

                while (true)
                {
                    Console.Write("Enter the IP address or the hostname to ping: ");
                    string pingAddress = Console.ReadLine();
                    if (string.IsNullOrEmpty(pingAddress)) continue;
                    if (pingAddress.Contains(" "))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong address.");
                        Console.ResetColor();
                        continue;
                    }

                    if (!ValidateAddress.Ip(pingAddress))
                    {
                        if (!ValidateAddress.HostName(pingAddress))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cannot resolve the address to IP address.");
                            Console.ResetColor();
                            continue;
                        }
                    }

                    CommandLineArguments.Address = pingAddress;
                    break;
                }

                while (true)
                {
                    Console.Write("Enter the RTT interval in ms (default is 500 ms): ");
                    string pingInterval = Console.ReadLine();
                    if (string.IsNullOrEmpty(pingInterval)) break;
                    if (int.TryParse(pingInterval, out int result))
                    {
                        CommandLineArguments.PingRttInterval = result;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong RTT interval.");
                        Console.ResetColor();
                        continue;
                    }
                    break;
                }

                while (true)
                {
                    Console.Write("Do you want to log ping to file (y/n)?: ");
                    string enableLogAnswer = Console.ReadLine();
                    if (enableLogAnswer != "y" && enableLogAnswer != "n") continue;
                    if (enableLogAnswer == "y")
                    {
                        CommandLineArguments.LogEnabled = true;
                    }
                    break;
                }
            }

            if (args.Length > 0)
            {
                UtilityArguments arguments = new UtilityArguments(args);

                if (args[0].StartsWith("/?") || args[0].StartsWith("?"))
                {
                    DisplayHelp();
                    Environment.Exit(0);
                }

                if (args[0].StartsWith("-"))
                {
                    DisplayHelp();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Wrong address. Please use the correct address.");
                    Console.ResetColor();
                    Environment.Exit(0);
                }
                else
                {
                    CommandLineArguments.Address = args[0];
                }

                if (!string.IsNullOrEmpty(arguments.L))
                {
                    if (int.TryParse(arguments.L, out int result))
                    {
                        CommandLineArguments.PacketSize = result;
                    }
                    else
                    {
                        DisplayHelp();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error parsing argument -l");
                        Console.WriteLine(" Wrong packet payload size.");
                        Console.ResetColor();
                        Environment.Exit(0);
                    }
                }

                if (!string.IsNullOrEmpty(arguments.C))
                {
                    if (int.TryParse(arguments.C, out int result))
                    {
                        CommandLineArguments.PingCount = result;
                    }
                    else
                    {
                        DisplayHelp();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error parsing argument -c");
                        Console.WriteLine(" Wrong number of ping echo requests.");
                        Console.ResetColor();
                        Environment.Exit(0);
                    }
                }

                if (!string.IsNullOrEmpty(arguments.W))
                {
                    if (int.TryParse(arguments.W, out int result))
                    {
                        CommandLineArguments.PingTimeout = result;
                    }
                    else
                    {
                        DisplayHelp();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error parsing argument -w");
                        Console.WriteLine(" Wrong timeout.");
                        Console.ResetColor();
                        Environment.Exit(0);
                    }
                }

                if (!string.IsNullOrEmpty(arguments.I))
                {
                    if (int.TryParse(arguments.I, out int result))
                    {
                        CommandLineArguments.PingRttInterval = result;
                    }
                    else
                    {
                        DisplayHelp();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error parsing argument -i");
                        Console.WriteLine(" Wrong RTT interval.");
                        Console.ResetColor();
                        Environment.Exit(0);
                    }
                }

                CommandLineArguments.DoNotFragment = arguments.F;
                CommandLineArguments.FollowTheName = arguments.Follow;
                CommandLineArguments.LogEnabled = arguments.Log;
            }

            //validate the address
            //logic: if address is IP address - continue pinging using the IP address;
            //       if address is name - try to resolve it to IP address and continue pinging using the first IP address from the resolved array of IP's;
            //       if both unsuccessful - terminate the program with the error code -1
            bool addressIsIp = ValidateAddress.Ip(CommandLineArguments.Address);

            if (addressIsIp)
            {
                Variables.Address = CommandLineArguments.Address;
                //disable FollowTheName option and set hostname to empty because using an ip address
                CommandLineArguments.FollowTheName = false;
                Variables.HostName = "";
            }
            else
            {
                bool addressIsHostName = ValidateAddress.HostName(CommandLineArguments.Address);

                if (addressIsHostName)
                {
                    Variables.Address = CommandLineArguments.Address;
                    Variables.HostName = CommandLineArguments.Address;

                    //change the global input address (from Command Line arguments) to the ip address => do not allow the ping module to resolve the hostname to an ip address
                    if (!CommandLineArguments.FollowTheName)
                    {
                        string singleIp = ResolveHostname.GetSingleIp(CommandLineArguments.Address);
                        if (!string.IsNullOrEmpty(singleIp))
                        {
                            Variables.Address = singleIp;
                        }
                    }
                }
                else
                {
                    string textHostNotFound = "Can't resolve the hostname '" + CommandLineArguments.Address + "' to an IP address.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(textHostNotFound);
                    Console.ResetColor();
                    Environment.Exit(-1);
                }
            }

            //save a reference so it does not get GC'd
            _consoleHandler = ConsoleCtrlCheck;

            //set handler that will trap exit
            SetConsoleCtrlHandler(_consoleHandler, true);

            Ping ping = new Ping(CommandLineArguments);
            ping.StartPing();

            Result();
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

    private static void DisplayHelp()
        {
            Console.WriteLine("Usage: p2t.exe ipaddress/name -option1 -option2 -option...");
            Console.WriteLine("options:");
            Console.WriteLine("    -l size   Packet payload size, optional. Default is 32 byte.");
            Console.WriteLine("    -c count  Number of ping echo requests, optional. Default is infinite.");
            Console.WriteLine("    -w ms     Timeout in ms. to wait for each reply, optional. Default is 2000 ms.");
            Console.WriteLine("    -i ms     Interval between pings RTT in ms, optional. Default is 500 ms.");
            Console.WriteLine("    -f        Do not fragment. The default is false (disabled).");
            Console.WriteLine("    -log      Write output to log file.");
            Console.WriteLine("    -follow   Follow the hostname. Do not fix IP address when resolving a name for the first time.");
            Console.WriteLine("              The name will resolve to the IP address at every ping. The default is false (disabled).");
            Console.WriteLine("              Only works when using the host name as the address.");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("          p2t.exe 8.8.8.8");
            Console.WriteLine("          p2t.exe www.google.com -log");
            Console.WriteLine("          p2t.exe 8.8.8.8 -l 1500 -c 100 -i 250 -log");
            Console.WriteLine("          p2t.exe www.google.com -l 1500 -c 100 -i 250 -log -follow");
            Console.WriteLine();
        }

        private static void Result()
        {
            WriteLog writeLog = new WriteLog(false);

            Console.WriteLine();
            writeLog.Append("");

            if (Statistic.PingSuccess == 0 && Statistic.PingLost == 0 && Statistic.TraceRoutes == 0)
            {
                string textAllLost = "Packets: sent - 0; Lost - 0%; Traceroutes: 0; Avg.RTT: -- ms; Unique IP addresses: 0;";
                Console.WriteLine(textAllLost);
                if (CommandLineArguments.LogEnabled)
                {
                    writeLog.Append(textAllLost);
                }

                return;
            }

            if (Statistic.PingSuccess == 0 && Statistic.TraceRoutes != 0)
            {
                string textTraceroutesOnly = $"Packets: sent - {Statistic.PingLost}; Lost - 100%; Traceroutes: {Statistic.TraceRoutes}; Avg.RTT: -- ms; Unique IP addresses: 0;";
                Console.WriteLine(textTraceroutesOnly);
                writeLog.Append(textTraceroutesOnly);
                return;
            }

            if (Statistic.PingSuccess == 0 && Statistic.PingLost != 0 && Statistic.TraceRoutes == 0)
            {
                string textLostOnly = $"Packets: sent - {Statistic.PingLost}, lost - 100%; Traceroutes: 0; Avg.RTT: -- ms; Unique IP addresses: 0;";
                Console.WriteLine(textLostOnly);
                writeLog.Append(textLostOnly);
                return;
            }

            string textOther = $"Packets: sent - {Statistic.PingSuccess + Statistic.PingLost}; Lost - {Statistic.PingLost} ({(Statistic.PingLost * 100) / (Statistic.PingSuccess + Statistic.PingLost)}%); Traceroutes: {Statistic.TraceRoutes}; Avg.RTT: {Statistic.RttSumm / Statistic.PingSuccess} ms; Unique IP addresses: {Statistic.GetIp.Count}";
            Console.WriteLine(textOther);
            writeLog.Append(textOther);

            if (CommandLineArguments.FollowTheName)
            {
                Console.WriteLine("Used IP's:");
                writeLog.Append("Used IP's:");
                foreach (string ipAddress in Statistic.GetIp)
                {
                    Console.WriteLine("  " + ipAddress);
                    writeLog.Append("  " + ipAddress);
                }
            }
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CtrlCEvent:
                    Ping.Cancel = true;
                    break;

                case CtrlTypes.CtrlBreakEvent:
                    Ping.Cancel = true;
                    break;

                case CtrlTypes.CtrlCloseEvent:
                    Ping.Cancel = true;
                    Result();
                    break;

                case CtrlTypes.CtrlLogoffEvent:
                    Ping.Cancel = true;
                    Result();
                    break;

                case CtrlTypes.CtrlShutdownEvent:
                    Ping.Cancel = true;
                    break;
            }

            return true;
        }
    }
}