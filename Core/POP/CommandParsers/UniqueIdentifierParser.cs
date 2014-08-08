using Core;

namespace Core.POP.CommandParsers
{
	public static class UniqueIdentifierParser
	{
		public static string Parse(string s)
		{
			// Format:
			// +OK msgID uid
			
			s = Protocol.RemoveHeader(s);
			var element = s.Split(' ');
			
			return element[1];
		}
	}
}