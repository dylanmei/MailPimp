
using Nancy.ViewEngines;
using TinyIoC;
using Nancy;

namespace MailPimp.ViewEngine
{
	public class ViewEngineBoostrapper : DefaultNancyBootstrapper
	{
		protected override void InitialiseInternal(TinyIoCContainer container)
		{
			base.InitialiseInternal(container);
			container.Register<IViewLocationProvider, DistantViewLocationProvider>();
		}
	}	
}
