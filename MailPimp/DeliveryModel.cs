using System.Collections.Generic;

namespace MailPimp
{
	public class DeliveryModel
	{
		public DeliveryModel()
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