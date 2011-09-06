using Nancy;
using Nancy.ModelBinding;
using MailPimp.Mail;
using MailPimp.Templates;
using System.Linq;

namespace MailPimp
{
	public class TemplateModule : NancyModule
	{
		readonly IMailSender sender;
		readonly ITemplateEngine engine;

		public TemplateModule(IMailSender sender, ITemplateEngine engine)
			: base("/templates")
		{
			this.engine = engine;
			this.sender = sender;

			Get["/"] = parameters => {
				return View["templates", new {
					Templates = engine.GetTemplateLocations()
						.OrderBy(t => t.Path)
				}];
			};
//			Get["/{Name}"] = parameters => {
//			    return Response.AsJson(new {parameters.Name});
//			};
			Post["/{Name}/deliver"] = parameters => {
				string template = parameters.Name;
				var model = this.Bind<TemplateModel>();
				Sender.Send(GetMailbag(template, model));
				return HttpStatusCode.OK;
			};
			Post["/{Name}/mailbag"] = parameters => {
				string template = parameters.Name;
				var model = this.Bind<TemplateModel>();
				return Response.AsJson(GetMailbag(template, model));
			};
			Post["/{Name}/display"] = parameters => {
				string template = parameters.Name;
				var model = this.Bind<TemplateModel>();
				return Template[template, model];
			};
		}

		public IMailSender Sender
		{
			get { return sender; }
		}

		public ITemplateEngine Engine
		{
			get { return engine; }
		}

		public TemplateRenderer Template
		{
			get { return new TemplateRenderer(this); }
		}

		Mailbag GetMailbag(string template, TemplateModel delivery)
		{
		    var mailbag = new Mailbag(delivery.From, delivery.To, delivery.Subject);

		    using (var buffer = new TextStream())
		    {
		        Template[template, delivery].Invoke(buffer);
		        mailbag.Contents = buffer.ToString();
		    }

		    return mailbag;
		}
	}
}