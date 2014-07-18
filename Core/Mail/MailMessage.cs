using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;


using Utils;


namespace Core.Mail
{
	public struct POPMessage
	{
		public string ID;
		public List<Person> Receivers;
		public Person Sender;
		public string Body;
		public string Subject;
		public Encoding CharSet;
		public DateTime ArrivalTime;
		public bool ContainsHTML;
		
	}

}
