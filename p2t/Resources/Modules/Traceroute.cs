using System;
using System.Net.NetworkInformation;

namespace p2t.Resources.Modules
{
    class Traceroute
    {
        public static bool Cancel = false;
        public void StartTraceroute(string ipAddress, int timeout)
        {
            WriteLog writeLog = new WriteLog(false);
            const int maxTtl = 16;
            int timeoutOption = timeout;
            byte[] sizeOption = new byte[32];
            new Random().NextBytes(sizeOption);

            var pingSender = new System.Net.NetworkInformation.Ping();
            string startText = $"Start traceroute at {DateTime.Now.ToString("HH:mm:ss.fff")}";
            Console.WriteLine(startText);
            writeLog.Append(startText);
            
            for (int ttl = 1; ttl <= maxTtl; ttl++)
            {
                if (Cancel)
                {
                    break;
                }

                PingOptions options = new PingOptions(ttl, true);
                PingReply reply = pingSender.Send(ipAddress, timeoutOption, sizeOption, options);

                if (reply != null && reply.Status == IPStatus.TtlExpired)
                {
                    // TtlExpired means we've found an address, but there are more addresses
                    string replyAddress = $"  {reply.Address}";
                    Console.WriteLine(replyAddress);
                    writeLog.Append(replyAddress);
                    continue;
                }

                if (reply != null && reply.Status == IPStatus.TimedOut)
                {
                    // TimedOut means this ttl is no good, we should continue searching
                    string replyNothing = "  *";
                    Console.WriteLine(replyNothing);
                    writeLog.Append(replyNothing);
                    continue;
                }

                if (reply != null && reply.Status == IPStatus.Success)
                {
                    // Success means the traceroute has completed
                    string endText = $"Finished traceroute at {DateTime.Now.ToString("HH:mm:ss.fff")}";
                    Console.WriteLine(endText);
                    writeLog.Append(endText);
                }

                // if we ever reach here, we're finished, so break

                if (reply != null && reply.Status != IPStatus.Success)
                {
                    string endAlternative = $"  {reply.Status.ToString()}";
                    Console.WriteLine(endAlternative);
                    writeLog.Append(endAlternative);
                }

                break;
            }
        }
    }
}
