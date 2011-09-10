using System;
using System.IO;

namespace MailPimp.Templates
{
	public interface ITemplateBuilder
	{
		Action<Stream> RenderTemplate(ITemplateFileFinder finder, TemplateLocation location, TemplateModel model);
	}

	public class TemplateBuilder : ITemplateBuilder
	{
		readonly ITemplateEngine engine;
		static readonly Action<Stream> EmptyView = x => {};

		public TemplateBuilder(ITemplateEngine engine)
		{
			this.engine = engine;
		}

		public Action<Stream> RenderTemplate(ITemplateFileFinder finder, TemplateLocation location, TemplateModel model)
		{
			return location == null
				? EmptyView
				: RenderView(finder, location, model);
		}

		Action<Stream> RenderView(ITemplateFileFinder finder, TemplateLocation location, TemplateModel model)
		{
            return stream => {
                var result = engine.CreateView(finder, location);
                using (var writer = new StreamWriter(stream))
                {
					result.View.Bind(model);
					result.View.Execute(writer);
					writer.Flush();
                }
            };
		}
	}
}