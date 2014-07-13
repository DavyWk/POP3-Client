using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Collections.Generic;

using Utils;
using Core.Helpers;
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
		
		public EStates State { get; private set; }
		
		
		public POP3Client(string host, int port, bool ssl = false)
		{
			Host = host;
			Port = port;
			client = new TcpClient();
			SSL = ssl;
			
			IPAddress[] ips = Dns.GetHostAddresses(host);
			foreach(IPAddress i in ips)
			{
				// Check for the first IPv4 address.
				if(i.AddressFamily == AddressFamily.InterNetwork)
				{
					IP = i;
					break;
				}
			}
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
		
		public bool Connect(bool dummy = false)
		{
			return Protocol.Protocol.CheckHeader(Connect());
		}
		
		/// <summary>
		/// Connects to the POP3 server.
		/// </summary>
		/// <returns>The welcome message or an empty string if connection failed.</returns>
		public string Connect()
		{
			client.Connect(new IPEndPoint(IP,Port));
			
			if(client.Connected)
			{
				stream = client.GetStream();
				Utils.Logger.Network("Connected to {0}:{1}",Host,Port);
				
				if(SSL)
				{
					SslStream secureStream = new SslStream(stream);
					secureStream.AuthenticateAsClient(Host);
					
					stream = secureStream;
					
					if(secureStream.IsAuthenticated)
						Utils.Logger.Network("SSL activated");
					else
						return string.Empty;
				}
				State = EStates.Authorization;
				
				return Receive(); // Server ready
			}
			else
				return string.Empty;
		}
		
		

		#region Internal Send/Receive functions
		public void SendCommand(string format, params object[] args)
		{
			SendCommand(string.Format(format,args));
		}
		
		public void SendCommand(string command)
		{
			stream.Write(Encoding.UTF8.GetBytes(command + Constants.Terminator), 0, command.Length + Constants.TerminatorLength);
			stream.Flush();
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
			
			response = Encoding.ASCII.GetString(received.ToArray());
			
			return response;
		}
		#endregion
		
		public string Login(string emailAddress, string password)
		{
			if(State != EStates.Authorization)
				throw new InvalidOperationException(string.Format("Cannot execute this command during the {0} state",State.ToString()));
			
			if(string.IsNullOrEmpty(emailAddress))
				emailAddress.ThrowIfNullOrEmpty("emailAddress");
			
			if(string.IsNullOrEmpty(password))
				password.ThrowIfNullOrEmpty("password");
			
			SendCommand("{0} {1}", Commands.USER, emailAddress);
			Receive(); // "Send pass".
			
			SendCommand("{0} {1}",Commands.PASS, password);
			
			string response = Receive();

			if(Protocol.Protocol.CheckHeader(response))
				State = EStates.Transaction;
			
			return response;
		}
		
		public string Quit()
		{
			// POP3 logic ... cf. RFC 1939 p10
			if(State == EStates.Transaction)
				State = EStates.Update;
			
			SendCommand(Commands.QUIT);
			
			return Receive();
		}
	}
}