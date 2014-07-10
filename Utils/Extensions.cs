using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Utils
{
	public static class Extensions
	{
		public static string Capitalize(this string s)
		{
			string ret = s.Trim();
			if (string.IsNullOrEmpty(ret))
				return ret;

			ret = char.ToUpper(ret[0]) + ret.Substring(1);
			return ret;
		}

		public static string ToAsciiString(this SecureString s)
		{
			string ret;
			IntPtr pointer = Marshal.SecureStringToBSTR(s);
			ret = Marshal.PtrToStringBSTR(pointer);
			Marshal.ZeroFreeBSTR(pointer);

			return ret;
		}
		
		public static string SubstringEx(this string s,char begin, char end)
		{
			int bIndex = s.IndexOf(begin) + 1; // +1: doesn't include the begin character
			int eIndex = s.IndexOf(end);
			
			return s.Substring(bIndex,eIndex - bIndex);
		}
	}
}
