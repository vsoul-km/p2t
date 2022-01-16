using System;

namespace p2t.Resources.DataTypes
{
    class GlobalVariables
    {
        public GlobalVariables()
        {
            LogStartTime = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
        }
        public string LogStartTime { get; }
        public string Address { get; set; }
        public string AddressHostName { get; set; }
        public string OriginalAddress { get; set; }
    }
}