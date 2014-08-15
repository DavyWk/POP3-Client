using Core;
using Core.POP;
using Core.Network;

using Utils;

namespace Core.Command
{
	public static class Stat
	{
		private const string example = "Usage: stat";
		
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
			
			Logger.Info("STATS: {0} messages, {1} bytes total",
			            kv.Key, kv.Value);
		}
	}
	
}