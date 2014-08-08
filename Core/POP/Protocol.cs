namespace Core.POP
{
	public static class Protocol
	{
		public static bool CheckHeader(string s)
		{
			if(s.TrimStart().StartsWith(Constants.OK))
				return true;
			else
				return false;
		}
		
		/// <summary>
		/// Removes the "+OK"/"-ERR" header.
		/// </summary>
		/// <remarks>Also removes the trailling space.</remarks>
		public static string RemoveHeader(string s)
		{
			s = s.Trim();
			
			if(CheckHeader(s))
				s = s.Replace("+OK ", string.Empty);
			else
				s = s.Replace("-ERR ", string.Empty);
			
			return s;
		}
	}
}
