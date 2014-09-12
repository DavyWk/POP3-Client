using POP;
using Utils;

namespace CommandLine
{
	public static class Reset
	{
		
		public static void Execute(ref POP3Client c)
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
			c.Reset();

			Logger.Success("Reset successfull.");
		}
	}
}
