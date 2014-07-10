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

		/// <summary>
		/// Gets a System.String form a SecureString
		/// </summary>
		/// <returns>The original string</returns>
		public static string ToAsciiString(this SecureString s)
		{ // it doesnt't secure the string but its easier to handle password input in a SecureString
			string ret;
			IntPtr pointer = Marshal.SecureStringToBSTR(s);
			ret = Marshal.PtrToStringBSTR(pointer);
			Marshal.ZeroFreeBSTR(pointer);

			return ret;
		}
		
		/// <summary>
		/// Gets the string between two characters.
		/// </summary>
		/// <returns>The returned string does not include begin and end</returns>
		public static string SubstringEx(this string s,char begin, char end,int startIndex)
		{
			
			int bIndex = s.IndexOf(begin,startIndex) + 1; // +1: doesn't include the begin character
			int eIndex = s.IndexOf(end,bIndex);
			
			return s.Substring(bIndex,eIndex - bIndex);
		}
		
		/// <summary>
		/// Gets the string between two characters.
		/// </summary>
		/// <returns>The returned string does not include begin and end</returns>
		public static string SubstringEx(this string s,char begin, char end)
		{
			return SubstringEx(s,begin,end,0);
		}
	}
}
