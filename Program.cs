using System;
using System.IO;
using System.Collections.Generic;

using Utils;
using POP;
using CommandLine;

namespace POP3_Client
{
	class Program
	{
		
		static int Main(string[] args)
		{
			//const string host = "pop.gmail.com";
			//pop-mail.outlook.com
			//const int port = 995;
			
			string[] exit = {"exit", "x",};
			string[] open = { "open","o"  };
			string[] quit = { "quit","q"  };
			string[] login = { "login", "l" };
			string[] stat = { "stat", "s", "stats" };
			string[] list = { "list" };
			string[] uid = { "uid" };
			string[] delete =  { "delete", "d", "del" };
			string[] reset = { "reset", "rset" };
			string[] retrieve = { "retrieve", "r", "retr"  };
			string[] help = { "help", "h" };
			
			string logFile = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				"errors.log");
			
			Logger.Bind(logFile);
			Console.Title = "POP3 Client";
			
			POP3Client c = null;
			
			string cmd;
			while(!CheckExit((cmd = Console.ReadLine()), exit))
			{
				var cmdArgs = cmd.Split(' ');
				if(CheckForCommand(cmdArgs, open))
					Open.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, quit))
					Quit.Execute(ref c);	
				else if(CheckForCommand(cmdArgs, login))
					Login.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, stat))
					Stat.Execute(ref c);
				else if(CheckForCommand(cmdArgs, list))
					List.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, uid))
					UniqueIdentifier.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, delete))
					Delete.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, reset))
					Reset.Execute(ref c);
				else if(CheckForCommand(cmdArgs, retrieve))
					Retrieve.Execute(ref c, cmdArgs);
				else if(CheckForCommand(cmdArgs, help))
					Help.Execute(cmdArgs);
				else
					Logger.Error("Unknown command \"{0}\". Use the \"help\" command to get help.", cmdArgs[0]);
			}
			
			Logger.Info("POP3Client developed by Davy.W https://github.com/DavyWk");
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



