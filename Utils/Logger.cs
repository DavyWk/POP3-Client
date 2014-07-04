using System;

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
        public static void Command(string format, params object[] args)
        {
            string s = string.Format(format, args);
            
            if (s.StartsWith("+OK"))
                Log(ELogTypes.Success, s.Replace("+OK",string.Empty).Capitalize());
            else if (s.StartsWith("-ERR"))
                Log(ELogTypes.Error, s.Replace("-ERR", string.Empty).Capitalize());
            else
                Log(ELogTypes.Unknown, s);
        }

        public static void Info(string format, params object[] args)
        {
            Log(ELogTypes.Info, format, args);
        }

        public static void Network(string format, params object[] args)
        {
            Log(ELogTypes.Network, format, args);
        }

        public static void Debug(string format, params object[] args)
        {
            Log(ELogTypes.Debug, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Log(ELogTypes.Error, format, args);
        }

        public static void Success(string format, params object[] args)
        {
            Log(ELogTypes.Success, format, args);
        }

        public static void Unknown(string format, params object[] args)
        {
            Log(ELogTypes.Unknown, format, args);
        }

        public static void Inbox(string format, params object[] args)
        {
            Log(ELogTypes.Inbox, format, args);
        }

        public static void Log(ELogTypes logType, string format, params object[] args)
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
            Console.WriteLine(format, args);
        }
    }
}
