using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Utils;


namespace Core.Mail
{
	public struct Mail
	{
		public string ID;
		public SenderInfo Sender;
		public string Body;
		public string Subject;
		public Encoding CharSet;
		public DateTime ArrivalTime;
		
		public Mail(List<string> message)
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
				else if(true)
				{
					
				}
			}
			


		}
	}

}
