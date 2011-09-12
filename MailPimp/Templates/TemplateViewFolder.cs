using System;
using System.Collections.Generic;
using System.Linq;
using Spark.FileSystem;

namespace MailPimp.Templates
{
	public interface ITemplateViewFolder : IViewFolder
	{
	}

	public class TemplateViewFolder : ITemplateViewFolder
	{
		readonly ITemplateRepository repository;

		public TemplateViewFolder(ITemplateRepository repository)
		{
			this.repository = repository;
		}

		public IList<string> ListViews(string path)
		{
			return repository.Templates
				.Where(l => IsTemplateInFolder(l, path))
				.Select(l => l.Name).ToList();
		}

		public bool HasView(string path)
		{
			return repository.Templates.Any(l => IsTemplateInPath(l, path));
		}

		bool IsTemplateInFolder(Template template, string path)
		{
			return repository.IsTemplateInFolder(template, path);
		}

		bool IsTemplateInPath(Template template, string path)
		{
			return repository.IsTemplateInPath(template, path);
		}

		public IViewFile GetViewSource(string path)
		{
			if (!HasView(path))
				throw new TemplateNotFoundException(path);
			return new TemplateViewFile(repository.Templates.First(l => IsTemplateInPath(l, path)));
		}
	}
}