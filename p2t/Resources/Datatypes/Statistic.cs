using System.Collections.Generic;

namespace p2t.Resources.DataTypes
{
    class Statistic
    {
        private List<string> IpList = new List<string>();
        public int PingSuccess { get; set; }
        public int PingLost { get; set; }
        public long RttSumm { get; set; }
        public int TraceRoutes { get; set; }
        public string AddIp
        {
            set
            {
                if (!IpList.Contains(value))
                {
                    IpList.Add(value);
                }
            }
        }
        public List<string> GetIp => IpList;
    }
}
