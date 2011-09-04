using System;
using Nancy;
using Nancy.ModelBinding;

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
			Post["/{name}/deliver"] = parameters => {
				throw new NotImplementedException();
			};
			Post["/{name}/mailbag"] = parameters => {
				var model = this.Bind<DeliveryModel>();
				return Response.AsJson(GetMailbag(model));
			};
			Post["/{name}/display"] = parameters => {
				var model = this.Bind<DeliveryModel>();
				return View["happy.spark", model];
			};
		}

		Mailbag GetMailbag(DeliveryModel delivery)
		{
			var mailbag = new Mailbag(delivery.From, delivery.To, delivery.Subject);

			using (var buffer = new TextStream())
			{
				View["happy.spark", delivery].Invoke(buffer);
				mailbag.Contents = buffer.ToString();
			}

			return mailbag;
		}
	}
}