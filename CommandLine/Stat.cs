using POP;

using Utils;

namespace CommandLine
{
	public static class Stat
	{
		
		public static void Execute(ref POP3Client c)
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
			
			var kv = c.GetStats();
			
			if(kv.Key != -1)
				Logger.Info("{0} messages, {1} bytes total",
				            kv.Key, kv.Value);
			else
				Logger.Error("Error while retrieving stats");
		}
	}
	
}