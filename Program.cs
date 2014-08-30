using System;
using System.IO;
using System.Collections.Generic;

using Utils;
using POPLib;
using CommandLine;

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
			
			string[] exitCommands = { "x", "exit" };
			
			string logFile = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				"errors.log");
			
			Logger.Bind(logFile);
			Console.Title = "POP3 Client";
			
			POP3Client c = null;
			
			string cmd;
			while(!CheckExit((cmd = Console.ReadLine()), exitCommands))
			{
				var cmdArgs = cmd.Split(' ');
				if(CheckForCommand(cmdArgs, "open"))
					Open.Execute(ref c, cmd);
				else if(CheckForCommand(cmdArgs, "quit"))
					Quit.Execute(ref c);	
				else if(CheckForCommand(cmdArgs, "login"))
					Login.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, new string[] {"stat", "stats"}))
					Stat.Execute(ref c);
				else if(CheckForCommand(cmdArgs, "list"))
					List.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, "uid"))
					UniqueIdentifier.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, new string[] {"delete", "del"}))
					Delete.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, "reset"))
					Reset.Execute(ref c);
				else if(CheckForCommand(cmdArgs, new string[] { "retrieve", "retr" }))
					Retrieve.Execute(ref c, cmdArgs);
				else
					Logger.Error("Unknown command{0}",
					             cmdArgs[0] != string.Empty ?
					             string.Format(" {0}", cmdArgs[0]) : ".");
			}
			
			Logger.Info("POP3Client developed by Davy.W cnetadev@outlook.com");
			Console.ReadLine();

			return 0;
		}
		
		private static bool CheckForCommand(string[] args, string[] cmd)
		{
			foreach(var s in cmd)
			{
				if(CheckForCommand(args, s))
					return true;
			}
			return false;
		}
		
		private static bool CheckForCommand(string[] args, string cmd)
		{
			string cmdName = args[0].ToLower();
			
			return (cmdName == cmd);
		}
		
		private static bool CheckExit(string cmd, string[] exitCommands)
		{
			cmd = cmd.Trim();
			foreach(var s in exitCommands)
				if(cmd == s)
					return true;
			
			return false;
		}
		
	}
	
}



