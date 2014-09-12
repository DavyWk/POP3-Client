using System;
using System.Collections.Generic;

using POP;
using Utils;

namespace CommandLine
{
	public static class List
	{
		
		public static void Execute(ref POP3Client c, string[] args)
		{
			if(c == null)
			{
				Logger.Error("Not connected, use the open command first");
				return;
			}
			if(!c.LoggedIn)
			{
				Logger.Error("Not logged in, use the login command first");
				return;
			}
			
			if(args.Length < 2)
			{
				Logger.Error("Not enough arguments, use the help command");
				return;
			}
			
			
			bool all = args.Contains("-a") || args.Contains("-A");
			int msgID;
			
			int.TryParse(args[1], out msgID);
			
			if(all)
			{
				var dic = c.ListMessages();
				List.Display(dic);
			}
			else
			{
				if(msgID == 0)
				{
					Logger.Error("Invalid argument : {0}", args[1]);
					return;
				}
				
				var kv = c.ListMessage(msgID);
				List.Display(kv);
			}
		}
		
		private static void Display(Dictionary<int, int> list)
		{
			foreach(var kv in list)
				Display(kv);
		}
		
		private static void Display(KeyValuePair<int, int> info)
		{
			if(info.Value != -1)
				Logger.Inbox("{0} - {1} bytes", info.Key, info.Value);
			else
			{
				int id = info.Key;
				Logger.Inbox("{0} - Invalid message ID", id);
			}
		}
	}
}
