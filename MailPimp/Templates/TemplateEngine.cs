using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spark;

namespace MailPimp.Templates
{
	public interface ITemplateEngine
	{
		IEnumerable<TemplateLocation> GetTemplateLocations();
		Action<Stream> RenderTemplate(string templateName, TemplateModel model);
	}

	public class TemplateEngine : ITemplateEngine
	{
		readonly ITemplateRepository templates;
        readonly IDescriptorBuilder descriptorBuilder;
        readonly ISparkViewEngine engine;
        readonly ISparkSettings settings;
		static readonly Action<Stream> EmptyView = x => {};

		public TemplateEngine(ITemplateRepository templates)
		{
			this.templates = templates;
			settings = new SparkSettings {
				PageBaseType = typeof (TemplateView).FullName,
			};
			engine = new SparkViewEngine(settings) {
				ViewFolder = templates
			};
			descriptorBuilder = new DescriptorBuilder(engine);
		}

		public IEnumerable<TemplateLocation> GetTemplateLocations()
		{
			return templates.Locations;
		}

		public Action<Stream> RenderTemplate(string templateName, TemplateModel model)
		{
			var location = templates.Locations
				.FirstOrDefault(l => l.Name == templateName);
			return location == null
				? EmptyView
				: RenderView(location, model);
		}

		Action<Stream> RenderView(TemplateLocation location, TemplateModel model)
		{
            return stream => {
                var result = CreateView(location);
                using (var writer = new StreamWriter(stream))
                {
					result.View.Bind(model);
					result.View.Execute(writer);
					writer.Flush();
                }
            };
		}

		TemplateViewResult CreateView(TemplateLocation location)
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