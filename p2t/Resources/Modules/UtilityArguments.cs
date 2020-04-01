//http://dotnetfollower.com/wordpress/2012/03/c-simple-command-line-arguments-parser/

namespace p2t.Resources.Modules
{
    public class UtilityArguments : InputArguments
    {
        public bool Log
        {
            get { return GetBoolValue("-log"); }
        }

        public bool F
        {
            get { return GetBoolValue("-f"); }
        }


        public bool Follow
        {
            get { return GetBoolValue("-follow"); }
        }

        public string L
        {
            get { return GetValue("l"); }
        }

        public string C
        {
            get { return GetValue("c"); }
        }

        public string W
        {
            get { return GetValue("w"); }
        }

        public string I
        {
            get { return GetValue("i"); }
        }

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
