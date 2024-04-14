using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using p2t.Resources.DataTypes;

namespace p2t.Resources.Modules
{
    class Ping
    {
        public static bool Cancel = false;
        private static string _ipAddress;
        private static string _originalAddress;
        private static bool _isAddress;
        private static int _packetSize;
        private static int _pingCount;
        private static int _pingTimeout;
        private static int _pingRttInterval;
        private static bool _dontFragmentFlag;
        private static bool _followTheName;
        private static bool _addDate;
        private static bool _noTrace;
        private static bool _errorsOnly;
        private static bool _usingTelegram;
        private static bool _telegramSendAll;
        private static bool _telegramSendErrors;
        private static string _telegramBotToken;
        private static string _telegramChatId;
        private static readonly WriteLog WriteLog = new WriteLog();
        public Ping(CommandLineArguments commandLineArguments)
        {
            //init variables
            _ipAddress = P2T.GlobalVariables.Address;
            _originalAddress = P2T.GlobalVariables.OriginalAddress;
            _isAddress = commandLineArguments.IsAddress;
            _packetSize = commandLineArguments.PacketSize;
            _pingCount = commandLineArguments.PingCount;
            _pingTimeout = commandLineArguments.PingTimeout;
            _pingRttInterval = commandLineArguments.PingRttInterval;
            _dontFragmentFlag = commandLineArguments.DoNotFragment;
            _followTheName = commandLineArguments.FollowTheName;
            _addDate = commandLineArguments.AddDate;
            _errorsOnly = commandLineArguments.ErrorsOnly;
            _noTrace = commandLineArguments.NoTrace;
            _usingTelegram = commandLineArguments.UsingTelegram;
            _telegramBotToken = commandLineArguments.TelegramBotToken;
            _telegramChatId = commandLineArguments.TelegramChatId;
            _telegramSendAll = commandLineArguments.TelegramSendAll;
            _telegramSendErrors = commandLineArguments.TelegramSendErrors;
        }
        public void StartPing()
        {
            string pingCountText = _pingCount == 0 ? "infinite" : _pingCount.ToString();
            System.Reflection.Assembly assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly();
            
            //Preparing the output header
            string displayHeader = "\n";

            displayHeader += "p2t.exe v" + assemblyVersion.GetName().Version + "\n";
            displayHeader += "Ping started. Used options:" + "\n";
            displayHeader += " Address: " + _originalAddress + "\n";
            displayHeader += " Is address: " + _isAddress + "\n";
            displayHeader += " Packet Size: " + _packetSize + " bytes" + "\n";
            displayHeader += " Ping Count: " + pingCountText + "\n";
            displayHeader += " Timeout: " + _pingTimeout + " ms" + "\n";
            displayHeader += " Interval: " + _pingRttInterval + " ms" + "\n";
            displayHeader += " Don't Fragment: " + _dontFragmentFlag + "\n";
            displayHeader += " Follow the Name: " + _followTheName + "\n";
            displayHeader += " Add date to each ping output: " + _addDate + "\n";
            displayHeader += " Errors only: " + _errorsOnly + "\n";
            displayHeader += " No trace: " + _noTrace + "\n";
            displayHeader += " Using Telegram bot to send errors: " + _usingTelegram + "\n";
            
            if (_errorsOnly)
            {
                displayHeader += "\n";
                displayHeader += " *Error mode is enabled. Only errors will be displayed and logged!" + "\n";
            }

            //if (!string.IsNullOrEmpty(_telegramBotToken))
            //{
            //    displayHeader += "Telegram bot token: " + _telegramBotToken + "\n";
            //}
            if (!string.IsNullOrEmpty(_telegramChatId))
            {
                displayHeader += " Telegram bot channel id: " + _telegramChatId + "\n";
            }
            if (_telegramSendAll)
            {
                displayHeader += " Telegram, send all: " + _telegramSendAll + "\n";
            }
            if (_telegramSendErrors & _usingTelegram)
            {
                displayHeader += " Telegram, only send errors: " + _telegramSendErrors + "\n";
            }
            //displayHeader += ("\n");

            WriteLog.Append(displayHeader);
            Console.WriteLine(displayHeader);

            if (_usingTelegram)
            {
                Telegram telegram = new Telegram(_telegramBotToken, _telegramChatId);
                telegram.SendMessage(displayHeader);
            }

            var pingObject = new System.Net.NetworkInformation.Ping();

            //start action
            if (_pingCount == 0)
            {
                while (!Cancel)
                {
                    Stopwatch stopWatchPingDuration = new Stopwatch();

                    stopWatchPingDuration.Start();

                    var pingReply = SendPing(pingObject);

                    stopWatchPingDuration.Stop();

                    if (pingReply.Status != IPStatus.Success && (int)stopWatchPingDuration.ElapsedMilliseconds < _pingTimeout)
                    {
                        System.Threading.Thread.Sleep(_pingTimeout - (int)stopWatchPingDuration.ElapsedMilliseconds);
                        ResultsProcessing(pingReply);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(_pingRttInterval - (int)stopWatchPingDuration.ElapsedMilliseconds);
                        ResultsProcessing(pingReply);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _pingCount; i++)
                {
                    if (Cancel)
                    {
                        break;
                    }

                    Stopwatch stopWatchPingDuration = new Stopwatch();

                    stopWatchPingDuration.Start();

                    var pingReply = SendPing(pingObject);

                    stopWatchPingDuration.Stop();

                    if (pingReply.Status != IPStatus.Success && (int)stopWatchPingDuration.ElapsedMilliseconds < _pingTimeout)
                    {
                        System.Threading.Thread.Sleep(_pingTimeout - (int)stopWatchPingDuration.ElapsedMilliseconds);
                        ResultsProcessing(pingReply);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(_pingRttInterval - (int)stopWatchPingDuration.ElapsedMilliseconds);
                        ResultsProcessing(pingReply);
                    }
                }
            }
        }

        private static PingReply SendPing(System.Net.NetworkInformation.Ping pingObject)
        {
            // Create a buffer of data to be transmitted.
            byte[] sizeOption = new byte[_packetSize];

            new Random().NextBytes(sizeOption);

            // Set options for transmission:
            // The data can go through 32 gateways or routers
            // before it is destroyed, and the data packet
            // can be fragmented.
            var pingOptions = new PingOptions(16, _dontFragmentFlag);

            try
            {
                return pingObject.Send(_ipAddress, _pingTimeout, sizeOption, pingOptions);

            }
            catch
            {
                return null;
            }
        }

        private static void ResultsProcessing(PingReply pingReply)
        {
            Telegram telegram = new Telegram(_telegramBotToken, _telegramChatId);

            try
            {
                if (_followTheName & !_isAddress)
                {
                    if (!string.IsNullOrEmpty(AddressValidator.GetIp(P2T.GlobalVariables.AddressHostName)))
                    {
                        _ipAddress = AddressValidator.GetIp(P2T.GlobalVariables.AddressHostName);
                    }
                }

                //Console.WriteLine(reply.Status.ToString());

                if (pingReply != null && pingReply.Status == IPStatus.PacketTooBig)
                {
                    P2T.Statistics.PingLost++;
                    string pingPacketIsToBigMessage = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + " The packet is too big and network does not allow to pass it.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pingPacketIsToBigMessage);
                    Console.ResetColor();
                    WriteLog.Append(pingPacketIsToBigMessage);

                    if (_followTheName)
                    {
                        P2T.Statistics.AddIp = _ipAddress;
                    }

                    if (_telegramSendAll || _telegramSendErrors)
                    {
                        telegram.SendMessage(pingPacketIsToBigMessage);
                    }

                    return;
                }

                if (pingReply != null && pingReply.Status == IPStatus.TimedOut)
                {
                    P2T.Statistics.PingLost++;
                    string pingLostText = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + " Ping " + _ipAddress + " timeout.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pingLostText);
                    Console.ResetColor();
                    WriteLog.Append(pingLostText);

                    if (!_noTrace)
                    {
                        P2T.Statistics.TraceRoutes++;
                        Traceroute startTraceroute = new Traceroute();

                        //Start traceroute if ping error
                        startTraceroute.StartTraceroute(_ipAddress, _pingTimeout);
                    }

                    if (_followTheName)
                    {
                        P2T.Statistics.AddIp = _ipAddress;
                    }

                    //Console.WriteLine(reply.Status.ToString());
                    if (_telegramSendAll || _telegramSendErrors)
                    {
                        telegram.SendMessage(pingLostText);
                    }

                    return;
                }

                if (pingReply != null && pingReply.Status != IPStatus.TimedOut && pingReply.Status != IPStatus.Success)
                {
                    P2T.Statistics.PingLost++;
                    string pingLostText = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + " Ping " + _ipAddress + " timeout (" + pingReply.Status.ToString() + ").";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pingLostText);
                    Console.ResetColor();
                    WriteLog.Append(pingLostText);

                    if (!_noTrace)
                    {
                        P2T.Statistics.TraceRoutes++;
                        Traceroute startTraceroute = new Traceroute();

                        //Start traceroute if ping error
                        startTraceroute.StartTraceroute(_ipAddress, _pingTimeout);
                    }

                    if (_followTheName)
                    {
                        P2T.Statistics.AddIp = _ipAddress;
                    }

                    //Console.WriteLine(reply.Status.ToString());
                    if (_telegramSendAll || _telegramSendErrors)
                    {
                        telegram.SendMessage(pingLostText);
                    }

                    return;
                }

                if (pingReply != null && pingReply.Status == IPStatus.Success)
                {
                    P2T.Statistics.PingSuccess++;
                    P2T.Statistics.RttSumm += pingReply.RoundtripTime;

                    string ttl = "0";

                    if (pingReply.Options != null)
                    {
                        if (!string.IsNullOrEmpty(pingReply.Options.Ttl.ToString()))
                        {
                            ttl = pingReply.Options.Ttl.ToString();
                        }
                    }

                    if (_followTheName)
                    {
                        P2T.Statistics.AddIp = _ipAddress;
                    }

                    string pingSuccessText = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + "  Reply from: " + _ipAddress + "  fragment=" + (!_dontFragmentFlag).ToString() + "  bytes=" + _packetSize.ToString() + "  time=" + pingReply.RoundtripTime.ToString() + "ms  TTL=" + ttl;
                    
                    if (!_errorsOnly)
                    {
                        Console.WriteLine(pingSuccessText);
                        WriteLog.Append(pingSuccessText);
                    }

                    if (_telegramSendAll)
                    {
                        telegram.SendMessage(pingSuccessText);
                    }

                    return;
                }

                P2T.Statistics.PingLost++;
                string unknownErrorText = "An unknown error occurred...";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(unknownErrorText);
                Console.ResetColor();
                WriteLog.Append(unknownErrorText);

                if (_telegramSendAll || _telegramSendErrors)
                {
                    telegram.SendMessage(unknownErrorText);
                }

            }
            catch (PingException pingException) when (pingException.InnerException is SocketException socketException && socketException.SocketErrorCode == SocketError.HostNotFound)
            {
                //Console.WriteLine(socketException.ToString());
                string textHostNotFound = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + " Can't resolve the hostname '" + _ipAddress + "' to an IP address.";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(textHostNotFound);
                Console.ResetColor();
                WriteLog.Append(textHostNotFound);

                if (_telegramSendAll || _telegramSendErrors)
                {
                    telegram.SendMessage(textHostNotFound);
                }
            }
            catch (PingException pingException)
            {
                //Console.WriteLine(pingException.ToString());
                string pingModuleException = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + " Ping module error exception: " + pingException;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(pingModuleException);
                Console.ResetColor();
                WriteLog.Append(pingModuleException);

                if (_telegramSendAll || _telegramSendErrors)
                {
                    telegram.SendMessage(pingModuleException);
                }
            }
        }
    }
}