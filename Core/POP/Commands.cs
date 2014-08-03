namespace Core.POP
{
	public static class Commands
	{
		// Authorization state.
		
		/// <summary>
		/// C: USER emailAddress <br/>
		/// S: OK/ERR
		/// </summary>
		public const string USER = "USER";
		
		/// <summary>
		/// C: PASS password <br/>
		/// S: Mailbox contains x msg
		/// </summary>
		public const string PASS = "PASS";
		
		
		// Transaction state.
		
		/// <summary>
		/// C: RETR msgNumber <br/>
		/// S: *message* <br/>
		/// ..
		/// </summary>
		public const string RETRIEVE = "RETR";
		
		/// <summary>
		/// C: LIST <br/>
		/// S: msgNumber nbOfBytes <br/>
		/// ..
		/// </summary>
		public const string LISTALL = "LIST";
		
		/// <summary>
		/// C: LIST msgNumber <br/>
		/// S: msgNumber nbOfBytes
		/// </summary>
		public const string LISTMSG = "LIST";
		
		/// <summary>
		/// C: DELE msgNumber <br/>
		/// S: OK/ERR
		/// </summary>
		public const string DELETE = "DELE";
		
		/// <summary>
		/// C: NOOP <br/>
		/// S: OK/ERR
		/// </summary>
		public const string NoOperation = "NOOP";
		
		/// <summary>
		/// C: STAT <br/>
		/// S: NumberOfMessages SizeOfAllMessages
		/// </summary>
		public const string STAT = "STAT";
		
		
		// Authorization OR Update state.
		
		/// <summary>
		/// C: QUIT <br/>
		/// S: Tells what changed
		/// </summary>
		public const string QUIT = "QUIT";
		
	}
}