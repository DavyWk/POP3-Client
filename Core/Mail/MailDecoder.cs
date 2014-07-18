using System;
using System.Globalization;
using System.Text;

using Utils;

namespace Core.Mail
{
	public static class MailDecoder
	{
		public static string RemoveEncoding(string s)
		{
			// For strings like:
			// "=?UTF-8?B?ZGF2eWRhdmVraw==?="
			// "=?ENCODING?X?encoded?="
			
			int index = 0;
			s = s.Trim();
			//if(!s.StartsWith("\"=?") && !s.StartsWith("=?"))
			//	return s;
			
			string strCharset = s.SubstringEx('?', '?');
			if(strCharset == string.Empty)
				return s;
			
			while((index = s.IndexOf("=?")) > -1)
			{
				string current = string.Empty;
				// index is at the beginning of the encoded string.
				
				// End of encoded string
				int followingSpace = s.IndexOf(' ', index);
				
				// If there are no spaces, get the whole string.
				if(followingSpace == -1)
					followingSpace = s.Length;
				current = s.Substring(index, followingSpace - index);
				
				if(followingSpace < s.Length - 1)
					s = s.Remove(followingSpace, 1);

				
				Encoding charset = Encoding.GetEncoding(strCharset);
				// Skip ? after strCharset
				index = current.IndexOf('?', 3) + 1;
				char code = current[index];
				// Skip the second '?'
				index = current.IndexOf('?', index) + 1;
				string original = current.Substring(0, index);
				
				int endIndex = 0;
				string decoded = string.Empty;
				string encoded = string.Empty;
				
				if(code == 'B') // Base64
				{
					// Two padding characters.
					endIndex = current.IndexOf("==",index) + 2;
					// One padding character.
					if(endIndex == 1)
					{
						endIndex = current.IndexOf('=', index) + 1;
						
						if(endIndex == current.Length)
							endIndex = current.IndexOf('?',index);
					}
					
					encoded = current.Substring(index, endIndex - index);
					decoded = GetStringFromEncodedBase64(encoded,
					                                     charset);
				}
				else
				{
					endIndex = current.IndexOf(' ',index);
					// If there are no spaces, get the whole string.
					if(endIndex == -1)
						endIndex = current.IndexOf('?',index);
				}


				if(decoded == string.Empty)
				{
					if((index != -1) && (endIndex != -1))
					{
						encoded = current.Substring(index, endIndex - index);
						byte[] raw = Encoding.UTF8.GetBytes(encoded);
						decoded = charset.GetString(raw);
						decoded = RemoveJunk(decoded);
					}

				}
				
				string ret = current.Replace(original, string.Empty);
				if(!string.IsNullOrWhiteSpace(encoded))
					ret = ret.Replace(encoded, decoded);
				
				ret = ret.Replace("?=", string.Empty);
				s = s.Replace(current, ret);
				
			}
			
			return s;
		}
		
		public static string RemoveJunk(string s)
		{
			const string hexChars = "ABCDEF0123456789";
			
			string current = s;
			int index = 0;
			int lastIndex = -1;
			
			while((index < current.Length)
			      && (index = current.IndexOf("=",index)) > -1)
			{
				if(lastIndex == index)
				{
					index++;
					continue;
				}
				index++;
				if((index + 1) > current.Length - 1)
					continue;
				
				string hex = current.Substring(index, 2);
				// original is used to keep the = sign at the
				// beginning.
				string original = current.Substring(index - 1, 3);
				
				if(hex.ToCharArray().Contains(hexChars.ToCharArray()))
				{
					char c = GetCharFromHex(hex);
					if(c == '\0')
						continue;
					
					string temp = c.ToString();
					//current = current.Remove(index - 1,1);
					current = current.Replace(original,temp);
				}
				index--;
				lastIndex = index;
			}

			
			return current;
		}
		
		private static char GetCharFromHex(string hexString)
		{
			if(char.IsLetterOrDigit(hexString[0])
			   && (char.IsLetterOrDigit(hexString[1])
			       || char.IsWhiteSpace(hexString[1])))
			{
				long val = 0;
				long.TryParse(hexString,
				              NumberStyles.AllowHexSpecifier,
				              CultureInfo.InvariantCulture,
				              out val);
				if(val == 0)
					return '\0';
				
				char c = (char)val;
				return c;
			}
			
			return '\0';
		}
		
		private static string GetStringFromEncodedBase64(string s,
		                                                 Encoding enc = null)
		{
			if(enc == null) // Compile-time constant
				enc = Encoding.UTF8;
			
			byte[] raw = Convert.FromBase64String(s);
			
			return enc.GetString(raw);
		}
	}
}
