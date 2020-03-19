using System;
using System.Net;

namespace p2t.Resources
{
    public static class ValidateAddress
    {
        public static bool Ip(string addressToCheck)
        {
            try
            {
                //  Split string by ".", check that array length is 4
                string[] arrOctets = addressToCheck.Split('.');
                if (arrOctets.Length != 4)
                    return false;

                //Check each substring checking that parses to byte
                byte outByte = 0;
                foreach (string strOctet in arrOctets)
                {
                    if (!byte.TryParse(strOctet, out outByte))
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

        public static bool HostName(string addressToCheck)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(addressToCheck);
                if (!String.IsNullOrEmpty(host.AddressList[0].ToString()))
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
    }
}
