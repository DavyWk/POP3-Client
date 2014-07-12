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
			int index;
			
			if((index = s.IndexOf(Constants.OK)) > 0)
			{
				s = s.Substring(index,s.Length - index);
			}
			else if((index = s.IndexOf(Constants.ERROR)) > 0)
			{
				s = s.Substring(index,s.Length - index);
			}
			
			return s;
		}
	}
}
