using Nancy;
using Nancy.ModelBinding;
using MailPimp.Mail;

namespace MailPimp
{
	public class TemplateModule : NancyModule
	{
		public TemplateModule()
			: base("/templates")
		{
			//Get["/"] = parameters => {
			//    return Response.AsJson(new {Name = "Nancy"});
			//};
			//Get["/{Name}"] = parameters => {
			//    return Response.AsJson(new {parameters.Name});
			//};
			Post["/{Name}/deliver"] = parameters => {
				string template = parameters.Name;
				var model = this.Bind<DeliveryModel>();
				Sender.Send(GetMailbag(template, model));
				return HttpStatusCode.OK;
			};
			Post["/{Name}/mailbag"] = parameters => {
				string template = parameters.Name;
				var model = this.Bind<DeliveryModel>();
				return Response.AsJson(GetMailbag(template, model));
			};
			Post["/{Name}/display"] = parameters => {
				string template = parameters.Name;
				var model = this.Bind<DeliveryModel>();
				return View[template, model];
			};
		}

		static IMailSender Sender
		{
			get
			{
				return new MailSender();
			}
		}

		Mailbag GetMailbag(string template, DeliveryModel delivery)
		{
			var mailbag = new Mailbag(delivery.From, delivery.To, delivery.Subject);

			using (var buffer = new TextStream())
			{
				View[template, delivery].Invoke(buffer);
				mailbag.Contents = buffer.ToString();
			}

			return mailbag;
		}
	}
}