using POP;
using Utils;

namespace CommandLine
{
	public static class Quit
	{
		public static void Execute(ref POP3Client c)
		{
			if(c != null)
			{
				if((c.State == POPState.Authorization) ||
				   (c.State == POPState.Transaction))
				{
					if(c.Connected)
					{
						string exitMessage = c.Quit();
						if(exitMessage == string.Empty)
							Logger.Success("Disconnected from {0}", c.Host);
						else
							Logger.Command(exitMessage);
						c.Dispose();
						c = null;
					}
				}
				else
				{
					c.Dispose();
					c = null;
				}
			}
			else
				Logger.Error("Already disconnected");
		}
	}
}