namespace Core.Protocol
{
	public static class Constants
	{
		public const string MultiLineTerminator = ".";
			
		public const string Terminator = "\r\n";
		public const short TerminatorLength = 2;
		
		public const string OK = "+OK";
		public const string ERROR = "-ERR";
	}
}