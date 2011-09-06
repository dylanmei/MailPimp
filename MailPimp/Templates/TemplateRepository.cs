using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Spark.FileSystem;

namespace MailPimp.Templates
{
	public interface ITemplateRepository : IViewFolder
	{
		IEnumerable<TemplateLocation> Locations { get; }
	}

	public class TemplateRepository : ITemplateRepository
	{
		readonly static Uri BucketUri = new Uri(BucketConfig.GetValue());
		readonly static Uri TemplatesUri = new Uri(BucketUri, "templates/");

		public IEnumerable<TemplateLocation> Locations
		{
			get { return GetTemplateLocations(BucketUri, new[] { "spark" }); }
		}

		static IEnumerable<TemplateLocation> GetTemplateLocations(Uri baseUri, IEnumerable<string> supportedExtensions)
		{
			var client = new WebClient();

			using (var stream = client.OpenRead(baseUri))
			{
				if (stream != null)
				{
					var bucket = XElement.Load(stream);
					var elements =
						from content in bucket.Elements2("Contents")
						where content.Element2("Size").Value != "0"
						select NewTemplateLocationFromBucketElement(content);

					return elements.Where(c =>
						supportedExtensions.Contains(c.Extension));
				}
			}

			return new TemplateLocation[0];
		}

		static TemplateLocation NewTemplateLocationFromBucketElement(XElement element)
		{
			return new TemplateLocation(TemplatesUri) {
				Path = GetLocalTemplatePathFromBucketKey(element.Element2("Key").Value),
				LastModified = DateTime.Parse(element.Element2("LastModified").Value)
			};
		}

		static string GetLocalTemplatePathFromBucketKey(string key)
		{
			return key.Replace("templates/", "");
		}

		public IList<string> ListViews(string path)
		{
			return Locations
				.Where(l => IsTemplateInDirectory(l, path))
				.Select(l => l.Name).ToList();
		}

		public bool HasView(string path)
		{
			return Locations.Any(l => IsTemplateAtPath(l, path));
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
			return new Template(Locations.First(l => IsTemplateAtPath(l, path)));
		}
	}

	// Ignore namespace on XElements <ListBucketResult xmlns="..."></ListBucketResult>
	static class What_I_mean_not_what_I_say_extensions
	{
		public static IEnumerable<XElement> Elements2(this XElement source, string localName)
		{
			return source.Elements().Where(e => e.Name.LocalName == localName);
		}

		public static IEnumerable<XElement> Elements2<T>(this IEnumerable<T> source, string localName)
			where T : XContainer
		{
			return source.Elements().Where(e => e.Name.LocalName == localName);
		}

		public static XElement Element2(this XElement source, string localName)
		{
			return source.Elements().FirstOrDefault(e => e.Name.LocalName == localName);
		}

		public static XElement Element2<T>(this IEnumerable<T> source, string localName)
			where T : XContainer
		{
			return source.Elements().FirstOrDefault(e => e.Name.LocalName == localName);
		}
	}
}