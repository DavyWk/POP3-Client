using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;

using Utils;
using Core.Mail;

namespace Core.Mail
{
	public class MessageParser
	{
		public MailMessage Message { get; private set; }
		
		private List<string> lines;
		
		public MessageParser(List<string> messageLines)
		{
			lines = messageLines;
			MailMessage m = new MailMessage();
			
			foreach(var l in lines)
			{
				if(l.StartsWith("Message-ID:"))
					m.ID = GetID(l);
				else if(l.StartsWith("From:"))
					m.Sender = GetSender(l);
				else if(l.StartsWith("To:"))
					m.Receivers = GetReceivers(l);
				else if(l.StartsWith("Subject:"))
					m.Subject = GetSubject(l);
				else if(l.StartsWith("Date:"))
					m.ArrivalTime = GetDate(l);
				else if(l.StartsWith("Content-Type:"))
					m.CharSet = GetEncoding(l);
				else if(l.StartsWith("X-OriginalArrivalTime:"))
					break; // Not 100% accurate.
			}
			
			// No idea why this happens.
			if(m.Subject == null)
				m.Subject = string.Empty;
			
			m.ContainsHTML = CheckHTML();
			m.Body = GetBody(m.CharSet);
			
			Message = m;
		}
		
		private string GetID(string s)
		{
			return s.SubstringEx('<','>');
		}
		
		private Person GetSender(string s)
		{
			Person p = new Person();
			
			int index = s.IndexOf(':') + 2;
			
			if(s.IndexOf('"') > 0)
				p.Name = s.SubstringEx('"', '"');
			else
				p.Name = s.SubstringEx(' ', ' ');
			
			p.EMailAddress = s.SubstringEx('<', '>');
			if(p.EMailAddress == string.Empty)
				p.EMailAddress = s.Substring(index,s.Length - index);
			
			return p;
		}
		
		private List<Person> GetReceivers(string s)
		{
			int index = 0;
			int lastIndex = 0;
			var receivers = new List<Person>();
			
			do
			{
				lastIndex = index;
				char[] delimitor = new char[] { '"', ' ', '<'};
				
				if((index = s.IndexOfAny(delimitor,index)) > 0)
				{
					Person receiver = new Person();
					receiver.Name = s.SubstringEx('"', '"',index);
					
					if(receiver.Name == string.Empty)
						receiver.Name = s.SubstringEx(' ', '<',index).Trim();
					
					receiver.EMailAddress = s.SubstringEx('<', '>', index);
					
					receivers.Add(receiver);
				}
				else
				{
					index = s.IndexOfAny(new char[] { ' ', ','},lastIndex);
					
					
					if((index == -1) || (lastIndex == index))
					{
						index = s.Length;
						receivers.Add(new Person(string.Empty,s));
						break;
					}
					
					string address  = s.Substring(index + 1);
					receivers.Add(new Person(string.Empty,s));
				}
			}
			while ((index = s.IndexOf(',', index)) > 0);
			
			return receivers;
		}
		
		private string GetSubject(string s)
		{
			// Skip space.
			int index = s.IndexOf(':') + 2;
			
			if((index != -1) && (index < s.Length))
			{
				s = s.Substring(index,s.Length - index);
				s = RemoveJunk(s);
				if(s.Trim() == "RE:")
					s = "RE: (No Subject)";
				
				return s.Trim();
			}
			else
				return "(No Subject)";
		}
		
		private DateTime GetDate(string s)
		{
			s = RemoveJunk(s);
			string dateFormat = "ddd dd MMM yyyy HH:mm:ss";
			int index = s.IndexOf(':') + 1;
			string date = s.Substring(index,s.Length - index);
			
			// If there is a double space, remove one space.
			if(date.IndexOf("  ") > 0)
				date = date.Remove(date.IndexOf("  "),1);
			

			date = date.Replace(",", string.Empty).Trim();
			
			// Special parenthesis like  02:31:57 +0000 (GMT+00:00)
			index = date.LastIndexOf('(');
			if(index == -1)
			{
				char[] delimitor = new char[]  {'-','+',' '};
				index = date.LastIndexOfAny(delimitor);
			}
			
			int lastSpace = date.LastIndexOf(' ') - 1;
			if((lastSpace == -2) || (lastSpace < index))
				lastSpace = date.Length - 1;
			
			// Remove stuff between parentheses.
			if((date[index] == '(') && (date[lastSpace] == ')'))
			{
				date = date.Remove(index,date.Length - index);
				
				index = date.LastIndexOfAny(new char[] { '-', '+'});
				lastSpace = date.Length - 1;
			}
			
			string utcOffset = string.Empty;
			if((date[index] == '-') || (date[index] == '+'))
			{
				index++;
				utcOffset = date.Substring(index,lastSpace - index);
				index--;
				
				date = date.Substring(0,index);
			}
			
			int offsetHours = 0;
			int.TryParse(utcOffset, out offsetHours);
			offsetHours /= 100;
			TimeSpan offset = new TimeSpan(Math.Abs(offsetHours),0,0);
			
			// Remove any etra character at the end of the string.
			for(int i = date.Length - 1; i > -1; i--)
			{
				if(char.IsDigit(date[i]))
				{
					i++;
					date = date.Remove(i,date.Length - i);
					break;
				}
			}

			// Checks for different date format.
			int day;
			int.TryParse(date.Substring(0,1), out day);
			if(day > 0)
				dateFormat = dateFormat.Replace("ddd dd", "d");
			
			day = 0;
			int.TryParse(date.Substring(4,2), out day);
			if((day > 0) && (day < 10))
				dateFormat = dateFormat.Replace("ddd dd", "ddd d");
			
			date = date.Trim();
			
			
			DateTime dt = new DateTime(0);
			try
			{
				dt = DateTime.ParseExact(
					date,
					dateFormat,
					CultureInfo.InvariantCulture);
				
				// Add global UTC offset and remove local UTC offset.
				dt += offset;
				dt += TimeZone.CurrentTimeZone.GetUtcOffset(dt);
			}
			catch(FormatException ex)
			{
				Logger.Exception(ex);
				dt = new DateTime(0);
			}
			
			return dt;
		}
		
		private Encoding GetEncoding(string s)
		{
			string encoding;
			int index = s.IndexOf('=') + 1;
			
			encoding = s.SubstringEx('"','"',index);
			
			// In case there is no quotation marks.
			if(encoding == string.Empty)
				encoding = s.Substring(index,s.Length - index);
			
			if(encoding.StartsWith("Content-Type:"))
				return Encoding.UTF8;
			if(s.Contains("/text") || s.Contains("/html")
			   && encoding != string.Empty)
				return Encoding.GetEncoding(encoding);
			else
				return Encoding.UTF8;
		}
		
		private string GetBody(Encoding charset = null)
		{
			// "Compile-time constant".
			if(charset == null)
				charset = Encoding.UTF8;
			
			var lBody = new List<string>();
			string body = string.Empty;
			int bodyStart = Int32.MaxValue;
			
			int htmlBegin = -1;
			int htmlEnd = -1;
			for(int i = 0; i < lines.Count; i++)
			{
				if((htmlBegin == -1) && lines[i].StartsWith("<html>") ||
				   lines[i].StartsWithEx("<!DOCTYPE html>"))
					htmlBegin = i;
				if((htmlEnd == -1) && lines[i].StartsWith("</html>"))
					htmlEnd = i;
				
				if(i > bodyStart)
				{
					if(lines[i].EndsWith("="))
						lines[i] = lines[i].Remove(lines[i].Length -1 , 1);
					lines[i] = RemoveJunk(lines[i]);
					
					lBody.Add(lines[i]);
					
					// If bodyStart is already set, don't need to check again.
					continue;
				}
				
				// Not accurate.
				if(lines[i].StartsWith("X-OriginalArrivalTime:"))
				{
					// Skips blank line after X-OriginalArrivalTime.
					bodyStart = i + 1;
				}
			}
			
			if((htmlBegin != -1) && (htmlEnd != -1) && (htmlBegin < htmlEnd))
				lBody = lines.GetRange(htmlBegin, htmlEnd - htmlBegin);
			

			body = string.Join("",lBody.ToArray());
			
			byte[] raw = Encoding.UTF8.GetBytes(body);
			body = charset.GetString(raw);
			
			return body;
		}
		
		private bool CheckHTML()
		{
			var kv = CheckHtml();
			
			if((kv.Key != -1) && (kv.Value != -1))
				return true;
			else
				return false;
		}
		
		private KeyValuePair<int,int> CheckHtml()
		{
			int begin = Int32.MaxValue;
			int end =  Int32.MaxValue;
			
			for(int i = 0; i < lines.Count; i++)
			{
				if((end == Int32.MaxValue) && lines[i].StartsWith("<html>") ||
				   lines[i].StartsWithEx("<!DOCTYPE html"))
				{
					begin = i;
					continue;
				}
				
				if((begin < i) && lines[i].StartsWith("</html"))
				{
					end = i;
					break;
				}
			}
			
			// Check for errors.
			if((begin == Int32.MaxValue) || (end == Int32.MaxValue)
			   || (begin > end))
			{
				begin = -1;
				end = -1;
			}
			
			return new KeyValuePair<int,int>(begin,end);
		}
		
		private string RemoveJunk(string s)
		{
			const string hexChars = "ABCDEF0123456789";
			
			string current = s;
			int index = 0;
			int lastIndex = 0;
			
			while((index < current.Length)
			      && (index = current.IndexOf("=",index)) > 0)
			{
				if(lastIndex == index)
				{
					index++;
					continue;
				}
				index++;
				if((index + 2) > current.Length - 1)
					continue;
				
				string hex = current.Substring(index, 2);
				if(hex.ToCharArray().Contains(hexChars.ToCharArray()))
				{
					if(char.IsLetterOrDigit(hex[0])
					   && char.IsLetterOrDigit(hex[1])
					   || char.IsWhiteSpace(hex[1]))
					{

						int val = 0;
						int.TryParse(hex,
						             NumberStyles.AllowHexSpecifier,
						             CultureInfo.InvariantCulture,
						             out val);
						if(val == 0)
							continue;
						
						char c = (char)val;
						string temp = c.ToString();
						current = current.Remove(index - 1,1);
						current = current.Replace(hex,temp);
					}
				}
				index--;
				lastIndex = index;
			}
			return current;
		}

	}
}