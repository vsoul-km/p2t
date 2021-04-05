//http://dotnetfollower.com/wordpress/2012/03/c-simple-command-line-arguments-parser/

namespace p2t.Resources.Modules
{
    public class UtilityArguments : InputArguments
    {
        public bool Log => GetBoolValue("-log");
        public bool F => GetBoolValue("-f");
        public bool D => GetBoolValue("-d");
        public bool T => GetBoolValue("-t");
        public bool Ta => GetBoolValue("-ta");
        public bool Te => GetBoolValue("-te");
        public bool Follow => GetBoolValue("-follow");
        public string L => GetValue("l");
        public string C => GetValue("c");
        public string W => GetValue("w");
        public string I => GetValue("i");
        public string Tt => GetValue("tt");
        public string Tc => GetValue("tc");

        public UtilityArguments(string[] args) : base(args)
        {
        }

        protected bool GetBoolValue(string key)
        {
            if (Contains(key))
            {
                return true;
            }
            return false;
        }
    }
}
