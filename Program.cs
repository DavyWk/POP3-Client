using System;
using System.IO;
using System.Collections.Generic;

using Utils;
using Core.Mail;
using Core.Helpers;
using Core.Network;
using Core.POP;
using Core.Command;

namespace POP3_Client
{
	// POP3 protocol : http://www.faqs.org/rfcs/rfc1939.html
	class Program
	{

		static int Main(string[] args)
		{
			const string host = "pop.gmail.com";
			//pop-mail.outlook.com
			const int port = 995;
			
			string logFile = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				"errors.log");
			
			Logger.Bind(logFile);
			Console.Title = "POP3 Client";
			
			POP3Client c = null;
			
			string cmd;
			while((cmd = Console.ReadLine()) != "x")
			{
				var cmdArgs = cmd.Split(' ');
				if(CheckForCommand(cmdArgs, "open"))
					Open.Execute(ref c, cmd);
				else if(CheckForCommand(cmdArgs, "quit"))
					Quit.Execute(ref c);	
				else
					Logger.Error("Unknown command{0}",
					             cmdArgs[0] != string.Empty ?
					             string.Format(" {0}", cmdArgs[0]) : ".");
			}
			
			Logger.Info("POP3Client developed by Davy.W, thanks for using.");
			Console.ReadLine();

			return 0;
		}
		
		private static bool CheckForCommand(string[] args, string cmd)
		{
			string cmdName = args[0].ToLower();
			
			return (cmdName == cmd);
		}
		
	}
	
}



