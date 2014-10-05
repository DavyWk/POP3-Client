using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using POP;
using Utils;

namespace CommandLine
{
	public static class Retrieve
	{
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
				Logger.Error("Not enough arguments, use the help command");
				return;
			}
			
			int msgID;
			int.TryParse(args[1], out msgID);
			if(msgID < 1)
			{
				Logger.Error("Invalid ID: {0}", args[1]);
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
			
			if(data.Count == 0)
				DefaultFormat(m, ref data);
			
			if(!args.Contains("-f", true))
			{
				Logger.Inbox(data.ToString("\r\n"));
				return;
			}
			
			bool html = m.ContainsHTML && args.Contains("-b");
			// Saving as HTML file if the user requests ONLY the body.
			WriteToFile(msgID, m.Subject, data, html);
			
		}
		
		private static void WriteToFile(int id, string subject,
		                                List<string> buffer, bool html = false)
		{
			var extensions =  new string[] { "txt", "html" };
			string fileName = string.Format("{0}-{1}.{2}",
			                                id, subject, html ?
			                                extensions[1] : extensions[0]);
			fileName =  CleanPath(fileName);
			string filePath = Path.Combine(System.Environment.CurrentDirectory,
			                               fileName);
			
			if(buffer.Count == 0)
			{
				Logger.Error("Nothing to write");
				return;
			}
			
			using(var fs = File.Create(filePath))
			{
				using(var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
					sw.WriteLine(buffer.ToString("\r\n"));
			}
			
			Logger.Success("Saved to {0}", filePath);
			Logger.Info("Would you like to open the file ? (y/n)");
			
			char ans = char.ToLower(Console.ReadLine()[0]);
			if(ans == 'y')
				System.Diagnostics.Process.Start(filePath);
		}
		
		private static void DefaultFormat(POPMessage message,
		                                  ref List<string> buffer)
		{
			buffer.Add("ID: {0}", message.ID);
			buffer.Add("Subject: {0}", message.Subject);
			buffer.Add("Sent from {0} the {1} at {2}",
			           message.Sender.EMailAddress,
			           message.ArrivalTime.ToShortDateString(),
			           message.ArrivalTime.ToShortTimeString());
			
			var receivers = new List<string>();
			
			foreach(var p in message.Receivers)
				receivers.Add(p.EMailAddress);
			
			buffer.Add("Receivers: {0}", receivers.ToString(", "));
		}
		
		/// <summary>
		/// Removes invalid path characters.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		private static string CleanPath(string s)
		{
			int index = 0;
			char[] invalidChars = Path.GetInvalidFileNameChars();
			char[] chars = s.ToCharArray();
			var sb = new StringBuilder(s);
			
			foreach(char c in chars)
			{
				if(invalidChars.Contains(c))
				{
					index = sb.IndexOf(c, index);
					sb = sb.Remove(index, 1);
				}
			}
			
			return sb.ToString();
		}
	}
}
