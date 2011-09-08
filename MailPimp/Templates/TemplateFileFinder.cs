using System;
using System.Collections.Generic;
using System.Linq;
using Spark.FileSystem;

namespace MailPimp.Templates
{
	public interface ITemplateFileFinder : IViewFolder
	{
	}

	public class TemplateFileFinder : ITemplateFileFinder
	{
		readonly ITemplateRepository repository;

		public TemplateFileFinder(ITemplateRepository repository)
		{
			this.repository = repository;
		}

		public IList<string> ListViews(string path)
		{
			return repository.Locations
				.Where(l => IsTemplateInDirectory(l, path))
				.Select(l => l.Name).ToList();
		}

		public bool HasView(string path)
		{
			return repository.Locations.Any(l => IsTemplateAtPath(l, path));
		}

		static bool IsTemplateInDirectory(TemplateLocation location, string path)
		{
			if (path.Contains(@"\")) path = path.Replace(@"\", "/");
			return string.Compare(path, location.Directory, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		static bool IsTemplateAtPath(TemplateLocation location, string path)
		{
			if (path.Contains(@"\")) path = path.Replace(@"\", "/");
			return string.Compare(path, location.Path, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public IViewFile GetViewSource(string path)
		{
			if (!HasView(path))
				throw new TemplateNotFoundException(path);
			return new TemplateFile(repository.Locations.First(l => IsTemplateAtPath(l, path)));
		}
	}
}