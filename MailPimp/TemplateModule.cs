using System;
using System.Dynamic;
using Nancy;
using Nancy.ModelBinding;

namespace MailPimp
{
	public class DynamicViewModel
	{
		public object From { get; set; }
		public ExpandoObject Model { get; set; }
	}

	public class HappyModel
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}

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
				//dynamic model = this.Bind<ExpandoObject>();
				throw new NotImplementedException();
			};
			Post["/{name}/render"] = parameters => {
				var model = this.Bind<EmailModel>();
				//var viewModel = new DynamicViewModel {
				//    From = new {Name = model.From.Name, Email = model.From.Email},
				//    Model = model.Model
				//};
				//var viewModel = new HappyModel {
				//    Name = model.Name,
				//    Value = model.Value
				//};
				return View["happy.spark", model];
			};
		}
	}
}