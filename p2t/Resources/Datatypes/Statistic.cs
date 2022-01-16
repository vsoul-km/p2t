using System.Collections.Generic;

namespace p2t.Resources.DataTypes
{
    class Statistic
    {
        private List<string> _uniqueIpAddressesList = new List<string>();
        public int PingSuccess { get; set; }
        public int PingLost { get; set; }
        public long RttSumm { get; set; }
        public int TraceRoutes { get; set; }
        public string AddIp
        {
            set
            {
                if (!_uniqueIpAddressesList.Contains(value))
                {
                    _uniqueIpAddressesList.Add(value);
                }
            }
        }
        public List<string> GetUniqueIpAddresses => _uniqueIpAddressesList;
    }
}
