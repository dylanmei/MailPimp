
using System.Collections.Generic;
using MailPimp.ViewEngine;

namespace MailPimp
{
	public abstract class EmailView : View
	{
		public Address From { get; set; }
		public ICollection<Address> To { get; set; }
		public string Subject { get; set; }

		public override void Bind(dynamic model)
		{
			if (model is EmailModel)
				Bind((EmailModel)model);
			Model = model.Model;
		}

		void Bind(EmailModel model)
		{
			From = model.From;
			To = model.To;
			Subject = model.Subject;
		}
	}
}