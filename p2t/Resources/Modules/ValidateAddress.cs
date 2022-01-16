using System;
using System.Net;

namespace p2t.Resources.Modules
{
    public static class ValidateAddress
    {
        public static bool ValidateIp(string addressToCheck)
        {
            try
            {
                //  Split string by ".", check that array length is 4
                string[] arrOctets = addressToCheck.Split('.');
                if (arrOctets.Length != 4)
                    return false;

                //Check each substring checking that parses to byte

                foreach (string strOctet in arrOctets)
                {
                    if (!byte.TryParse(strOctet, out byte outByte))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool ValidateHostName(string addressToCheck)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(addressToCheck);
                if (!string.IsNullOrEmpty(host.AddressList[0].ToString()))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        public static string GetIp(string addressToCheck)
        {
            if (ValidateHostName(addressToCheck))
            {
                return ResolveHostname.GetSingleIp(addressToCheck);
            }

            return null;
        }
    }
}
