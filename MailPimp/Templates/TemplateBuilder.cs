using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MailPimp.Templates
{
	public interface ITemplateBuilder
	{
		IEnumerable<Template> Templates { get; }
		Action<Stream> RenderTemplate(string templateName, TemplateModel model);
	}

	public class TemplateBuilder : ITemplateBuilder
	{
		readonly ITemplateEngine engine;
		readonly ITemplateRepository templates;
		static readonly Action<Stream> EmptyView = x => {};

		public TemplateBuilder(ITemplateEngine engine, ITemplateRepository templates)
		{
			this.engine = engine;
			this.templates = templates;
		}

		public IEnumerable<Template> Templates
		{
			get
			{
				return templates.Templates
					.Where(l => l.Folder == "");
			}
		}

		public Action<Stream> RenderTemplate(string templateName, TemplateModel model)
		{
			var location = Templates
				.FirstOrDefault(l => l.Name == templateName);

			return location == null
				? EmptyView
				: RenderView(location, model);
		}

		Action<Stream> RenderView(Template location, TemplateModel model)
		{
            return stream => {
                var result = engine.CreateView(new TemplateViewFolder(templates), location);
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