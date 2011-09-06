
using System;
using System.IO;
using MailPimp.Templates;
using Nancy;

namespace MailPimp
{
	public class TemplateRenderer : IHideObjectMembers
	{
		private readonly TemplateModule module;

		public TemplateRenderer(TemplateModule module)
		{
			this.module = module;
		}

		public Action<Stream> this[string templateName, TemplateModel model]
		{
			get
			{
				return module.Engine.RenderTemplate(templateName, model);
			}
		}
	}
}