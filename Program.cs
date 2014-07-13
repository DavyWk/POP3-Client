using System;
using System.Collections.Generic;

using Utils;
using Core.Helpers;
using Core.Network;
using Core.Protocol;

namespace POP3_Client
{
	// POP3 protocol : http://www.faqs.org/rfcs/rfc1939.html
	class Program
	{

		static int Main(string[] args)
		{
			const string host = "pop-mail.outlook.com"; // pop-mail.outlook.com
			const int port = 995;
			
			string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),"errors.log");
			Logger.Bind(logFile);
			
			Console.Title = "POP3 Client";
			
			POP3Client c = new POP3Client(host,port,true);
			
			Logger.Network(Protocol.RemoveHeader(c.Connect()));
			
			string address;
			System.Security.SecureString password;
			Console.Write("Enter your address email: ");
			address = Console.ReadLine();
			Console.Write("Enter your passowrd: ");
			password = Core.Helpers.Helpers.ReadPassword();
			
			Logger.Command(c.Login(address,password.ToAsciiString()));
			
			foreach (KeyValuePair<int,int> kv in c.ListMessages())
				Logger.Inbox("{0} - {1} bytes", kv.Key,kv.Value);
			
			Logger.Command(c.Quit());
			
			//Logger.Command(c.Connect());
			//Logger.Command(c.Quit());
			
			c.Close();

			Console.ReadLine();
			return 0;
		}
		
	}
	
}



