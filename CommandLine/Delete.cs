using Core.Network;
using Utils;

namespace CommandLine
{
	public static class Delete
	{
		private const string example = "Usage: delete msgID";
		
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
				Logger.Error("Not enough arguments");
				Logger.Error(example);
				return;
			}
			
			int msgID = -1;
			int.TryParse(args[1], out msgID);
			
			if(msgID == -1)
				Logger.Error("Invalid argument : {0}", args[1]);
			else
			{
				string response = c.Delete(msgID);
				Logger.Command(response);
			}		
		}
	}
}