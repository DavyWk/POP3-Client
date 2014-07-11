using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;

using Core.Protocol;

namespace Core.Network
{
	public class POP3Client : IDisposable
	{
		private bool disposed;
		private TcpClient client;
		private SslStream stream;
		
		public int Port { get; private set;}
		public string Host { get; private set;}
		public IPAddress IP {get; private set;}
		
		public POP3Client(string host, int port)
		{
			Host = host;
			IP = Dns.GetHostAddresses(Host)[0];
			Port = port;
			client = new TcpClient(new IPEndPoint(IP,port));
			
			
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
		
		private void SendCommand(string command)
		{
			stream.Write(Encoding.UTF8.GetBytes(command + Constants.Terminator), 0, command.Length + Constants.TerminatorLength);
		}
	}
}