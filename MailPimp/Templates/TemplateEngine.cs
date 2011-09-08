using System.Collections.Generic;
using Spark;

namespace MailPimp.Templates
{
	public interface ITemplateEngine
	{
		TemplateViewResult CreateView(TemplateLocation location);
	}

	public class TemplateEngine : ITemplateEngine
	{
        readonly IDescriptorBuilder descriptorBuilder;
        readonly ISparkViewEngine engine;
        readonly ISparkSettings settings;

		public TemplateEngine(ITemplateFileFinder fileFinder)
		{
			settings = new SparkSettings {
				PageBaseType = typeof (TemplateView).FullName,
			};
			engine = new SparkViewEngine(settings) {
				ViewFolder = fileFinder
			};
			descriptorBuilder = new DescriptorBuilder(engine);
		}

		public TemplateViewResult CreateView(TemplateLocation location)
		{
        	var viewPath = location.Directory;
        	var viewName = location.Name;
            var searchedLocations = new List<string>();
            var descriptorParams = new DescriptorParameters(
                GetNamespaceEncodedPathViewPath(viewPath),
                viewName,
                null,
                true,
                null);

            var descriptor = descriptorBuilder.BuildDescriptor(
                descriptorParams, searchedLocations);

            if (descriptor == null)
                return new TemplateViewResult(searchedLocations);

            var entry = engine.CreateEntry(descriptor);
            var view = entry.CreateInstance() as TemplateView;
			//if (view != null)
			//{
			//    view.RenderContext = renderContext;
			//}

            return new TemplateViewResult(view);			
		}

		static string GetNamespaceEncodedPathViewPath(string viewPath)
		{
		    //return viewPath.Replace('/', '_');
			return viewPath;
		}		
	}
}