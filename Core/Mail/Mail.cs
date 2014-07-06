using System;
using System.Net;
using System.Text;


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
		
		public Mail(string headers)
		{
			
			int index = headers.IndexOf("([") + 2;
			string ip = headers.Substring(index,headers.IndexOf("])") - index);
			
			ID = string.Empty;
			Sender = new SenderInfo(IPAddress.Parse(ip),string.Empty,string.Empty);
			Body = string.Empty;
			Subject = string.Empty;
			CharSet = Encoding.UTF8;
			ArrivalTime = new DateTime(0);
		}
	}

}
