using System.Collections.Generic;
using Spark;

namespace MailPimp.Templates
{
	public interface ITemplateEngine
	{
		TemplateViewResult CreateView(ITemplateViewFolder finder, Template template);
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

		public TemplateViewResult CreateView(ITemplateViewFolder finder, Template template)
		{
        	var viewPath = template.Folder;
        	var viewName = template.Name;
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