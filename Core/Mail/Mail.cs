﻿using System;
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
			
			index = headers.IndexOf("charset=") + 8;
			s = headers.Substring(index,headers.IndexOf("Content-",index) - index);
			s = s.Replace("\"","");
			CharSet = Encoding.GetEncoding(s);
			s = string.Empty;
			index = 0;
			
			index = headers.IndexOf("OriginalArrivalTime: ") + 20;
			s = headers.Substring(index,headers.IndexOf("(",index) - index);
			ArrivalTime = DateTime.Parse(s);
			s = string.Empty;
			index = 0;
			
			index = headers.IndexOf("FILETIME=[");
			index = headers.IndexOf(']',index) + 1;
			
			Body = headers.Substring(index,headers.Length - index);
			

		}
	}

}
