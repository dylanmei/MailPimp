using System.Collections.Generic;

namespace MailPimp
{
	public class EmailModel
	{
		public EmailModel()
		{
			To = new List<Address>();
			From = new Address();
		}

		public Address From { get; private set; }
		public List<Address> To { get; private set; }
		public string Subject { get; set; }

		public dynamic Model { get; set; }
	}
}