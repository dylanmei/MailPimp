using System.Collections.Generic;
using Spark;
using Spark.FileSystem;

namespace MailPimp.Templates
{
	public interface ITemplateEngine
	{
		TemplateViewResult CreateView(ITemplateFileFinder finder, TemplateLocation location);
	}

	public class TemplateEngine : ITemplateEngine
	{
        readonly ISparkViewEngine engine;
        readonly IDescriptorBuilder descriptorBuilder;

		public TemplateEngine()
		{
			engine = new SparkViewEngine(new SparkSettings {
				PageBaseType = typeof (TemplateView).FullName,
			});
			descriptorBuilder = new DescriptorBuilder(engine);
		}

		public TemplateViewResult CreateView(ITemplateFileFinder finder, TemplateLocation location)
		{
        	var viewPath = location.Directory;
        	var viewName = location.Name;
            var searchedLocations = new List<string>();
            var descriptorParams = new DescriptorParameters(
                viewPath, viewName, null, true, null);

			engine.ViewFolder = finder;
            var descriptor = descriptorBuilder.BuildDescriptor(
                descriptorParams, searchedLocations);

            if (descriptor == null)
                return new TemplateViewResult(searchedLocations);

            var entry = engine.CreateEntry(descriptor);
            var view = entry.CreateInstance() as TemplateView;

            return new TemplateViewResult(view);			
		}
	}
}