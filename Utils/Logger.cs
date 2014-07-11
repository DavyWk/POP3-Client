using System;
using System.IO;

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
	}

	public static class Logger
	{
		
		private static ELogTypes status;
		private static TextWriter writer =  Console.Out;

		/// <summary>
		/// Retrieves the type of the last logged message
		/// </summary>
		public static ELogTypes Status
		{
			get {return status;}
			private set { status = value;}
		}
		
		public static void Command(string format, params object[] args)
		{
			string s = string.Format(format, args);
			
			if (s.StartsWith("+OK"))
				LogConsole(ELogTypes.Success, s.Replace("+OK",string.Empty).Capitalize());
			else if (s.StartsWith("-ERR"))
				LogConsole(ELogTypes.Error, s.Replace("-ERR", string.Empty).Capitalize());
			else
				LogConsole(ELogTypes.Unknown, s);
		}

		public static void Info(string format, params object[] args)
		{
			LogConsole(ELogTypes.Info, format, args);
		}

		public static void Network(string format, params object[] args)
		{
			LogConsole(ELogTypes.Network, format, args);
		}

		public static void Debug(string format, params object[] args)
		{
			LogConsole(ELogTypes.Debug, format, args);
		}

		public static void Error(string format, params object[] args)
		{
			LogConsole(ELogTypes.Error, format, args);
		}

		public static void Success(string format, params object[] args)
		{
			LogConsole(ELogTypes.Success, format, args);
		}

		public static void Unknown(string format, params object[] args)
		{
			LogConsole(ELogTypes.Unknown, format, args);
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

		public static void LogConsole(ELogTypes logType, string format, params object[] args)
		{
			ConsoleColor c = Console.ForegroundColor;

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
			
			if(args.Length > 0)
				Console.WriteLine(format, args);
			else
				Console.WriteLine(format);

			status = logType;
		}
		
		public static void LogFile(ELogTypes logType,string format, params object[] args)
		{
			string formatted = string.Format(format,args);
			string text = string.Format("<{0}> {1}",logType.ToString(),formatted);
			
			LogFile(text);
		}
		
		public static void LogFile(string text)
		{
			if(writer == Console.Out)
			{
				writer.WriteLine(text);
			}
			else
			{
				DateTime dt = DateTime.Now;
				string time = dt.ToString("[MM-dd-yyyy@hh:mm]");
				
				writer.WriteLine("{0} {1}",time,text);
			}
		}
	}
}
