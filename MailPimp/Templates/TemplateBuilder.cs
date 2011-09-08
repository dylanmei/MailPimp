using System;
using System.IO;
using System.Linq;

namespace MailPimp.Templates
{
	public interface ITemplateBuilder
	{
		Action<Stream> RenderTemplate(string templateName, TemplateModel model);
	}

	public class TemplateBuilder : ITemplateBuilder
	{
		readonly ITemplateRepository templates;
		readonly ITemplateEngine engine;
		static readonly Action<Stream> EmptyView = x => {};

		public TemplateBuilder(ITemplateEngine engine, ITemplateRepository templates)
		{
			this.engine = engine;
			this.templates = templates;
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
                var result = engine.CreateView(location);
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