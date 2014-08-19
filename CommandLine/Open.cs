﻿using System;

using POP;

using Utils;

namespace CommandLine
{
	public static class Open
	{
		private const string example = "Usage: open host port\nOptions:\n-s : SSL";
		
		//TODO: Rewrite this using Execute(ref POP3Client c, string[] args)
		public static void Execute(ref POP3Client c, string cmd)
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
					return;
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
				return;
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
		}
	}
}