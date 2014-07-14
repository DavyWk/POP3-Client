using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;


using Utils;


namespace Core.Mail
{
	public struct Message
	{
		public string ID;
		public List<Person> Receivers;
		public Person Sender;
		public string Body;
		public string Subject;
		public Encoding CharSet;
		public DateTime ArrivalTime;
		public bool ContainsHTML;
		
		// Rewrite this with a static MessageParser class.
		public Message(List<string> message)
		{
			// Default initialization.
			ID = string.Empty;
			Receivers = new List<Person>();
			Sender = new Person(string.Empty,string.Empty);
			Body = string.Empty;
			Subject = "(No suject)";
			CharSet = Encoding.UTF8;
			ArrivalTime = new DateTime(0);
			ContainsHTML = false;
			
			foreach(string line in message)
			{
				
				string s = string.Empty;
				int index = 0;
				
				if(line.StartsWith("Message-ID:"))
				{
					ID = line.SubstringEx('<','>');
				}
				
				else if(line.StartsWith("From:"))
				{
					index = line.IndexOf(':') + 2;
					
					if(line.IndexOf('"') > 0)
						Sender.Name =  line.SubstringEx('"','"');
					else
						Sender.Name = line.SubstringEx(' ',' ');
					
					Sender.EMailAddress = line.SubstringEx('<','>');
				}
				
				else if(line.StartsWith("To:"))
				{
					int lastIndex = 0;
					do
					{
						lastIndex = index;
						if((index = line.IndexOfAny(new char[] { '"','<'},index)) > 0)
						{
							Person receiver = new Person();
							receiver.Name = line.SubstringEx('"','"',index);
							
							// Just in case.
							if(receiver.Name == string.Empty)
								receiver.Name = line.SubstringEx(' ',' ');
							
							receiver.EMailAddress = line.SubstringEx('<','>',index);
							
							Receivers.Add(receiver);
						}
						else
						{
							
							if(index == -1)
								index = 0;
							
							index = line.IndexOfAny(new char[] {' ', ','},lastIndex);
							

							// If there is nothing atfer this one.
							if((index == -1) || (lastIndex == index))
							{
								s = string.Empty;
								index = line.Length;
								Receivers.Add(new Person(string.Empty,s));
								break;
							}
							
							s = line.Substring(index + 1);
							Receivers.Add(new Person(string.Empty,s));
						}
					} while ((index = line.IndexOf(',',index)) > 0);
					
				}
				
				else if(line.StartsWith("Subject:"))
				{
					index = line.IndexOf(':') + 2; // skip space
					
					// In case there is no subject.
					if(index < line.Length)
						Subject = line.Substring(index,line.Length - index);
				}
				
				else if(line.StartsWith("Date:"))
				{
					string dateFormat = "ddd dd MMM yyyy HH:mm:ss";
					
					index = line.IndexOf(' ') + 1;
					string date = line.Substring(index,line.Length - index);
					
					// If there is a double space, delete one space.
					if(date.IndexOf("  ") > 0)
						date = date.Remove(date.IndexOf("  "),1);
					
					date = date.Trim();
					date = date.Replace(",","");
					
					// Special parenthesis like  02:31:57 +0000 (GMT+00:00)
					index = date.LastIndexOf('(');
					if(index == -1)
						index = date.LastIndexOfAny(new char[] { '(','-','+',' '});
					
					// In case there are parenthesis after the
					// UTC offset.
					//if(date[index] == '(')
					//	index--;
					
					int lastSpace = date.LastIndexOf(' ') - 1;
					if((lastSpace == -2) || (lastSpace < index))
						lastSpace = date.Length - 1;
					
					// Remove stuff between parentheses.
					if((date[index] == '(') && (date[lastSpace] == ')'))
					{
						date = date.Remove(index,date.Length - index);
						
						index = date.LastIndexOfAny(new char[] {'-','+'});
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
					
					// Remove any extra characters at the end of the string.
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
						dateFormat = dateFormat.Replace("ddd dd","d");
					
					day = 0;
					
					int.TryParse(date.Substring(4,2),out day);
					if((day > 0) && (day < 10))
						dateFormat = dateFormat.Replace("ddd dd", "ddd d");
					
					date = date.Trim();

					
					// Here, dt is equal to a default DateTime.
					DateTime dt = new DateTime(0);
					try
					{
						dt = DateTime.ParseExact(date,dateFormat,System.Globalization.CultureInfo.InvariantCulture);
						
						// Add global UTC offset and remove local UTC offset.
						dt += offset;
						dt += TimeZone.CurrentTimeZone.GetUtcOffset(dt);
					}
					catch(FormatException ex)
					{
						Logger.Exception(ex);
						ArrivalTime = new DateTime(0);
						continue;
					}
					
					ArrivalTime = dt;
				}
				
				else if(line.StartsWithEx("Content-Type:"))
				{
					if(line.Contains("text/html"))
					{
						index = line.IndexOf('=') + 1; // charset=
						
						s = line.SubstringEx('"', '"',index);
						
						if(s == string.Empty)
							s = line.Substring(index, line.Length - index);
						
						// No charset
						if(s.StartsWith("Content-Type:"))
							continue;
						
						CharSet = Encoding.GetEncoding(s);
					}

				}
				
				else if(line.StartsWith("X-OriginalArrivalTime:"))
				{
					// Exit the foreach loop because the remaining
					// lines will be part of the body.
					break;
				}
			}
			
			List<string> lBody = new List<string>();
			// The size of the mail should not exeed Int32.MaxValue.
			int bodyStart = Int32.MaxValue;
			
			int htmlBegin = -1;
			int htmlEnd = -1;
			for(int i = 0; i < message.Count;i++)
			{
				if(message[i].StartsWith("<html>") ||
				   message[i].StartsWith("<!DOCTYPE html"))
					htmlBegin = i;
				if(message[i].StartsWith("</html>"))
					htmlEnd = i;
				
				// No idea why, but sometimes there are equal signs
				// at the end of the line.
				if(message[i].EndsWith("="))
					message[i] = message[i].Remove(message[i].Length - 1, 1);
				
				if(i > bodyStart)
				{			
					lBody.Add(message[i]);
					
					// If bodyStart is already set, don't need to
					// check again.
					continue;
				}
				
				if(message[i].StartsWith("X-OriginalArrivalTime:"))
				{ // not accurate.
					// Skips blank line after X-OriginalArrivalTime.
					bodyStart = i + 1;
				}
				
			}
			
			if((htmlBegin != -1) && (htmlEnd != -1) && (htmlBegin < htmlEnd))
			{
				ContainsHTML = true;
				lBody = message.GetRange(htmlBegin,htmlEnd - htmlBegin);
			}
			
			
			Body = string.Join("",lBody.ToArray());
			
			
			// Not very accurate.
			byte[] raw = Encoding.UTF8.GetBytes(Body);
			Body = CharSet.GetString(raw);
		}
	}

}
