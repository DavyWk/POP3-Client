using System.Collections.Generic;

using Core.POP;

namespace Core.POP.CommandParser
{
	public static class StatParseer
	{
		public static KeyValuePair<int, int> Parse(string s)
		{
			if(!Protocol.CheckHeader(s))
				return new KeyValuePair<int, int>(0, 0);
			
			s = Protocol.RemoveHeader(s);
			string[] splitted = s.Split(' ');
			int nb = int.Parse(splitted[0]);
			int size = int.Parse(splitted[1]);
			
			return new KeyValuePair<int, int>(nb, size);
		}
	}
}