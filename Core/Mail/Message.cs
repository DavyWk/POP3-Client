using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Utils;


namespace Core.Mail
{
	public struct Message
	{
		public string ID;
		public SenderInfo Sender;
		public string Body;
		public string Subject;
		public Encoding CharSet;
		public DateTime ArrivalTime;
		
		public Message(List<string> message)
		{
			// NULL initialization
			ID = string.Empty;
			Sender = new SenderInfo(IPAddress.Any,string.Empty,string.Empty);
			Body = string.Empty;
			Subject = string.Empty;
			CharSet = Encoding.ASCII;
			ArrivalTime = new DateTime(0);
			
			
			foreach(string line in message)
			{
				string s;
				int index;
				
				if(line.StartsWith("Message-ID:"))
				{
					ID = line.SubstringEx('<','>');
				}
				else if(line.StartsWith("Date:"))
				{
					const string dateFormat = "ddd, dd MMM yyyy HH:mm:ss";
					index = line.IndexOf(' ');
					string date = line.Substring(index,line.Length - index);
					index = date.LastIndexOf('-');
					string utcOffset = date.Substring(index,date.Length -index);
					int offsetHours = int.Parse(utcOffset)/100;
					TimeSpan offset = new TimeSpan(Math.Abs(offsetHours),0,0);
					
					date = date.Substring(0,index).Trim();
					DateTime dt =DateTime.ParseExact(date,dateFormat,System.Globalization.CultureInfo.InvariantCulture);

					// Add global UTC offset and remove local UTC offset.
					dt += offset;
					dt += TimeZone.CurrentTimeZone.GetUtcOffset(dt);
					
					ArrivalTime = dt;
				}
			}
			


		}
	}

}
