using Core.Network;
using Utils;

namespace Core.Command
{
	public static class Quit
	{
		public static void Execute(ref POP3Client c)
		{
			if(c != null)
			{
				if(c.State == POP.POPState.Authorization)
				{
					Logger.Command(c.Quit());
					c.Dispose();
					c = null;
				}
				else	
				{
					c.Dispose();
					c = null;
				}
			}
		}
	}
}