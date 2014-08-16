using Core.POP;
using Core.Mail;
using Utils;

namespace CommandLine
{
	public static class Retrieve
	{
		private const string example = 
			@"Usage: retrieve msgID
-r : Raw (whole message)
-h : Header only
-s : Subject only
-d : Date only
-b : Body only
-f fileName: Save to file
			";
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
				Logger.Error("Not enough arguments\n{0}", example);
				return;
			}
			
			int msgID;
			int.TryParse(args[1], out msgID);
			if(msgID < 1)
			{
				Logger.Error("Invalid ID: {0}\n{1}", args[1], example);
				return;
			}
			
			POPMessage m = c.GetMessage(msgID);
			
			if(m.ID == Constants.INVALID)
			{
				Logger.Error(m.Body.Capitalize()); // The error message
				return;
			}
			
			
			if(args.Contains("-s", true))
			{
				Logger.Inbox(m.Subject);
			}
			
			if(args.Contains("-d", true))
			{
				Logger.Inbox(m.ArrivalTime.ToString());
			}
			
			if(args.Contains("-b", true))
			{
				Logger.Inbox(m.Body);
			}
		}
	}
}