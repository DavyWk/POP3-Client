using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

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
				
				if((followingSpace != s.Length)
				   && (s[followingSpace + 1] ==  '=')
				   && (s[followingSpace + 2] == '?'))
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
				
				if(code == 'B') // The string is encoded using Base64.
				{
					// Two padding characters.
					endIndex = current.IndexOf("==",index) + 2;
					// One padding character.
					if(endIndex == 1)
						endIndex = current.IndexOf('=', index) + 1;
					// No padding character.
					if(current[endIndex - 2] == '?')
						endIndex -= 2;
					
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
						decoded = RemoveJunk(decoded, charset);
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
		
		public static string RemoveJunk(string s, Encoding enc = null)
		{
			if(enc == null)
				enc = Encoding.UTF8;
			/* 
               Sometimes, UTF characters are encoded like:
			   =C3=EA=90
			   Get all of the hexadecimal chars between the = signs, and then
			   use Encoding.GetString() to get the UTF character.
			*/
			
			// Should be const.
			char[] HexChars = { 'A', 'B', 'C', 'D', 'E', 'F',
				'0','1', '2', '3', '4', '5', '6', '7', '8', '9'};
			
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
				
				// Just to know where the original string starts.
				// -1: Keep the '=' sign.
				lastIndex = index - 1;
				
				int next = index;
				var hex = new List<byte>();
				
				string hexString = current.Substring(index, 2);
				if(!hexString.ToCharArray().Contains(HexChars))
					break;
				byte b = (byte)GetCharFromHex(hexString);
				if(b == 0)
					break;
				
				hex.Add(b);
				while((next = current.IndexOf('=', next) + 1) > index)
				{
					if((next - 3) != index)
						break;
					
					index = next;
					
					hexString = current.Substring(index, 2);
					if(!hexString.ToCharArray().Contains(HexChars))
						break;
					b = (byte)GetCharFromHex(hexString);
					if(b == 0)
						break;
					hex.Add(b);
				}
				// Gets to the end of the encoded string.
				index += 2;
				
				// Original is used to keep the = sign at the
				// beginning.
				string original =
					current.Substring(lastIndex, index - lastIndex);
				string decoded = enc.GetString(hex.ToArray());
				current = current.Replace(original, decoded);
				
				// Reset index for next iteration.
				index  = lastIndex + 1;
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
				byte val = 0;
				byte.TryParse(hexString,
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
