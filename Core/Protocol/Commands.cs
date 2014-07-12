namespace Core.Protocol
{
	public static class Commands
	{
		// Authorization state.
		
		/// <summary>
		/// C: USER emailAddress
		/// S: OK/ERR
		/// </summary>
		public const string USER = "USER";
		
		/// <summary>
		/// C: PASS password
		/// S: Mailbox contains x msg
		/// </summary>
		public const string PASS = "PASS";
		
		
		// Transaction state.
		
		/// <summary>
		/// C: RETR msgNumber
		/// S: *message*
		/// ..
		/// </summary>
		public const string RETRIEVE = "RETR";
		
		/// <summary>
		/// C: LIST
		/// S: msgNumber nbOfBytes
		/// ..
		/// </summary>
		public const string LIST = "LIST";
		
		/// <summary>
		/// C: DELE msgNumber
		/// S: ???
		/// </summary>
		public const string DELETE = "DELE";
		
		// Authorization OR Update state.
		
		/// <summary>
		/// C: QUIT
		/// S: tells what changed
		/// </summary>
		public const string QUIT = "QUIT";
		
	}
}