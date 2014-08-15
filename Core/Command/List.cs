﻿using System;
using System.Collections.Generic;

using Core.Network;
using Utils;

namespace Core.Command
{
	public static class List
	{
		private static string example = "Usage: list msgID\nOptions: -a Lists all messages";
		
		public static void Execute(ref POP3Client c, string[] args)
		{
			if(c == null)
			{
				Logger.Error("Not connected, use the open command first");
				return;
			}
			if(c.LoggedIn == false)
			{
				Logger.Error("Not logged in, use the login command first");
				return;
			}
			
			if(args.Length < 2)
			{
				Logger.Error("Not enough arguments");
				Logger.Error(example);
				return;
			}
			
			
			bool all = args.Contains("-a") || args.Contains("-A");
			int msgID = -1;
			
			int.TryParse(args[1], out msgID);
			
			if(all)
			{
				var dic = c.ListMessages();
				List.Display(dic);
			}
			else
			{
				if(msgID == -1)
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
		
		private static void Display(KeyValuePair<int, int> list)
		{
			Logger.Inbox("{0} - {1} bytes", list.Key, list.Value);
			return;
		}
	}
}