﻿using System;
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
		
		public Message(List<string> message)
		{
			// Default initialization.
			ID = string.Empty;
			Receivers = new List<Person>();
			Sender = new Person(string.Empty,string.Empty);
			Body = string.Empty;
			Subject = string.Empty;
			CharSet = Encoding.ASCII;
			ArrivalTime = new DateTime(0);
			
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
					do
					{
						if((index = line.IndexOf('"',index)) > 0)
						{
							Person receiver = new Person();
							receiver.Name = line.SubstringEx('"','"',index);
							receiver.EMailAddress = line.SubstringEx('<','>',index);
							
							Receivers.Add(receiver);
						}
						else
						{
							if(index == -1)
								index = 0;
							
							index = line.IndexOf(' ',index);
							Receivers.Add(new Person(string.Empty,line.Substring(index)));
						}
					} while ((index = line.IndexOf(',',index)) > 0);
					
				}
				
				else if(line.StartsWith("Subject:"))
				{
					index = line.IndexOf(':') + 2; // skip space
					Subject = line.Substring(index,line.Length - index);
				}
				
				else if(line.StartsWith("Date:"))
				{
					string dateFormat = "ddd dd MMM yyyy HH:mm:ss";
					
					index = line.IndexOf(' ') + 1;
					string date = line.Substring(index,line.Length - index);
					index = date.LastIndexOfAny(new char[] {'-','+'});
					
					string utcOffset = date.Substring(index,date.Length - index);
					int offsetHours = int.Parse(utcOffset) / 100;
					TimeSpan offset = new TimeSpan(Math.Abs(offsetHours),0,0);
					
					int day;
					int.TryParse(date.Substring(0,1), out day);
					if(day > 0)
					{
						dateFormat = dateFormat.Replace("ddd dd","d");
					}
					
					date = date.Substring(0,index).Trim();
					date = date.Replace(",","");
					
					// Here, dt is equal to a default DateTime.
					DateTime dt = ArrivalTime;
					try
					{
						dt = DateTime.ParseExact(date,dateFormat,System.Globalization.CultureInfo.InvariantCulture);
						
						// Add global UTC offset and remove local UTC offset.
						dt += offset;
						dt += TimeZone.CurrentTimeZone.GetUtcOffset(dt);
					}
					catch(FormatException ex)
					{
						Logger.Error("FormatException: {0} {1}",ex.Message,ex.StackTrace);
					}
					
					ArrivalTime = dt;
				}
				
				else if(line.StartsWithEx("Charset="))
				{
					index = line.IndexOf('=') + 1;
					s = line.Substring(index, line.Length - index);
					CharSet = Encoding.GetEncoding(s);
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
			
			for(int i = 0; i < message.Count;i++)
			{
				if(i > bodyStart)
				{
					lBody.Add(message[i]);
					
					// If bodyStart is already set, don't need to
					// check again.
					continue;
				}
				
				if(message[i].StartsWith("X-OriginalArrivalTime:"))
				{
					// Skips blank line after X-OriginalArrivalTime.
					bodyStart = i + 1;
					continue;
				}
				
			}
			
			Body = string.Join("",lBody.ToArray());


		}
	}

}
