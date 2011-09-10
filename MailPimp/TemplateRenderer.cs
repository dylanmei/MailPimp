using System;
using System.IO;
using System.Linq;
using MailPimp.Templates;
using Nancy;

namespace MailPimp
{
	public class TemplateRenderer : IHideObjectMembers
	{
		readonly ITemplateBuilder builder;
		readonly ITemplateRepository templates;

		public TemplateRenderer(ITemplateBuilder builder, ITemplateRepository templates)
		{
			this.builder = builder;
			this.templates = templates;
		}

		public Action<Stream> this[string templateName, TemplateModel model]
		{
			get
			{
				var location = templates.Locations
					.FirstOrDefault(l => l.Name == templateName);
				return builder.RenderTemplate(new TemplateFileFinder(templates), location, model);
			}
		}
	}
}