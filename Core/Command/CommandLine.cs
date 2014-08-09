namespace Core.Command
{
	public static class CommandLine
	{
		/// <summary>
		/// 
		/// </summary>
		/// <example>open pop.host.com port</example>
		/// <remarks>Port is 995 by default.<br/>
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
		/// <example>get messageID</example>
		/// <remarks>Options: <br/>
		/// default: Formatted
		/// -r : Raw (whole message) <br/>
		/// -h : Header only <br/>
		/// -s : Subject only <br/>
		/// -d : Date only <br/>
		/// -b : Body only <br/>
		/// </remarks>
		public const string Get = "get";
		
		/// <summary>
		/// 
		/// </summary>
		public const string Quit = "quit";
		
		/// <summary>
		/// 
		/// </summary>
		/// <example>uid msgID</example>
		public const string UID = "uid";
	}
}
