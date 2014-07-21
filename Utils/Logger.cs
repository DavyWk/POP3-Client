﻿using System;
using System.IO;

using Core.Protocol;

namespace Utils
{
	public enum LogType
	{
		Info,
		Error,
		Debug,	
		Inbox,
		Network,
		Success,
		Unknown,
		Exception,
	}

	// Not thread-safe.
	public static class Logger
	{
		
		private static LogType status;
		private static string filePath = string.Empty;

		/// <summary>
		/// Retrieves the type of the last logged message.
		/// </summary>
		public static LogType Status
		{
			get { return status; }
			private set { status = value;}
		}
		
		public static void Command(bool file, string format,
		                           params object[] args)
		{
			string s = string.Format(format, args).Trim();
			
			if(file)
			{
				if(Protocol.CheckHeader(s))
					LogFile(LogType.Success,Protocol.RemoveHeader(s));
				else if(s.StartsWith(Constants.ERROR))
					LogFile(LogType.Error, Protocol.RemoveHeader(s));
				else
					LogFile(LogType.Unknown,s);
			}
			else
			{
				if(Protocol.CheckHeader(s))
					LogConsole(LogType.Success, Protocol.RemoveHeader(s));
				else if(s.StartsWith(Constants.ERROR))
					LogConsole(LogType.Error, Protocol.RemoveHeader(s));
				else
					LogConsole(LogType.Unknown,s);
			}
		}
		public static void Command(string format, params object[] args)
		{
			Command(false,format,args);
		}

		public static void Exception(Exception ex)
		{ // Exceptions will always be logged on both console and file.
			Error(ex.Message);
			Error(true, ex.ToString());
		}
		
		public static void Info(bool file, string format, params object[] args)
		{
			if(file)
				LogFile(LogType.Info, format, args);
			else
				LogConsole(LogType.Info, format, args);
		}
		public static void Info(string format, params object[] args)
		{
			Info(false, format, args);
		}

		public static void Network(bool file, string format,
		                           params object[] args)
		{
			if(file)
				LogFile(LogType.Network, format, args);
			else
				LogConsole(LogType.Network, format, args);
		}
		public static void Network(string format, params object[] args)
		{
			Network(false, format, args);
		}

		public static void Debug(bool file, string format, params object[] args)
		{
			if(file)
				LogFile(LogType.Debug, format, args);
			else
				LogConsole(LogType.Debug, format, args);
		}
		public static void Debug(string format, params object[] args)
		{
			Debug(false, format, args);
		}

		public static void Error(bool file,string format, params object[] args)
		{
			if(file)
				LogFile(LogType.Error, format, args);
			else
				LogConsole(LogType.Error, format, args);
		}
		public static void Error(string format, params object[] args)
		{
			Error(false, format, args);
		}

		public static void Success(bool file, string format,
		                           params object[] args)
		{
			if(file)
				LogFile(LogType.Success, format, args);
			else
				LogConsole(LogType.Success, format, args);
		}
		public static void Success(string format, params object[] args)
		{
			Success(false, format, args);
		}

		public static void Inbox(bool file,string format, params object[] args)
		{
			if(file)
				LogFile(LogType.Inbox, format, args);
			else
				LogConsole(LogType.Inbox, format, args);
		}
		public static void Inbox(string format, params object[] args)
		{
			Inbox(false, format, args);
		}

		public static void Unknown(bool file, string format,
		                           params object[] args)
		{
			if(file)
				LogFile(LogType.Unknown, format, args);
			else
				LogConsole(LogType.Unknown, format, args);
		}
		public static void Unknown(string format, params object[] args)
		{
			Unknown(false, format, args);
		}
		
		
		private static ConsoleColor GetColor(LogType type)
		{
			ConsoleColor c = Console.ForegroundColor;
			
			switch (type)
			{
				case LogType.Info:
					c = ConsoleColor.White;
					break;
				case LogType.Network:
					c = ConsoleColor.Cyan;
					break;
				case LogType.Debug:
					c = ConsoleColor.DarkGreen;
					break;
				case LogType.Error:
					c = ConsoleColor.Red;
					break;
				case LogType.Success:
					c = ConsoleColor.Green;
					break;
				case LogType.Unknown:
					c = ConsoleColor.Yellow;
					break;
				case LogType.Inbox:
					c = ConsoleColor.Magenta;
					break;

				default:
					break;
			}
			
			return c;
		}
		
		public static void LogConsole(LogType type, string format,
		                              params object[] args)
		{
			ConsoleColor c = Console.ForegroundColor;
			string formatted;
			// Helps avoiding some FormatExceptions.
			if(args.Length == 0)
				formatted = format;
			else
				formatted = string.Format(format, args);

			status = type;
			
			Console.ForegroundColor = GetColor(type);
			string colored = string.Format("<{0}>", type.ToString());
			Console.Write("{0, -9} ", colored);
			Console.ResetColor();
			
			Console.WriteLine(formatted);
			
		}
		

		public static void LogFile(LogType type,string format,
		                           params object[] args)
		{
			string formatted = string.Format(format, args);
			string header = string.Format("<{0}>", type.ToString());
			string text = string.Format("{0, -9} {1}",
			                            header, formatted);
			
			status = type;
			
			LogFile(text);
		}
		
		
		private static void LogFile(string text)
		{
			if(filePath == string.Empty)
				return;
			
			DateTime dt = DateTime.Now;
			string time = dt.ToString("[MM-dd-yyyy@hh:mm]");
			
			using(var sw = File.AppendText(filePath))
			{
				sw.WriteLine("{0} {1}", time, text);
			}
		}
		
		public static void Bind(string path)
		{
			filePath = path;
		}
		
		public static void UnBind()
		{
			filePath = string.Empty;
		}
	}
}
