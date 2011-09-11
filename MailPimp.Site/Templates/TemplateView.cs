using System.Collections.Generic;
using System.IO;
using Spark;

namespace MailPimp.Templates
{
	public abstract class TemplateView : SparkViewBase
	{
		public Address From { get; set; }
		public ICollection<Address> To { get; set; }
		public string Subject { get; set; }
        public dynamic Model { get; protected set; }

		public void Bind(TemplateModel model)
		{
			From = model.From;
			To = model.To;
			Subject = model.Subject;
			Model = model.Model;
		}

        public void Execute(TextWriter writer)
        {
            RenderView(writer);
        }
	}
}