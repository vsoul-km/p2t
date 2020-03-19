using System;

namespace p2t.Resources.DataTypes
{
    class Variables
    {
        public string Date = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
        public string LogStartTime => Date;
        public string Address { get; set; }
        public string HostName { get; set; }
    }
}
