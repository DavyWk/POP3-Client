using System.Net;

namespace Core.Mail
{
	public struct SenderInfo
	{
		public IPAddress Address;
		public string Name;
		public string EMailAddress;
		
		public SenderInfo(IPAddress address, string name, string emailAddress)
		{
			Address = address;
			Name = name;
			EMailAddress = emailAddress;
		}
	}
	
}
