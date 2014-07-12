using System;

using Utils;
using Core.Network;

namespace POP3_Client
{
	// POP3 protocol : http://www.faqs.org/rfcs/rfc1939.html
	class Program
	{

		static int Main(string[] args)
		{
			const string host = "pop.gmail.com"; // pop-mail.outlook.com
			const int port = 995;
			
			Logger.Bind(@"C:\Users\DavyOnly\Desktop\test.txt");
			
			Console.Title = "POP3 Client";
			
			POP3Client c = new POP3Client(host,port,true);
			
			Logger.Command(c.Connect());
			
			string address;
			System.Security.SecureString password;
			Console.Write("Enter your address email: ");
			address = Console.ReadLine();
			Console.Write("Enter your passowrd: ");
			password = Core.Helpers.Helpers.ReadPassword();
			
			Logger.Command(c.Login(address,password.ToAsciiString()));
			
			Logger.Command(c.Quit());
			
			c.Close();

			Console.ReadLine();
			return 0;
		}
		
	}
	
}



