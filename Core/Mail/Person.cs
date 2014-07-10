using System.Net;

namespace Core.Mail
{
	public struct Person
	{
		public string Name;
		public string EMailAddress;
		
		public Person(string name, string emailAddress)
		{
			EMailAddress = emailAddress;
			Name = name;
		}
	}
	
}
