using System;
using System.Text;

namespace Utils
{
	public static class Helpers
	{

		public static string ReadPassword(string msg)
		{
			Console.Write(msg);
			
			return ReadPassword();
		}
		
		public static string ReadPassword()
		{
			var sb = new StringBuilder();
			ConsoleKeyInfo key;
			int initialLeft = Console.CursorLeft;
			
			do
			{
				key = Console.ReadKey(true);
				
				if(key.Key == ConsoleKey.Backspace)
				{
					if(sb.Length > 0 && Console.CursorLeft > initialLeft)
					{
						sb.Remove(sb.Length - 1, 1);
						Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
						Console.Write(' ');
						Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
					}
					continue;
				}
				
				if(((int)key.KeyChar >= 30) && ((int)key.KeyChar <= 126))
				{
					sb.Append(key.KeyChar);
					Console.Write('*');
				}
			} while (key.Key != ConsoleKey.Enter);
			
			Console.WriteLine();
			
			return sb.ToString();
		}
	}
}
