using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Collections.Generic;

using Utils;
using Core.Mail;
using Core.Helpers;
using Core.POP;
using Core.POP.CommandParser;

namespace Core.Network
{
	public class POP3Client : IDisposable
	{
		private bool disposed;
		private TcpClient client;
		private Stream stream;
		private const string invalidOperation =
			"Cannot execute this command in the {0} state";
		
		public int Port { get; private set; }
		public string Host { get; private set; }
		public IPAddress IP { get; private set; }
		public bool SSL { get; private set; }
		public bool Connected { get; private set; }
		
		public POPState State { get; private set; }
		
		
		public POP3Client(string host, int port, bool ssl = false)
		{
			Host = host;
			Port = port;
			client = new TcpClient();
			SSL = ssl;
			
			IPAddress[] ips = Dns.GetHostAddresses(host);
			foreach(var i in ips)
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
				InternalClose();
			}
			
			disposed = true;
		}
		
		/// <summary>
		/// Cleans up ressources on forced exit.
		/// </summary>
		private void InternalClose()
		{
			if(stream != null)
				stream.Close();
			
			if(client != null)
				client.Close();
		}
		
		/// <summary>
		/// Only called on handled exceptions.
		/// </summary>
		private void InternalExit()
		{
			InternalClose();
			Logger.Error("Exiting ...");
			Console.ReadLine();
			Environment.Exit(1);
		}
		#endregion
		
		#region Internal Send/Receive functions
		// These functions will be public until I finish the public API.
		
		public void SendCommand(string format, params object[] args)
		{
			SendCommand(string.Format(format,args));
		}
		
		public void SendCommand(string command)
		{
			if(!this.Connected)
				return;
			
			try
			{
				var buffer = Encoding.UTF8.GetBytes(string.Concat(
					command,
					Constants.Terminator));
				
				stream.Write(buffer, 0, buffer.Length);
				stream.Flush();
			}
			catch (Exception ex)
			{
				if(ex is SocketException || ex is IOException)
				{
					Logger.Exception(ex);
					InternalExit();
				}
				else
					throw;
			}

		}
		
		public List<string> ReceiveMultiLine()
		{
			var received = new List<string>();
			
			while(true)
			{
				string line = Receive();
				
				if(line == Constants.MultiLineTerminator)
					break;
				if(line.StartsWith(Constants.ERROR))
					break;
				
				received.Add(line);
			}
			
			return received;
		}
		
		public string Receive()
		{
			var received = new List<byte>();
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
			catch (Exception ex)
			{
				if(ex is SocketException || ex is IOException)
				{
					Logger.Exception(ex);
					InternalExit();
				}
				else
					throw;
			}
			
			response = Encoding.UTF8.GetString(received.ToArray());
			
			return response;
		}
		
		#endregion
		
		/// <summary>
		/// Connects to the POP3 server.
		/// </summary>
		/// <returns>True if the connection was sucessful,
		/// false otherwise.</returns>
		public bool Connect(bool dummy = false)
		{
			return Protocol.CheckHeader(Connect());
		}
		
		/// <summary>
		/// Connects to the POP3 server.
		/// </summary>
		/// <returns>The welcome message or
		/// an empty string if the connection failed.</returns>
		public string Connect()
		{
			if(this.Connected)
			{
				Quit();
				InternalClose();
				this.Connected = false;
				
				client = new TcpClient();
			}
			
			try
			{
				client.Connect(new IPEndPoint(IP, Port));
			}
			catch(SocketException ex)
			{
				Logger.Exception(ex);
				InternalExit();
			}
			
			if(client.Connected)
			{
				stream = client.GetStream();
				Utils.Logger.Network("Connected to {0}({1}) on port {2}",
				                     IP, Host, Port);
				this.Connected = true;
				
				if(SSL)
				{
					var secureStream = new SslStream(stream);
					secureStream.AuthenticateAsClient(Host);
					
					stream = secureStream;
					
					if(secureStream.IsAuthenticated)
						Utils.Logger.Network("SSL activated");
					else
						return string.Empty;
				}
				State = POPState.Authorization;
				
				return Receive(); // Server ready
			}
			else
				return string.Empty;
		}
		
		


		
		public string Login(string emailAddress, string password)
		{
			if(State != POPState.Authorization)
				throw new InvalidOperationException(
					string.Format(invalidOperation, State.ToString()));
			
			if(string.IsNullOrEmpty(emailAddress))
				emailAddress.ThrowIfNullOrEmpty("emailAddress");
			
			if(string.IsNullOrEmpty(password))
				password.ThrowIfNullOrEmpty("password");
			
			SendCommand("{0} {1}", Commands.USER, emailAddress);
			// "Send pass".
			Receive();
			
			SendCommand("{0} {1}", Commands.PASS, password);
			
			string response = Receive();

			if(Protocol.CheckHeader(response))
			{
				SendCommand(Commands.NoOperation);
				
				string check = Receive();
				if(!Protocol.CheckHeader(check))
				{
					return check; // Login limit ?
				}
				State = POPState.Transaction;
			}

			return response;
		}
		
		/// <summary>
		/// Disconnects from the POP3 server.
		/// WARNING: DOES NOT CLOSE THE CONNECTION
		/// </summary>
		/// <returns>The server's exit meassage</returns>
		public string Quit()
		{
			// POP3 logic ... cf. RFC 1939 p10
			if(State == POPState.Transaction)
				State = POPState.Update;
			
			SendCommand(Commands.QUIT);
			
			return Receive();
		}
		
		/// <summary>
		/// Gets the list  messages stored on the server.
		/// </summary>
		/// <returns>A dictorinary of key-value pair where:<br/>
		/// 	Key: ID of the message <br/>
		/// 	Value: Size (in bytes)</returns>
		public Dictionary<int,int> ListMessages()
		{
			if(State != POPState.Transaction)
				throw new InvalidOperationException(
					string.Format(invalidOperation, State.ToString()));
			
			SendCommand(Commands.LIST);
			Receive();
			List<string> received = ReceiveMultiLine();
			
			return ListParser.Parse(received);
		}
		
		
		public List<POPMessage> GetMessages()
		{
			var messageList = new List<POPMessage>();
			
			foreach(KeyValuePair<int,int> kv in ListMessages())
			{
				messageList.Add(GetMessage(kv.Key));
			}
			
			return messageList;
		}
		
		public POPMessage GetMessage(int messageID)
		{
			if(State != POPState.Transaction)
				throw new InvalidOperationException(
					string.Format(invalidOperation, State.ToString()));
			
			SendCommand("{0} {1}", Commands.RETRIEVE, messageID);
			
			return new MailParser(ReceiveMultiLine()).Message;
		}
	}
}