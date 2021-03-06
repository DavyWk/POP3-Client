﻿using System;

using POP;

using Utils;

namespace CommandLine
{
	public static class Open
	{
		
		public static void Execute(ref POP3Client c, string[] args)
		{
			string host = string.Empty;
			int port = 0;
			bool ssl = false;
			
			if(c != null)
			{
				Logger.Info("A connection is already openned, do you want to abort it ? (y/n)");
				var ans = Console.ReadLine();
				if((ans == string.Empty) || (ans.ToLower() == "y"))
				{
					Quit.Execute(ref c);
				}
				else
					return;
			}
			
			if(args.Length > 1)
				host = args[1];
			if(args.Length > 2)
				int.TryParse(args[2], out port);
			
			
			if(args.Contains("-s") || args.Contains("-S"))
				ssl = true;
			
			
			if(string.IsNullOrEmpty(host))
			{
				Logger.Error("host cannot be an empty string");
				return;
			}
			string ret = string.Empty;
			c = new POP3Client(host, port, ssl);
			try
			{
				ret = c.Connect();
			}
			catch(System.Security.Authentication.AuthenticationException ex)
			{
				Logger.Network("AuthenticationException: {0}", ex.Message);
				c.Dispose();
				c = null;
				return;
			}
			
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