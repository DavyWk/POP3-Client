﻿using System;
using System.IO;
using System.Text;
using System.Security;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Collections.Generic;
using System.Runtime.InteropServices;


using Utils;
using Core.Mail;
using Core.CommandParser;

namespace POP3_Client
{
	// POP3 protocol : http://www.faqs.org/rfcs/rfc1939.html
	class Program
	{

		static int Main(string[] args)
		{
			const string host = "pop-mail.outlook.com";
			const int port = 995;

			Console.Title = "POP3 Client";

			TcpClient client = new TcpClient();
			client.Connect(Dns.GetHostAddresses(host),port);

			if (!client.Connected)
			{
				Logger.Network("Failed to connect to {0}", host);
				Console.ReadKey();
				return 1;
			}
			Logger.Network("Connected to {0}:{1}", host, port);

			using (SslStream s = new SslStream(client.GetStream()))
			{
				
				s.AuthenticateAsClient(host);

				if (!s.IsAuthenticated)
				{
					Console.WriteLine("Error while activating SSL connection");
					Console.ReadKey();
					return 1;
				}
				
				Logger.Network("SSL connection activated");
				Logger.Command(Receive(s));
				
				Console.Write("Enter email address: ");
				SendCommand(s, string.Format("USER {0}", Console.ReadLine()));
				Logger.Command(Receive(s));

				Console.Write("Enter password: ");
				SecureString pw = Utilities.ReadPassword();
				SendCommand(s, string.Format("PASS {0}", pw.ToAsciiString()));
				Logger.Command(Receive(s));
				pw.Dispose();
				
				if(Logger.Status == ELogTypes.Error) // connection failed ?
				{
					Console.ReadLine();
					return 1;
				}
				
				SendCommand(s, "LIST");
				//Logger.Command(Receive(s));  # of messages
				Dictionary<int, int> messages = ListParser.Parse(ReceiveMultiLine(s));
				//ListParser.Display(messages);
				Console.WriteLine();
				
				if(messages.Count > 0)
				{
					SendCommand(s, string.Format("RETR {0}", messages.Count-1));
					Receive(s); // only header
					List<string> lines = ReceiveMultiLine(s);
					
					Message m = new Message(lines);
					
					Logger.Info("Subject: {0}",m.Subject);
					Logger.Info("MessageID: {0}",m.ID);
					Logger.Info("ArrivalTime: {0}", m.ArrivalTime.ToString());
					Logger.Info("Sender's EmailAddress: {0}",m.Sender.EMailAddress);
					Logger.Info("Sender's name: {0}",m.Sender.Name);
					foreach(Person p in m.Receivers)
						Logger.Info("Receiver: \"{0}\" <{1}>",p.Name.Trim(),p.EMailAddress);
					Logger.Info("Encoding : {0}",m.CharSet.BodyName.ToUpper());
					
					Console.WriteLine();
					/*
					string whole = string.Join("",lines.ToArray());
					Message m = new Message(whole);
					Logger.Info("Sender's EmailAddress: {0}",m.Sender.EMailAddress);
					Logger.Info("Sender's name: {0}",m.Sender.Name);
					Logger.Info("MessageID: {0}",m.ID);
					Logger.Info("Subject: {0}",m.Subject);
					Logger.Info("Encoding : {0}",m.CharSet.BodyName.ToUpper());
					Logger.Info("ArrivalTime: {0}", m.ArrivalTime.ToString());
					Logger.Info("Body: ");
					Console.WriteLine("{0}",m.Body);
					 */
					
					foreach(string l in lines)
					{
						Logger.Unknown(l);
					}
				}



				SendCommand(s, "QUIT");
				Logger.Command(Receive(s));
			}



			Console.ReadLine();
			return 0;
		}

		static void SendCommand(Stream s, string command)
		{

			s.Write(Encoding.ASCII.GetBytes(command + "\r\n"), 0, command.Length + 2);
		}
		
		static string Receive(Stream s)
		{
			List<byte> str = new List<byte>();
			string response;
			try
			{
				while (true)
				{
					int b = s.ReadByte();
					if (b == 10 || b < 0) // \r
						break;
					if (b != 13) // \n
						str.Add((byte)b);
				}
			}
			catch (SocketException ex)
			{
				Logger.Error(ex.Message);
			}
			
			response = Encoding.UTF8.GetString(str.ToArray());

			return response;
		}

		static List<string> ReceiveMultiLine(Stream s)
		{
			List<string> list = new List<string>();
			
			while (true)
			{
				string line = Receive(s);

				if (line == ".")
					break;

				list.Add(line);
			}

			return list;
		}

		static bool CheckResponse(string s)
		{
			if (s.StartsWith("+OK"))
				return true;
			else
				return false;
		}

	}



}



