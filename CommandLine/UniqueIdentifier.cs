using System.IO;
using System.Collections.Generic;

using POP;
using Utils;

namespace CommandLine
{
	public static class UniqueIdentifier
	{
		
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
				Logger.Error("Not enough arguments, use the help command");
				return;
			}
			
			bool all = args.Contains("-a", true);
			bool file = args.Contains("-f", true);
			int msgID;
			
			int.TryParse(args[1], out msgID);
			
			if(all)
			{
				var dic = c.GetUID();
				
				if(file)
					SaveToFile(dic);
				else
					Display(dic);
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
				
				if(file)
					SaveToFile(kv);
				else
					Display(kv);
			}
			
		}
		
		private static void Display(Dictionary<int, string> uids)
		{
			foreach(var kv in uids)
				Display(kv);
		}
		
		private static void Display(KeyValuePair<int, string> uid)
		{
			if(uid.Key == -1)
				Logger.Error(uid.Value);
			else
				Logger.Inbox("{0} : {1}", uid.Key, uid.Value);
		}
		
		private static void SaveToFile(Dictionary<int, string> uids)
		{
			string filePath = Path.Combine(
				System.Environment.CurrentDirectory, "UniqueIdentifiers.txt");
			Logger.Info("Saving to: {0}", filePath);
			
			using(var sw = new StreamWriter(File.Create(filePath)))
			{
				foreach(var kv in uids)
				{
					sw.WriteLine(string.Format("{0} : {1}", kv.Key, kv.Value));
				}
			}
			Logger.Success("Saved {0} uids", uids.Count);
		}
		private static void SaveToFile(KeyValuePair<int, string> uid)
		{
			var dic = new Dictionary<int, string>();
			dic.Add(uid.Key, uid.Value);
			SaveToFile(dic);
		}
		
		
	}
}