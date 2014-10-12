using System;
using Utils;

namespace CommandLine
{
	// goal: Help.Show(Command.Delete)
	public static class Help
	{
		
		public static void Execute(string[] args)
		{
			if(args.Length < 2)
			{
				Help.Show(Command.Help);
				return;
			}
			string cmd = args[1].Capitalize();
			var c = Command.Unknown;
			Enum.TryParse(cmd, out c);
			if(!Enum.IsDefined(typeof(Command), cmd))
				c = Command.Unknown;
			Help.Show(c);
			
		}
		
		public static void Show(Command c)
		{
			string text;
			
			switch(c)
			{
				case Command.Delete:
					text = hDelete; 
					break;
					
				case Command.List:
					text = hList;
					break;
					
				case Command.Login:
					text = hLogin;
					break;
					
				case Command.Open:
					text = hOpen;
					break;
					
				case Command.Quit:
					text = hQuit;
					break;
					
				case Command.Reset:
					text = hReset;
					break;
					
				case Command.Retrieve:
					text = hRetrieve;
					break;
					
				case Command.Stat:
					text = hStat;
					break;
					
				case Command.Uid:
					text = hUniqueID;
					break;
					
				case Command.Help:
					text = hHelp;
					break;
					
				case Command.Unknown:
					text = "Unknown command";
					break;
					
				default:
					text = "No help text available";
					break;
			}
			
			Logger.Help(text);
		}
		
		private static string hDelete = @"DELETE- Deletes a message from the server.
Usage: delete/del/d [msgID]
Info: Deletion can be canceled by reset. See help reset for more information.";
		
		private static string hList = @"LIST- Displays the size (in bytes) of a message.
Usage: list [msgID]
Options:
-a: Displays the size of all the messages on the server.";
		
		private static string hLogin = @"LOGIN- Logs in the server using a username and a password.
Usage: login [enter username] [enter password]";
		
		private static string hOpen = @"OPEN- Connects to the server.
Usage: open [host] [port]
Options:
-s: Uses SSL to secure the connection.
Default: If you do not enter a port, the client will try to log in on port 995 with a secure connection.";
		
		private static string hQuit = @"QUIT- Closes the connection between the client and the server.
Usage: quit";
		
		private static string hReset = @"RESET- Cancels the deletion of messages.
Usage: reset/rset
Warning: This command can only cancel the deletion of message form the CURRENT session.";
			
		private static string hRetrieve = @"RETRIEVE- Retrieves a message from the server.
Usage: retrieve/retr [msgID]
Options:
-r: Raw (whole message)
-h: Header only
-s: Subject only
-d: Date only
-b: Body only
-f: Save to file
Default: [uid] [subject] [sender] [receiver(s)]";
		
		private static string hStat = @"STAT- Displays stat about the mailbox.
Usage: stat
Info: The statistics contains the number of message and the size of all the message combined.";
		
		private static string hUniqueID = @"UID- Displays the unique identifier of the message.
Usage: uid [msgID]
Options:
-a: Lists all UIDs present on the server.
-f: Saves the UID(s) to a file.";
		
		private static string hHelp =  @"HELP- Displays informations about the available commands. All commands are in lowercase.
DELETE- Deletes a message from the server.
LIST- Displays the size (in bytes) of a message.
LOGIN- Logs in the server using a username and a password.
OPEN- Connects to the server.
QUIT- Closes the connection between the client and the server.
RESET- Cancels the deletion of messages.
RETRIEVE- Retrieves a message from the server.
STAT- Displays stat about the mailbox.
UID- Displays the unique identifier of the message.";
	}
}