using System.Collections.Generic;

using POPLib;
using Utils;

namespace CommandLine
{
	public static class UniqueIdentifier
	{
		private const string example = "Usage: uid msgID\nOptions:\n -a : Lists all UIDS listed on the server";
		
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
			int msgID;
			
			int.TryParse(args[1], out msgID);
			
			if(all)
			{
				var dic = c.GetUID();
				UniqueIdentifier.Display(dic);
			}
			else
			{
				if(msgID == 0)
				{
					Logger.Error("Invalid argument: {0}", args[1]);
					return;
				}
				
				var s = c.GetUID(msgID);
				var kv = new KeyValuePair<int, string>(msgID, s);
				UniqueIdentifier.Display(kv);
			}
			
		}
		
		private static void Display(Dictionary<int, string> uid)
		{
			foreach(var kv in uid)
				Display(kv);
		}
		
		private static void Display(KeyValuePair<int, string> uid)
		{
			if(uid.Key == -1)
				Logger.Error(uid.Value);
			else
				Logger.Inbox("{0} : {1}", uid.Key, uid.Value);
		}
	}
}