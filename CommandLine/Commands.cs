namespace CommandLine
{
	public static class Commands
	{
		//TODO: Document everything		

		/// <summary>
		/// Connects to server
		/// </summary>
		/// <example>open pop.host.com port</example>
		/// <remarks>Default is port = 995 and ssl = true<br/>
		/// Options: <br/>
		/// -s : SSL</remarks>
		public const string Open = "open";
		
		/// <summary>
		/// 
		/// </summary>
		/// <example>login user@domain.com password</example>
		public const string Login = "login";
		
		/// <summary>
		/// 
		/// </summary>
		/// <example>retrieve messageID</example>
		/// <remarks>Options: <br/>
		/// default: Formatted
		/// -r : Raw (whole message) <br/>
		/// -h : Header only <br/>
		/// -s : Subject only <br/>
		/// -d : Date only <br/>
		/// -b : Body only <br/>
		/// -f : Save to a file <br/>
		/// </remarks>
		public const string Retrieve = "retrieve";
		
		/// <summary>
		/// Disconnects from the POP server.
		/// </summary>
		public const string Quit = "quit";
		
		/// <summary>
		/// Gets the unique identifier of the message.
		/// </summary>
		/// <example>uid msgID</example>
		public const string UID = "uid";
	}
}
