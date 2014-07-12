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
	}
}
