using System;
using System.Collections.Generic;
using System.Net;

namespace p2t.Resources.Modules
{
    public static class ResolveHostname
    {
        public static string GetSingleIp(string addressToResolve)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(addressToResolve);
                if (!String.IsNullOrEmpty(host.AddressList[0].ToString()))
                {
                    return host.AddressList[0].ToString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
        public static List<string> GetAllIp(string addressToResolve)
        {
            List<string> allIp = new List<string>();

            try
            {
                IPHostEntry host = Dns.GetHostEntry(addressToResolve);
                if (!String.IsNullOrEmpty(host.AddressList[0].ToString()))
                {
                    foreach (IPAddress ipAddress in host.AddressList)
                    {
                        allIp.Add(ipAddress.ToString());
                    }
                    return allIp;
                }

                return allIp;
            }
            catch
            {
                return allIp;
            }
        }
    }
}
