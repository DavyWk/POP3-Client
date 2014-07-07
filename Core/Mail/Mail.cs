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
			Sender = new SenderInfo(IPAddress.Any,string.Empty,string.Empty);
			
			int index = headers.IndexOf("([") + 2;
			string s = headers.Substring(index,headers.IndexOf("])",index) - index);

			#region Sender
			
			Sender.Address = IPAddress.Parse(s);
			index = 0;
			s = string.Empty;
			
			index = headers.IndexOf("From: ") + 6;
			s = headers.Substring(index, headers.IndexOf("<",index) - index);
			Sender.Name = s;
			s = string.Empty;
				
			index = headers.IndexOf('<',index) + 1;
			s = headers.Substring(index,headers.IndexOf('>',index) - index);
			Sender.EMailAddress = s;
			s = string.Empty;
			index = 0;
			
			#endregion
			
			index = headers.IndexOf("Message-ID: <") + 13;
			ID = headers.Substring(index,headers.IndexOf('@',index) - index);
			index = 0;
			
			index = headers.IndexOf("Subject: ") + 9;
			Subject = headers.Substring(index, headers.IndexOf("Mime",index,StringComparison.OrdinalIgnoreCase) - index);
			index = 0;
			
			
			
			Body = string.Empty;
			CharSet = Encoding.UTF8;
			ArrivalTime = new DateTime(0);
		}
	}

}
