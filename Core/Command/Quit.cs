﻿using Core.Network;
using Core.POP;
using Utils;

namespace Core.Command
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