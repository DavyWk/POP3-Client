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
		/// <remarks>/!\ Will mess up if the string
		/// is not correctly formatted.</remarks>
		public static string RemoveHeader(string s)
		{
			// Just in case.
			s = s.Trim();
			// Index of the beginning of the message.
			int index = s.IndexOf(" ") + 1;

			s = s.Substring(index, s.Length - index);
			
			return s;
		}
	}
}
