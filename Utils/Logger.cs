using System;
using System.IO;

using Core.Protocol;

namespace Utils
{
	public enum ELogTypes
	{
		Info,
		Network,
		Debug,
		Error,
		Success,
		Unknown,
		Inbox,
		Exception,
	}

	public static class Logger
	{
		
		private static ELogTypes status;
		private static TextWriter writer =  Console.Out;
		private static string filePath = string.Empty;

		/// <summary>
		/// Retrieves the type of the last logged message.
		/// </summary>
		public static ELogTypes Status
		{
			get { return status; }
			private set { status = value;}
		}
		
		public static void Command(bool file, string format,params object[] args)
		{
			string s = string.Format(format,args);
			
			if(file)
			{
				if(Protocol.CheckHeader(s))
					LogFile(ELogTypes.Success,s.Replace(Constants.OK,string.Empty).Trim());
				else if(s.StartsWith(Constants.ERROR))
					LogFile(ELogTypes.Error,s.Replace(Constants.ERROR,string.Empty).Trim());
				else
					LogFile(ELogTypes.Unknown,s);
			}
			else
			{
				if(Protocol.CheckHeader(s))
					LogConsole(ELogTypes.Success,s.Replace(Constants.OK,string.Empty).Trim());
				else if(s.StartsWith(Constants.ERROR))
					LogConsole(ELogTypes.Error,s.Replace(Constants.ERROR,string.Empty).Trim());
				else
					LogConsole(ELogTypes.Unknown,s);
			}
		}
		public static void Command(string format, params object[] args)
		{
			Command(false,format,args);
		}

		public static void Exception(Exception ex)
		{
			Error(ex.Message);
			Error(true,ex.ToString());
			
		}
		public static void Info(bool file, string format, params object[] args)
		{
			if(file)
				LogFile(ELogTypes.Info,format,args);
			else
				LogConsole(ELogTypes.Info,format,args);
		}
		public static void Info(string format, params object[] args)
		{
			Info(false,format,args);
		}

		public static void Network(bool file, string format, params object[] args)
		{
			if(file)
				LogFile(ELogTypes.Network,format,args);
			else
				LogConsole(ELogTypes.Network,format,args);
		}
		public static void Network(string format, params object[] args)
		{
			Network(false,format,args);
		}

		public static void Debug(bool file, string format, params object[] args)
		{
			if(file)
				LogFile(ELogTypes.Debug,format,args);
			else
				LogConsole(ELogTypes.Debug,format,args);
		}
		public static void Debug(string format, params object[] args)
		{
			Debug(false,format,args);
		}

		public static void Error(bool file,string format, params object[] args)
		{
			if(file)
				LogFile(ELogTypes.Error,format,args);
			else
				LogConsole(ELogTypes.Error,format,args);
		}
		public static void Error(string format, params object[] args)
		{
			Error(false,format,args);
		}

		public static void Success(bool file, string format, params object[] args)
		{
			if(file)
				LogFile(ELogTypes.Success,format,args);
			else
				LogConsole(ELogTypes.Success,format,args);
		}
		public static void Success(string format, params object[] args)
		{
			Success(false, format, args);
		}

		public static void Inbox(bool file,string format, params object[] args)
		{
			if(file)
				LogFile(ELogTypes.Inbox,format,args);
			else
				LogConsole(ELogTypes.Inbox,format,args);
		}
		public static void Inbox(string format, params object[] args)
		{
			Inbox(false, format, args);
		}

		public static void Unknown(bool file, string format, params object[] args)
		{
			if(file)
				LogFile(ELogTypes.Unknown,format,args);
			else
				LogConsole(ELogTypes.Unknown,format,args);
		}
		public static void Unknown(string format, params object[] args)
		{
			Unknown(false,format,args);
		}
		
		
		public static void LogConsole(ELogTypes logType, string format, params object[] args)
		{
			ConsoleColor c = Console.ForegroundColor;
			string formatted = string.Format(format,args);

			status = logType;
			
			switch (logType)
			{
				case ELogTypes.Info:
					c = ConsoleColor.White;
					break;
				case ELogTypes.Network:
					c = ConsoleColor.Cyan;
					break;
				case ELogTypes.Debug:
					c = ConsoleColor.DarkGreen;
					break;
				case ELogTypes.Error:
					c = ConsoleColor.Red;
					break;
				case ELogTypes.Success:
					c = ConsoleColor.Green;
					break;
				case ELogTypes.Unknown:
					c = ConsoleColor.Yellow;
					break;
				case ELogTypes.Inbox:
					c = ConsoleColor.Magenta;
					break;

				default:
					break;
			}
			
			Console.ForegroundColor = c;
			Console.Write("<{0}> ", logType.ToString());
			Console.ResetColor();
			
			Console.WriteLine(formatted);
			
		}
		

		public static void LogFile(ELogTypes logType,string format, params object[] args)
		{
			string formatted = string.Format(format,args);
			string text = string.Format("<{0}> {1}",logType.ToString(),formatted);
			
			status = logType;
			
			if((filePath != string.Empty) && (writer == Console.Out))
			   writer = File.AppendText(filePath);
			
			LogFile(text);
		}
		
		
		private static void LogFile(string text)
		{
				DateTime dt = DateTime.Now;
				string time = dt.ToString("[MM-dd-yyyy@hh:mm]");
				
				writer.WriteLine("{0} {1}",time,text);
				writer.Flush();
				writer.Dispose();
				writer = Console.Out;
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
