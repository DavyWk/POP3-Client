using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Collections.Generic;

using Core.Protocol;

namespace Core.Network
{
	public class POP3Client : IDisposable
	{
		private bool disposed;
		private TcpClient client;
		private Stream stream;
		
		public int Port { get; private set; }
		public string Host { get; private set; }
		public IPAddress IP { get; private set; }
		public bool SSL { get; private set; }
		
		
		public POP3Client(string host, int port, bool ssl = false)
		{
			Host = host;
			IP = Dns.GetHostAddresses(Host)[0];
			Port = port;
			client = new TcpClient();
			SSL = ssl;
		}
		
		#region Implementing IDisposable
		public void Close()
		{
			Dispose();
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if(disposed)
				return;
			
			if(disposing)
			{
				stream.Dispose();
				client.Close();
			}
			
			disposed = true;
		}
		#endregion
		
		public bool Connect()
		{
			client.Connect(new IPEndPoint(IP,Port));
			
			if(client.Connected)
			{
				stream = client.GetStream();
				
				if(SSL)
				{
					SslStream secureStream = new SslStream(stream);
					secureStream.AuthenticateAsClient(Host);
					
					if(secureStream.IsAuthenticated)
					{
						Utils.Logger.Network("SSL activated");
					}
					else
						return false;
				}

				Utils.Logger.Network("Connected");
				return true;
			}
			else
				return false;
		}
		

		public void SendCommand(string format, params object[] args)
		{
			SendCommand(string.Format(format,args));
		}
		
		public void SendCommand(string command)
		{
			stream.Write(Encoding.UTF8.GetBytes(command + Constants.Terminator), 0, command.Length + Constants.TerminatorLength);
		}
		
		public List<string> ReceiveMultiLine()
		{
			List<string> received = new List<string>();
			
			while(true)
			{
				string line = Receive();
				
				if(line == Constants.MultiLineTerminator)
					break;
				
				received.Add(line);
			}
			
			return received;
		}
		
		public string Receive()
		{
			List<byte> received = new List<byte>();
			string response;
			
			try
			{
				while(true)
				{
					int b = stream.ReadByte();
					if(b == Constants.Terminator[0] || b < 0)
						break;
					if(b != Constants.Terminator[1])
						received.Add((byte)b);
				}
			}
			catch (SocketException ex)
			{
				// Need to implement some kind of
				// OnError delegate to log
				// the exception.
				throw;
			}
			
			response = Encoding.UTF8.GetString(received.ToArray());
			
			return response;
		}
	}
}