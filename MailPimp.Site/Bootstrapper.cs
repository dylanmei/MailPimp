using System;
using Autofac;
using MailPimp.Templates;
using MailPimp.Storage.Amazon;
using Nancy.Bootstrappers.Autofac;

namespace MailPimp
{
	public partial class Bootstrapper : AutofacNancyBootstrapper
	{
		internal T Resolve<T>()
        {
            return ApplicationContainer.Resolve<T>();
        }

		protected override void InitialiseInternal(ILifetimeScope container)
		{
			base.InitialiseInternal(container);
			ConfigureStaticFiles();
		}

		protected override void ConfigureApplicationContainer(ILifetimeScope container)
		{
			base.ConfigureApplicationContainer(container);
			var builder = new ContainerBuilder();
			builder.RegisterAssemblyTypes(typeof (Template).Assembly)
				.AsImplementedInterfaces();
			builder.Update(container.ComponentRegistry);
		}

	    protected override void ConfigureRequestContainer(ILifetimeScope container)
	    {
	        base.ConfigureRequestContainer(container);
	    	var builder = new ContainerBuilder();
	    	builder.RegisterType<TemplateBuilder>()
				.As<ITemplateBuilder>();
	    	builder.RegisterInstance(new AmazonRepository(BucketConfig.GetName()))
				.As<ITemplateRepository>();
			builder.Update(container.ComponentRegistry);
	    }
	}
}