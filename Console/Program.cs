using System.Dynamic;
using Autofac;
using MailPimp.Storage.Amazon;
using MailPimp.Templates;

namespace MailPimp.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var builder = new ContainerBuilder();
			builder.RegisterAssemblyTypes(typeof(Template).Assembly)
				.Except<AmazonRepository>()
				.AsImplementedInterfaces();
	    	builder.RegisterInstance(new AmazonRepository("XXX"))
				.As<ITemplateRepository>();
			Render(builder.Build());
			System.Console.ReadLine();
		}

		static void Render(IContainer container)
		{
			var b = container.Resolve<ITemplateBuilder>();
			using (var stream = new TextStream())
			{
				var writer = b.RenderTemplate("muppets", GetModel());
				writer(stream);
				System.Console.Write(stream.ToString());
			}
		}

		static TemplateModel GetModel()
		{
			var model = new TemplateModel();
			model.From.Email = "kermit@muppets.com";
			model.From.Name = "Kermit";
			model.To.Add(new Address { Email = "jim@henson.com", Name = "Jim" });
			model.Subject = "Message from Kermit via MailPimp";

			model.Model = new ExpandoObject();
			model.Model.Message = "Hey boss, Miss Piggy is smokin' hot!";
			model.Model.Muppets = new ExpandoObject[3];
			model.Model.Muppets[0] = new ExpandoObject();
			model.Model.Muppets[0].Name = "Fozzie";
			model.Model.Muppets[1] = new ExpandoObject();
			model.Model.Muppets[1].Name = "Animal";
			model.Model.Muppets[2] = new ExpandoObject();
			model.Model.Muppets[2].Name = "Gonzo";

			return model;
		}
	}
}
