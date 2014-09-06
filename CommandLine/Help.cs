using Utils;

namespace CommandLine
{
	public enum Command
	{
		Delete,
		List,
		Login,
		Open,
		Quit,
		Reset,
		Retrieve,
		Stat,
		UniqueIdentifier
	}
	// Help.Show(Command.Delete)
	public static class Help
	{
		public static void Show(Command c)
		{
			
		}
		
		private string hDelete = @"DEL- Deletes a message from the server.
Usage: delete/del [msgID]
Info: Deletion can be canceled by reset. See help reset for more information.";
		
		private string hList = @"LIST- Displays the size (in bytes) of a message.
Usage: list [msgID]
Options:
-a: Displays the size of all the messages on the server.";
		
		private string hLogin = @"LOGIN- Logs in the server using a username and a password.
Usage: login [enter username] [enter password]";
		
		private string hOpen = @"OPEN- Connects to the server.
Usage: open [host] [port]
Options:
-s: Uses SSL to secure the connection.
Default: If you do not enter a port, the client will try to log in on port 995 with a secure connection.";
		
		private string hQuit = @"QUIT- Closes the connection between the client and the server.
Usage: quit";
		
		private string hReset = @"RESET- Cancels the deletion of messages.
Usage: reset/rset
Warning: This command can only cancel the deletion of message form the CURRENT session.";
			
		private string hRetrieve = @"RETR- Retrieves a message from the server.
Usage: retrieve/retr [msgID]
Options:
-r: Raw (whole message)
-h: Header only
-s: Subject only
-d: Date only
-b: Body only
-f: Save to file
Default: [uid] [subject] [sender] [receiver(s)]";
		
		private string hStat = @"STAT- Displays stat about the mailbox.
Usage: stat
Info: The statistics contains the number of message and the size of all the message combined.";
		
		private string hUniqueID = @"UID- Displays the unique identifier of the message.
Usage: uid [msgID]
Options:
-a: Lists all UIDs present on the server.";
	}
}