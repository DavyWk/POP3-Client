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
						Logger.Command(c.Quit());
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