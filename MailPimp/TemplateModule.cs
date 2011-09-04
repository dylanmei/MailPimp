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
			Get["/"] = parameters => {
				return Response.AsJson(new {Name = "Nancy"});
			};
			Get["/{Name}"] = parameters => {
				return Response.AsJson(new {parameters.Name});
			};
			Post["/{name}/deliver"] = parameters => {
				throw new NotImplementedException();
			};
			Post["/{name}/display"] = parameters => {
				var model = this.Bind<DeliveryModel>();

				return View["happy.spark", model];
			};
		}
	}
}