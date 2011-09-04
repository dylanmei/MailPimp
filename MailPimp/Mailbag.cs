using System.Linq;
using System.Collections.Generic;

namespace MailPimp
{
	public class Mailbag
	{
		public Address From { get; set; }
		public List<Address> To { get; private set; }
		public string Subject { get; set; }
		public string Contents { get; set; }

		public Mailbag()
		{
			To = new List<Address>();
		}

		public Mailbag(Address from , IEnumerable<Address> to, string subject)
		{
			From = from;
			To = new List<Address>(to);
			Subject = subject;
		}

		public IEnumerator<Mail> GetMail()
		{
			return To.Select(t => new Mail {
				From = From,
				To = t,
				Subject = Subject,
				Contents = Contents
			}).GetEnumerator();
		}
	}
}