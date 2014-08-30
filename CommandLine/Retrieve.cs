using System.IO;
using System.Collections.Generic;

using POP;
using Utils;

namespace CommandLine
{
	public static class Retrieve
	{
		private const string example =
			@"Usage: retrieve msgID
-r : Raw (whole message)
-h : Header only
-s : Subject only
-d : Date only
-b : Body only
-f : Save to file
			";
		public static void Execute(ref POP3Client c, string[] args)
		{
			if(c == null)
			{
				Logger.Error("Not connected, use the open command first");
				return;
			}
			if(!c.LoggedIn)
			{
				Logger.Error("Not logged in, use the login command first");
				return;
			}
			if(args.Length < 2)
			{
				Logger.Error("Not enough arguments\n{0}", example);
				return;
			}
			
			int msgID;
			int.TryParse(args[1], out msgID);
			if(msgID < 1)
			{
				Logger.Error("Invalid ID: {0}\n{1}", args[1], example);
				return;
			}
			
			POPMessage m = c.GetMessage(msgID);
			
			if(m.ID == Constants.INVALID)
			{
				Logger.Error(m.Body.Capitalize()); // The error message
				return;
			}
			
			var data = new List<string>();
			bool subject = args.Contains("-s", true);
			bool date = args.Contains("-d", true);
			bool body = args.Contains("-b", true);
			bool headers = args.Contains("-h", true);
			bool raw = args.Contains("-r", true);
			bool file = args.Contains("-f", true);
			
			
			if(subject)
			{
				string line = m.Subject;
				if(file)
					line.Insert(0, "Subject: ");
				data.Add(line);
			}
			
			if(date)
			{
				string line = m.ArrivalTime.ToString();
				if(file)
					line.Insert(0, "Date: ");
				data.Add(line);
			}
			
			if(body)
			{
				string line = m.Body;
				if(file)
					line.Insert(0, "Body:\r\n");
				data.Add(line);
			}
			
			if(headers)
			{
				string line = m.Header.ToString("\r\n");
				if(file)
					line.Insert(0, "Header: \r\n");
				data.Add(line);
			}
			
			if(raw)
			{
				string line = m.Raw.ToString("\r\n");
				if(file)
					line.Insert(0, string.Format("Dump of message {0}:\r\n",
					                             msgID));
				data.Add(line);
			}
			
			
			if(!args.Contains("-f", true))
			{
				if(data.Count == 0)
				{
					DefaultFormat(m, ref data);
				}
				
				Logger.Inbox(data.ToString("\r\n"));
				return;
			}
			
			WriteToFile(msgID, m.Subject, data);
			
		}
		
		private static void WriteToFile(int id, string subject, List<string> buffer)
		{
			string fileName = string.Format("{0}-{1}.txt", id, subject);
			string filePath = Path.Combine(System.Environment.CurrentDirectory,
			                               fileName);
			
			using(var fs = File.Create(filePath))
			{
				using(var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
					sw.WriteLine(buffer.ToString("\r\n"));
			}
			
			Logger.Info("Successfully written to {0}", filePath);
		}
		
		private static void DefaultFormat(POPMessage message, 
		                                  ref List<string> buffer)
		{
			buffer.Add("ID: {0}", message.ID);
			buffer.Add("Subject: {0}", message.Subject);
			buffer.Add("Sent from {0} at {1}",
			           message.Sender, message.ArrivalTime.ToString());
			var receivers = new List<string>();
			
			foreach(var p in message.Receivers)
				receivers.Add(p.EMailAddress);
			
			buffer.Add("Receivers: {0}", receivers.ToString(", "));
		}
	}
}
