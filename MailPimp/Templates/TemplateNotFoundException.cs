using System;

namespace MailPimp.Templates
{
	public class TemplateNotFoundException : Exception
	{
		public TemplateNotFoundException(string path)
			: base(string.Format("Template not found [{0}]", path ?? ""))
		{
		}
	}
}