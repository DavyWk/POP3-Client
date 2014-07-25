using System;
using System.IO;
using System.Collections.Generic;

using Utils;
using Core.Mail;
using Core.Helpers;
using Core.Network;
using Core.POP;

namespace POP3_Client
{
	// POP3 protocol : http://www.faqs.org/rfcs/rfc1939.html
	class Program
	{

		static int Main(string[] args)
		{
			const string host = "pop-mail.outlook.com";
			const int port = 995;
			
			string logFile = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				"errors.log");
			
			Logger.Bind(logFile);
			Console.Title = "POP3 Client";
			
			var c = new POP3Client(host, port, true);
			
			Logger.Network(Protocol.RemoveHeader(c.Connect()));
			
			string address;
			System.Security.SecureString password;
			Console.Write("Enter your address email: ");
			address = Console.ReadLine();
			Console.Write("Enter your passowrd: ");
			password = HelperMethods.ReadPassword();
			
			Logger.Command(c.Login(address, password.ToAsciiString()));
			if(Logger.Status == LogType.Error)
			{
				Console.ReadLine();
				Environment.Exit(1);
			}
			
			int size = 0;
			int nb = 0;
			
			foreach (KeyValuePair<int, int> kv in c.ListMessages())
			{
				size += kv.Value;
				nb++;
			}
			Logger.Inbox("{0} messages, {1} bytes total.", nb, size);
	
			c.GetMessage(307);
			
//			int i = 1;
//			foreach (POPMessage m in c.GetMessages())
//			{
//				// TODO: Fix NullReferenceException here
//				foreach(Person p in m.Receivers)
//					Logger.Inbox(true,"{0} : {1}", i, p.EMailAddress);
//				i++;
//			}
			
			Logger.Command(c.Quit());
			
			c.Close();
			Console.ReadLine();
			return 0;
		}
		
	}
	
}



