
using Core.POP;
using Utils;

namespace CommandLine
{
	public static class Login
	{
		private const string example = "Usage: login user@domain.xx password";
		
		public static void Execute(ref POP3Client c, string[] args)
		{
			if(c == null)
			{
				Logger.Error("Not connected to a server.");
				Logger.Error("Use the OPEN command to connect.");
				return;
			}
			
			if(args.Length != 3)
			{
				Logger.Error("Invalid arguments: {0}\n{1}", 
				             string.Join(" ", args), example);
				return;
			}
			
			string userName = args[1];
			string password = args[2];
			
			// too much ?
			if(string.IsNullOrWhiteSpace(userName))
			{
				Logger.Error("Email address cannot be null");
				return;
			}
			if(string.IsNullOrWhiteSpace(password))
			{
				Logger.Error("Password cannot be null");
				return;
			}
			
			string response = c.Login(userName, password);
			
			if(c.LoggedIn)
			{
				Logger.Success("Login successfull");
				Logger.Command(response);
			}
			else
			{
				Logger.Error("Login failed");
				Logger.Command(response);
			}
		}
	}
}