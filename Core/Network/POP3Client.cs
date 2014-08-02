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
		
		private readonly int _port;
		public int Port
		{
			get
			{
				return _port;
			}
		}
		
		private readonly string _host;
		public string Host
		{
			get
			{
				return _host;
			}
		}
		
		private readonly IPAddress _ip;
		public IPAddress IP
		{
			get
			{
				return _ip;
			}
		}
		
		private readonly bool _ssl;
		public bool SSL
		{
			get
			{
				return _ssl;
			}
		}
		
		public bool Connected
		{
			get
			{
				if(client != null)
					return client.Connected;
				else
					throw new ObjectDisposedException("client");
			}
		}	
		
		public POPState State { get; private set; }
		
		
		public POP3Client(string host, int port, bool ssl = false)
		{
			_host = host;
			_port = port;
			client = new TcpClient();
			_ssl = ssl;
			_ip = Dns.GetHostAddresses(Host)[0];
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
			if(!client.Connected)
				Connect();
			
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
			if(!Connected)
				Connect();
			
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
			if(Connected)
			{
				Quit();
				InternalClose();
				
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
		public Dictionary<int, int> ListMessages()
		{
			if(State != POPState.Transaction)
				throw new InvalidOperationException(
					string.Format(invalidOperation, State.ToString()));
			
			SendCommand(Commands.LIST);
			Receive();
			List<string> received = ReceiveMultiLine();
			
			return ListParser.Parse(received);
		}
		
		
		/// <summary>
		/// Gets all messages stored on the server.
		/// </summary>
		/// <remarks>Not optimized, can take minutes to execute.</remarks>
		public List<POPMessage> GetMessages()
		{
			var messageList = new List<POPMessage>();
			
			foreach(KeyValuePair<int,int> kv in ListMessages())
			{
				POPMessage m = GetMessage(kv.Key);
				if(m != null)
					messageList.Add(m);
			}
			
			return messageList;
		}
		
		/// <summary>
		/// Gets the message stored with "messageID" on the server. <br/>
		/// If the message does not exist, returns null.
		/// </summary>
		/// <returns>The message stored with "messageID" on the server,
		/// or a message with ID "-1" in case of error.</returns>
		public POPMessage GetMessage(int messageID)
		{
			if(State != POPState.Transaction)
				throw new InvalidOperationException(
					string.Format(invalidOperation, State.ToString()));
			
			SendCommand("{0} {1}", Commands.RETRIEVE, messageID);
			
			POPMessage ret = null;
			
			if(Protocol.CheckHeader(Receive()))
				ret = new MailParser(ReceiveMultiLine()).Message;
			
			return ret;
		}
		
		/// <summary>
		/// Get statistics of the mailbox.
		/// </summary>
		/// <returns>A KeyValuePair where : <br/>
		/// Key: Number of messages <br/>
		/// Value: Size of all messages (in byte)</returns>
		public KeyValuePair<int, int> GetStats()
		{
			if(State != POPState.Transaction)
				throw new InvalidOperationException(
					string.Format(invalidOperation, State.ToString()));
			
			SendCommand(Commands.STAT);
			
			return StatParseer.Parse(Receive());
		}
	}
}