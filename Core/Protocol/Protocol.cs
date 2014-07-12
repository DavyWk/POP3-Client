namespace Core.Protocol
{
	public static class Protocol
	{
		public static bool CheckHeader(string s)
		{
			if(s.StartsWith(Constants.OK))
				return true;
			else
				return false;
		}
		
		public static string RemoveHeader(string s)
		{
			int index = s.IndexOf(" ") + 1;
			s = s.Trim();
			
			// 0 because it should be the first character in the string.
			if(s.StartsWith(Constants.OK))
			{
				s = s.Substring(index,s.Length - index);
			}
			else if(s.StartsWith(Constants.ERROR))
			{
				s = s.Substring(index,s.Length - index);
			}
			
			return s;
		}
	}
}
