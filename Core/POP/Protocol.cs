namespace Core.POP
{
	public static class Protocol
	{
		public static bool CheckHeader(string s)
		{
			if(s.Trim().StartsWith(Constants.OK))
				return true;
			else
				return false;
		}
		
		public static string RemoveHeader(string s)
		{
			// Just in case.
			s = s.Trim();
			// Index of the beginning of the message.
			int index = s.IndexOf(" ") + 1;

			
			if(s.StartsWith(Constants.OK))
			{
				s = s.Substring(index, s.Length - index);
			}
			else if(s.StartsWith(Constants.ERROR))
			{
				s = s.Substring(index, s.Length - index);
			}
			
			return s;
		}
	}
}
