

namespace Core.Mail
{
	public static class MailParsingUtils
	{
		/// <summary>
		/// Removes every non-digit character at the end of the string.
		/// </summary>
		public static string RemoveCharEnding(string s)
		{
			for(int i = s.Length - 1; i > - 1; i--)
			{
				if(!char.IsDigit(s[i]))
					s = s.Remove(i, 1);
				else
					break;
			}
			
			return s;
		}
		
		public static string RemoveParenthesisEnding(string s)
		{
			int beg = -1;
			int end = -1;
			
			beg = s.IndexOf('(') - 1; // Also remove space just before.
			end = s.IndexOf(')');
			
			if((end == -1) || (beg == -2))
				return s;
			
			s = s.Remove(beg, end - beg);
			
			return s;
		}
	}
}