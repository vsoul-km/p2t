using System;
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
        private static WriteLog _writeLog = new WriteLog();
        public Ping(CommandLineArguments commandLineArguments)
        {
            //init variables
            _packetSize = commandLineArguments.PacketSize;
            _pingCount = commandLineArguments.PingCount;
            _pingTimeout = commandLineArguments.PingTimeout;
            _pingRttInterval = commandLineArguments.PingRttInterval;
            _dontFragmentFlag = commandLineArguments.DoNotFragment;
            _followTheName = commandLineArguments.FollowTheName;
        }
        public void StartPing()
        {
            string pingCountText = _pingCount == 0 ? "infinite" : _pingCount.ToString();

            _writeLog.Append("");
            _writeLog.Append("Host Name: " + P2T.Variables.HostName);
            _writeLog.Append("IP Address: " + P2T.Variables.Address);
            _writeLog.Append("Packet Size: " + _packetSize + " bytes");
            _writeLog.Append("Ping Count: " + pingCountText);
            _writeLog.Append("Timeout: " + _pingTimeout + " ms");
            _writeLog.Append("Interval: " + _pingRttInterval + " ms");
            _writeLog.Append("Don't Fragment: " + _dontFragmentFlag);
            _writeLog.Append("Follow the Name: " + _followTheName);
            _writeLog.Append("");

            Console.WriteLine();
            Console.WriteLine("Host Name: " + P2T.Variables.HostName);
            Console.WriteLine("IP Address: " + P2T.Variables.Address);
            Console.WriteLine("Packet Size: " + _packetSize + " bytes");
            Console.WriteLine("Ping Count: " + pingCountText);
            Console.WriteLine("Timeout: " + _pingTimeout + " ms");
            Console.WriteLine("Interval: " + _pingRttInterval + " ms");
            Console.WriteLine("Don't fragment: " + _dontFragmentFlag);
            Console.WriteLine("Follow the Name: " + _followTheName);
            Console.WriteLine();

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
            // Create a buffer of data to be transmitted.
            byte[] sizeOption = new byte[_packetSize];
            new Random().NextBytes(sizeOption);

            // Wait milliseconds for a reply.
            int timeoutOption = _pingTimeout;

            // Set options for transmission:
            // The data can go through 32 gateways or routers
            // before it is destroyed, and the data packet
            // can be fragmented.
            var options = new PingOptions(32, _dontFragmentFlag);

            var pingSender = new System.Net.NetworkInformation.Ping();

            // Send the request.
            try
            {
                var reply = pingSender.Send(P2T.Variables.Address, timeoutOption, sizeOption, options);

                //Console.WriteLine(reply.Status.ToString());

                if (reply != null && reply.Status == IPStatus.PacketTooBig)
                {
                    P2T.Statistic.PingLost++;
                    string output = $"{DateTime.Now:HH:mm:ss.fff} The packet is too big and network does not allow to pass it.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(output);
                    Console.ResetColor();
                    _writeLog.Append(output);

                    return;
                }

                if (reply != null && reply.Status == IPStatus.TimedOut)
                {
                    P2T.Statistic.PingLost++;
                    string output = $"{DateTime.Now:HH:mm:ss.fff} Timeout.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(output);
                    Console.ResetColor();
                    _writeLog.Append(output);

                    P2T.Statistic.TraceRoutes++;
                    Traceroute startTraceroute = new Traceroute();

                    //Start traceroute if ping error
                    startTraceroute.StartTraceroute(timeoutOption);

                    //Console.WriteLine(reply.Status.ToString());

                    return;
                }

                if (reply != null && reply.Status != IPStatus.Success)
                {
                    P2T.Statistic.PingLost++;
                    P2T.Statistic.TraceRoutes++;
                    Traceroute startTraceroute = new Traceroute();

                    //Start traceroute if ping error
                    startTraceroute.StartTraceroute(timeoutOption);

                    //Console.WriteLine(reply.Status.ToString());
                    return;
                }

                if (reply != null && reply.Status == IPStatus.Success)
                {
                    P2T.Statistic.PingSuccess++;
                    P2T.Statistic.RttSumm += reply.RoundtripTime;

                    string ttl = "0";

                    if (reply.Options != null)
                    {
                        if (!string.IsNullOrEmpty(reply.Options.Ttl.ToString()))
                        {
                            ttl = reply.Options.Ttl.ToString();
                        }
                    }

                    if (_followTheName)
                    {
                        P2T.Statistic.addIp = reply.Address.ToString();
                    }

                    string output = $"{DateTime.Now:HH:mm:ss.fff}  Reply from: {reply.Address}  fragment={(!_dontFragmentFlag).ToString()}  bytes={_packetSize.ToString()}  time={reply.RoundtripTime.ToString()}ms  TTL={ttl} ";
                    Console.WriteLine(output);
                    _writeLog.Append(output);
                    return;
                }

                P2T.Statistic.PingLost++;
                string textUnknownError = "Unknown error";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(textUnknownError);
                Console.ResetColor();
                _writeLog.Append(textUnknownError);
            }
            catch (PingException pingException) when (pingException.InnerException is SocketException socketException && socketException.SocketErrorCode == SocketError.HostNotFound)
            {
                //Console.WriteLine(socketException.ToString());
                string textHostNotFound = "Can't resolve the hostname '" + P2T.Variables.Address + "' to an IP address.";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(textHostNotFound);
                Console.ResetColor();
                _writeLog.Append(textHostNotFound);
                System.Threading.Thread.Sleep(_pingTimeout);
            }
            catch (PingException pingException)
            {
                //Console.WriteLine(pingException.ToString());
                string textPingException = "Ping module error exception: " + pingException;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(textPingException);
                Console.ResetColor();
                _writeLog.Append(textPingException);
                System.Threading.Thread.Sleep(_pingTimeout);
            }
        }
    }
}