﻿using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using p2t.Resources.DataTypes;

namespace p2t.Resources.Modules
{
    class Ping
    {
        public static bool Cancel = false;
        private static int _packetSize;
        private static int _pingCount;
        private static int _pingTimeout;
        private static int _pingRttInterval;
        private static bool _dontFragmentFlag;
        private static bool _followTheName;
        private static bool _addDate;
        private static bool _usingTelegram;
        private static bool _telegramSendAll;
        private static bool _telegramSendErrors;
        private static string _telegramBotToken;
        private static string _telegramChatId;
        private static readonly WriteLog WriteLog = new WriteLog();
        public Ping(CommandLineArguments commandLineArguments)
        {
            //init variables
            _packetSize = commandLineArguments.PacketSize;
            _pingCount = commandLineArguments.PingCount;
            _pingTimeout = commandLineArguments.PingTimeout;
            _pingRttInterval = commandLineArguments.PingRttInterval;
            _dontFragmentFlag = commandLineArguments.DoNotFragment;
            _followTheName = commandLineArguments.FollowTheName;
            _addDate = commandLineArguments.AddDate;
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
            displayHeader += "Host Name: " + P2T.Variables.HostName + "\n";
            displayHeader += "IP Address: " + P2T.Variables.Address + "\n";
            displayHeader += "Packet Size: " + _packetSize + " bytes" + "\n";
            displayHeader += "Ping Count: " + pingCountText + "\n";
            displayHeader += "Timeout: " + _pingTimeout + " ms" + "\n";
            displayHeader += "Interval: " + _pingRttInterval + " ms" + "\n";
            displayHeader += "Don't Fragment: " + _dontFragmentFlag + "\n";
            displayHeader += "Follow the Name: " + _followTheName + "\n";
            displayHeader += "Add Date to each ping output: " + _addDate + "\n";
            displayHeader += "Using Telegram bot to send errors: " + _usingTelegram + "\n";
            //if (!string.IsNullOrEmpty(_telegramBotToken))
            //{
            //    displayHeader += "Telegram bot token: " + _telegramBotToken + "\n";
            //}
            if (!string.IsNullOrEmpty(_telegramChatId))
            {
                displayHeader += "Telegram bot channel id: " + _telegramChatId + "\n";
            }
            if (_telegramSendAll)
            {
                displayHeader += "Telegram, send all: " + _telegramSendAll + "\n";
            }
            if (_telegramSendErrors & _usingTelegram)
            {
                displayHeader += "Telegram, only send errors: " + _telegramSendErrors + "\n";
            }
            //displayHeader += ("\n");

            WriteLog.Append(displayHeader);
            Console.WriteLine(displayHeader);

            if (_usingTelegram)
            {
                Telegram telegram = new Telegram(_telegramBotToken, _telegramChatId);
                telegram.SendMessage(displayHeader);
            }

            //start action
            if (_pingCount == 0)
            {
                while (!Cancel)
                {
                    DoPing();
                    System.Threading.Thread.Sleep(_pingRttInterval);
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
                    DoPing();
                    System.Threading.Thread.Sleep(_pingRttInterval);
                }
            }
        }
        private static void DoPing()
        {
            Telegram telegram = new Telegram(_telegramBotToken, _telegramChatId);

            // Create a buffer of data to be transmitted.
            byte[] sizeOption = new byte[_packetSize];
            new Random().NextBytes(sizeOption);

            // Wait milliseconds for a reply.
            int timeoutOption = _pingTimeout;

            // Set options for transmission:
            // The data can go through 32 gateways or routers
            // before it is destroyed, and the data packet
            // can be fragmented.
            var pingOptions = new PingOptions(32, _dontFragmentFlag);

            var pingSender = new System.Net.NetworkInformation.Ping();

            // Send the request.
            try
            {
                var pingReply = pingSender.Send(P2T.Variables.Address, timeoutOption, sizeOption, pingOptions);

                //Console.WriteLine(reply.Status.ToString());

                if (pingReply != null && pingReply.Status == IPStatus.PacketTooBig)
                {
                    P2T.Statistic.PingLost++;
                    string pingPacketIsToBigMessage = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + " The packet is too big and network does not allow to pass it.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pingPacketIsToBigMessage);
                    Console.ResetColor();
                    WriteLog.Append(pingPacketIsToBigMessage);

                    if (_telegramSendAll || _telegramSendErrors)
                    {
                        telegram.SendMessage(pingPacketIsToBigMessage);
                    }

                    return;
                }

                if (pingReply != null && pingReply.Status == IPStatus.TimedOut)
                {
                    P2T.Statistic.PingLost++;
                    string pingLostText = (_addDate ? $"[{DateTime.Now:(dd-MM-yyyy HH:mm:ss.fff)}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + " Ping " + pingReply.Address + " timeout. Start trace routing.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pingLostText);
                    Console.ResetColor();
                    WriteLog.Append(pingLostText);

                    P2T.Statistic.TraceRoutes++;
                    Traceroute startTraceroute = new Traceroute();

                    //Start traceroute if ping error
                    startTraceroute.StartTraceroute(timeoutOption);

                    //Console.WriteLine(reply.Status.ToString());

                    if (_telegramSendAll || _telegramSendErrors)
                    {
                        telegram.SendMessage(pingLostText);
                    }

                    return;
                }

                if (pingReply != null && pingReply.Status != IPStatus.Success)
                {
                    P2T.Statistic.PingLost++;
                    string pingLostText = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + "Ping " + pingReply.Address + " is lost. Start tracert.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pingLostText);
                    Console.ResetColor();
                    WriteLog.Append(pingLostText);

                    P2T.Statistic.TraceRoutes++;
                    Traceroute startTraceroute = new Traceroute();

                    //Start traceroute if ping error
                    startTraceroute.StartTraceroute(timeoutOption);

                    //Console.WriteLine(reply.Status.ToString());

                    if (_telegramSendAll || _telegramSendErrors)
                    {
                        telegram.SendMessage(pingLostText);
                    }

                    return;
                }

                if (pingReply != null && pingReply.Status == IPStatus.Success)
                {
                    P2T.Statistic.PingSuccess++;
                    P2T.Statistic.RttSumm += pingReply.RoundtripTime;

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
                        P2T.Statistic.AddIp = pingReply.Address.ToString();
                    }

                    string pingSuccessText = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + "  Reply from: " + pingReply.Address + "  fragment=" + (!_dontFragmentFlag).ToString() + "  bytes=" + _packetSize.ToString() + "  time=" + pingReply.RoundtripTime.ToString() + "ms  TTL=" + ttl;
                    Console.WriteLine(pingSuccessText);
                    WriteLog.Append(pingSuccessText);

                    if (_telegramSendAll)
                    {
                        telegram.SendMessage(pingSuccessText);
                    }

                    return;
                }

                P2T.Statistic.PingLost++;
                string гnknownErrorText = "An unknown error occurred...";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(гnknownErrorText);
                Console.ResetColor();
                WriteLog.Append(гnknownErrorText);

                if (_telegramSendAll || _telegramSendErrors)
                {
                    telegram.SendMessage(гnknownErrorText);
                }

            }
            catch (PingException pingException) when (pingException.InnerException is SocketException socketException && socketException.SocketErrorCode == SocketError.HostNotFound)
            {
                //Console.WriteLine(socketException.ToString());
                string textHostNotFound = (_addDate ? $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff}]" : $"[{DateTime.Now:HH:mm:ss.fff}]") + " Can't resolve the hostname '" + P2T.Variables.Address + "' to an IP address.";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(textHostNotFound);
                Console.ResetColor();
                WriteLog.Append(textHostNotFound);

                if (_telegramSendAll || _telegramSendErrors)
                {
                    telegram.SendMessage(textHostNotFound);
                }

                System.Threading.Thread.Sleep(_pingTimeout);
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

                System.Threading.Thread.Sleep(_pingTimeout);
            }
        }
    }
}