using System.Collections.Generic;

namespace MailPimp.Templates
{
	public interface ITemplateRepository
	{
		IEnumerable<Template> Templates { get; }
		bool IsTemplateInFolder(Template template, string path);
		bool IsTemplateInPath(Template template, string path);
	}
}