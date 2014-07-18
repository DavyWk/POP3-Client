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
			
			// Some dumb SMTP server don't add a Subject field.
			if(m.Subject == null)
				m.Subject = "(No Subject)";
			
			m.ContainsHTML = CheckHTML();
			m.Body = GetBody(m.CharSet);
			
			Message = m;
		}
		
		private static string GetID(string s)
		{
			return s.SubstringEx('<','>');
		}
		
		private Person GetSender(string s)
		{
			Person p = new Person();
			// In case there's something interesting on
			// the following line.
			int offset = lines.IndexOf(s);
			string nextLine = lines[offset + 1];
			
			int index = s.IndexOf(':') + 2;
			s = s.Substring(index, s.Length - index);
			index = 0;
			s = MailDecoder.RemoveEncoding(s);
			
			if(s.IndexOf('"') > 0)
				p.Name = s.SubstringEx('"', '"');
			else
			{
				index = s.IndexOf('<');
				if(index > 0)
				{
					p.Name = s.Substring(0, index - 1);
					p.Name = p.Name.Trim();
				}
				else
					index = 0;
			}

			
			p.EMailAddress = s.SubstringEx('<', '>');
			if(string.IsNullOrWhiteSpace(p.EMailAddress))
				p.EMailAddress = s.Substring(index,s.Length - index);
			
			if(string.IsNullOrWhiteSpace(p.EMailAddress))
			{ // Means that sender info is on the other line.
				return GetSender(nextLine);
			}
			
			if(nextLine.StartsWith("\t"))
			{
				nextLine = nextLine.Trim();
				// If next line contains a valid email address.
				if(nextLine.StartsWith("<")
				   && nextLine.Contains("@")
				   && nextLine.EndsWith(">"))
				{
					p.EMailAddress = nextLine.SubstringEx('<', '>');
				}
			}
			
			return p;
		}
		
		private List<Person> GetReceivers(string s)
		{
			int offset = lines.IndexOf(s);
			int index = 0;
			int lastIndex = 0;
			var receivers = new List<Person>();
			s = s.Replace("To:", string.Empty).Trim();
			
			// Handles multiple receivers.
			string nextLine = string.Empty;
			while((nextLine = lines[++offset]).StartsWith("  "))
			{
				// Remove double space.
				nextLine = nextLine.Remove(0, 2);
				s = string.Format("{0} {1}", s, nextLine);
			}
			
			s = MailDecoder.RemoveEncoding(s);
			char[] delimitor = new char[] { '"', ' ', '<'};
			
			do
			{
				lastIndex = index;
				
				if((index = s.IndexOfAny(delimitor,index)) > -1)
				{
					Person receiver = new Person();
					receiver.Name = s.SubstringEx('"', '"',index);
					
					if(receiver.Name == string.Empty)
						receiver.Name = s.SubstringEx(' ', '<',index).Trim();
					
					receiver.EMailAddress = s.SubstringEx('<', '>', index);
					
					// In case the name is the same as the email address.
					if(receiver.Name == receiver.EMailAddress)
						receiver.Name = string.Empty;
					
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
			int offset = lines.IndexOf(s);
			// Skip space.
			int index = s.IndexOf(':') + 2;
			string ret = "(No Subject)";
			if((index != -1) && (index < s.Length))
			{
				s = s.Substring(index,s.Length - index);
				s = MailDecoder.RemoveEncoding(s);
				if(s.Trim() == "RE:")
					s = "RE: (No Subject)";
				
				ret = s;
			}
			string nextLine = lines[offset+1];
			if(nextLine.StartsWith("\t") || nextLine.StartsWith(" "))
			{
				nextLine = nextLine.Replace(" ", string.Empty);
				nextLine = nextLine.Replace("\t", string.Empty);
				nextLine = MailDecoder.RemoveEncoding(nextLine);
				string.Concat(ret, nextLine);
			}
			// Some subjects are formatted like that ...
			ret = ret.Replace('_', ' ');
			
			return ret;
		}
		
		private static DateTime GetDate(string s)
		{
			s = MailDecoder.RemoveJunk(s);
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

				
				if(i > bodyStart)
				{
					if((htmlBegin == -1) && lines[i].StartsWith("<html>") ||
					   lines[i].StartsWithEx("<!DOCTYPE html>"))
						htmlBegin = i;
					if((htmlEnd == -1) && lines[i].StartsWith("</html>"))
						htmlEnd = i;
					
					// Sometimes lines end with = sign.
					if(lines[i].EndsWith("="))
						lines[i] = lines[i].Remove(lines[i].Length -1 , 1);
					lines[i] = MailDecoder.RemoveJunk(lines[i]);
					
					lBody.Add(lines[i]);
					
					// If bodyStart is already set, don't need to check again.
					continue;
				}
				
				// Not accurate.
				if(lines[i].StartsWith("X-OriginalArrivalTime:"))
				{
					// Skips blank lines after X-OriginalArrivalTime.
					
					int temp = i + 1;
					while(string.IsNullOrWhiteSpace(lines[temp]))
						temp++;
					
					bodyStart = temp;
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
		

	}
}