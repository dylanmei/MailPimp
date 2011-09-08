
using System;
using System.IO;
using MailPimp.Templates;
using Nancy;

namespace MailPimp
{
	public class TemplateRenderer : IHideObjectMembers
	{
		private readonly ITemplateBuilder builder;

		public TemplateRenderer(ITemplateBuilder builder)
		{
			this.builder = builder;
		}

		public Action<Stream> this[string templateName, TemplateModel model]
		{
			get
			{
				return builder.RenderTemplate(templateName, model);
			}
		}
	}
}