using System.Collections.Generic;
using MailPimp.ViewEngine;

namespace MailPimp
{
	public abstract class TemplateView : View
	{
		public Address From { get; set; }
		public ICollection<Address> To { get; set; }
		public string Subject { get; set; }

		public override void Bind(dynamic model)
		{
			if (model is DeliveryModel)
				Bind((DeliveryModel)model);
			Model = model.Model;
		}

		void Bind(DeliveryModel model)
		{
			From = model.From;
			To = model.To;
			Subject = model.Subject;
		}
	}
}