using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Spark;
using Spark.FileSystem;
using Nancy.ViewEngines;

namespace MailPimp.ViewEngine
{
    public class TemplateViewEngine : IViewEngine
    {
        readonly IDescriptorBuilder descriptorBuilder;
        readonly ISparkViewEngine engine;
        readonly ISparkSettings settings;

        public TemplateViewEngine()
        {
            settings = (ISparkSettings) ConfigurationManager.GetSection("spark") ?? new SparkSettings();
            engine = new SparkViewEngine(settings) {
                DefaultPageBaseType = typeof(TemplateView).FullName
            };

            descriptorBuilder = new DescriptorBuilder(engine);
        }

        public IEnumerable<string> Extensions
        {
            get { return new[] {"spark"}; }
        }

        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return stream => {
                var result = CreateView(viewLocationResult, renderContext);
                using (var writer = new StreamWriter(stream))
                {
					result.View.Bind(model);
					result.View.Execute(writer);
					writer.Flush();
                }
            };
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            engine.ViewFolder = GetMemoryViewMap(viewEngineStartupContext.ViewLocationResults);
        }

        ViewResult CreateView(ViewLocationResult viewLocationResult, IRenderContext renderContext)
        {
        	var viewPath = viewLocationResult.Location;
        	var viewName = viewLocationResult.Name;
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
                return new ViewResult(searchedLocations);

            var entry = renderContext.ViewCache.GetOrAdd(
                viewLocationResult, 
                x => engine.CreateEntry(descriptor));

            var view = entry.CreateInstance() as View;
            if (view != null)
            {
                view.RenderContext = renderContext;
            }

            return new ViewResult(view);
        }

        static InMemoryViewFolder GetMemoryViewMap(IEnumerable<ViewLocationResult> viewLocationResults)
        {
            var memoryViewMap = new InMemoryViewFolder();
            foreach (var viewLocationResult in viewLocationResults)
            {
                memoryViewMap.Add(GetViewFolderKey(viewLocationResult), viewLocationResult.Contents.Invoke().ReadToEnd());
            }
            return memoryViewMap;
        }

        static string GetViewFolderKey(ViewLocationResult viewLocationResult)
        {
            return string.Concat(GetNamespaceEncodedPathViewPath(viewLocationResult.Location),
				Path.DirectorySeparatorChar, viewLocationResult.Name, ".", viewLocationResult.Extension);
        }

        static string GetNamespaceEncodedPathViewPath(string viewPath)
        {
            return viewPath.Replace('/', '_');
        }
    }
}