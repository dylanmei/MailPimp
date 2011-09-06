
using Nancy;

namespace MailPimp.Templates
{
	class Bootstrapper : DefaultNancyBootstrapper
	{
		
		protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer(container);
			container.Register<ITemplateEngine, TemplateEngine>().AsSingleton();
		}
	}
}