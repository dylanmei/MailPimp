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

	        model.Model = new MuppetModel {
	            Message = "Hey boss, Miss Piggy is smokin' hot!",
	            Muppets = new[] {
	                new Muppet {Name = "Gonzo"},
	                new Muppet {Name = "Animal"},
	                new Muppet {Name = "Fozzie"}
	            }
	        };

	        return model;
	    }
	}

	public class MuppetModel
	{
	    public string Message { get; set; }
	    public Muppet[] Muppets { get; set; }
	}

	public class Muppet
	{
	    public string Name { get; set; }
	}
}
