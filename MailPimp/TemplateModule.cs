using Nancy;
using Nancy.ModelBinding;
using MailPimp.Mail;
using MailPimp.Templates;
using System.Linq;

namespace MailPimp
{
	public class TemplateModule : NancyModule
	{
		readonly ITemplateBuilder builder;
		readonly ITemplateRepository templates;

		public TemplateModule(ITemplateBuilder builder, ITemplateRepository templates, IMailSender sender)
			: base("/templates")
		{
			this.builder = builder;
			this.templates = templates;

			Get["/"] = parameters => {
				return View["templates", new {
					Templates = templates.Locations
						.OrderBy(t => t.Path)
				}];
			};
//			Get["/{Name}"] = parameters => {
//			    return Response.AsJson(new {parameters.Name});
//			};
			Post["/{Name}/deliver"] = parameters => {
				var model = this.Bind<TemplateModel>();
				sender.Send(GetMailbag((string)parameters.Name, model));
				return HttpStatusCode.OK;
			};
			Post["/{Name}/mailbag"] = parameters => {
				var model = this.Bind<TemplateModel>();
				return Response.AsJson(GetMailbag((string)parameters.Name, model));
			};
			Post["/{Name}/display"] = parameters => {
				var model = this.Bind<TemplateModel>();
				return Template[parameters.Name, model];
			};
		}

		TemplateRenderer Template
		{
			get { return new TemplateRenderer(builder, templates); }
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