using System;
using System.Security;

namespace Utils
{
	public static class Utilities
	{
		public static SecureString ReadPassword()
		{
			SecureString s = new SecureString();

			ConsoleKeyInfo key;
			do
			{
				key = Console.ReadKey(true);

				if (key.Key == ConsoleKey.Backspace)
				{
					if (s.Length > 0)
					{
						s.RemoveAt(s.Length - 1);
						Console.Write(key.KeyChar);
						Console.Write(" ");
						Console.Write(key.KeyChar);
					}

					continue;
				}

				// accept only letters
				if ((ConsoleKey.D0 <= key.Key) && (key.Key <= ConsoleKey.Z))
				{
					s.AppendChar(key.KeyChar);
					Console.Write("*");
				}

			} while (key.Key != ConsoleKey.Enter);

			Console.WriteLine();
			s.MakeReadOnly();

			return s;
		}
	}
}