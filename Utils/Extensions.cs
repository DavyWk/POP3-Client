using System;

namespace Utils
{
    public static class Extensions
    {
        public static string Capitalize(this string s)
        {
            string ret =  s.Trim();
            if(string.IsNullOrEmpty(ret))
                return ret;

            ret = char.ToUpper(ret[0]) + ret.Substring(1);
            return ret;
        }
    }
}
