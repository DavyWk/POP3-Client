using System;

using Core.POP;
using Core.Network;
using Utils;

namespace Core.Command
{
	public static class Open
	{
		private const string example = "Usage: open host port\nOptions:\n-s : SSL";
		
		public static bool Execute(ref POP3Client c, string cmd)
		{
			string host;
			int port = 0;
			bool ssl = false;
			
			if(c != null)
			{
				Logger.Info(@"A connection is already openned, do you want to abort it ? (y/n)");
				char ans = char.ToLower(Console.ReadLine()[0]);
				if(ans == 'y')
				{
					Quit.Execute(ref c);
				}
				else
					return false;
			}
			
			var args = cmd.Split(' ');
			
			
			host = args[1];
			if(args.Length > 2)
				int.TryParse(args[2], out port);
			
			
			if(args.Contains("-s") || args.Contains("-S"))
				ssl = true;
			
			
			if(string.IsNullOrEmpty(host))
			{
				Logger.Error(example);
				Logger.Error("host cannot be an empty string");
				return false;
			}
			
			c = new POP3Client(host, port, ssl);
			string ret = c.Connect();
			
			if(Protocol.CheckHeader(ret))
			{
				
				Logger.Network("Connected to {0} on port {1}", c.Host, c.Port);
				if(c.SSL)
					Logger.Network("SSL connection activated");
				Logger.Network(Protocol.RemoveHeader(ret));
			}
			else
			{
				Logger.Network("Connection failed{0}",
				               ret != string.Empty ? " :" : ".");
				if(ret != string.Empty)
					Logger.Network(ret);
			}
			
			return Protocol.CheckHeader(ret);
			// Not using c.Connected to avoid exceptions.
		}
	}
}