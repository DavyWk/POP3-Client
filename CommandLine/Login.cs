using System;
using System.Collections.Generic;

using POP;
using Utils;

namespace CommandLine
{
	public static class Login
	{
		
		public static void Execute(ref POP3Client c, string[] args)
		{
			if(c == null)
			{
				Logger.Error("Not connected to a server.");
				Logger.Error("Use the OPEN command to connect.");
				return;
			}
			
			if((args.Length  == 1) ||
			   ((args.Length == 2) && string.IsNullOrWhiteSpace(args[1])))
			{
				var l = new List<string>(args);
				
				if(args.Length  == 2)
					l.RemoveAt(1); //removes empty string
				
				Console.Write("Username: ");
				l.Add(Console.ReadLine());
				l.Add(Helpers.ReadPassword("Password: "));
				
				args =  l.ToArray();
				
				if(!c.Connected)
				{
					Logger.Error("Timeout expired.");
					Logger.Error("Use the OPEN command to reconnect");
					return;
				}
			}
			
			if(args.Length != 3)
			{
				Logger.Error("Invalid arguments: {0}", string.Join(" ", args));
				return;
			}
			
			string userName = args[1];
			string password = args[2];
			
			// too much ?
			if(string.IsNullOrWhiteSpace(userName))
			{
				Logger.Error("Email address cannot be null");
				return;
			}
			if(string.IsNullOrWhiteSpace(password))
			{
				Logger.Error("Password cannot be null");
				return;
			}
			
			string response = c.Login(userName, password);
			
			if(c.LoggedIn)
			{
				Logger.Success("Login successfull");
				Logger.Command(response);
			}
			else
			{
				Logger.Error("Login failed");
				Logger.Command(response);
			}
		}
	}
}