using MailPimp.Templates;
using Nancy;

namespace MailPimp
{
	public partial class Bootstrapper : DefaultNancyBootstrapper
	{
		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal(container);
			ConfigureStaticFiles(container);
		}

		protected override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer container)
		{
			base.ConfigureRequestContainer(container);
			container.Register<ITemplateBuilder, TemplateBuilder>().AsSingleton();
			container.Register<ITemplateRepository, TemplateRepository>().AsSingleton();
		}
	}
}