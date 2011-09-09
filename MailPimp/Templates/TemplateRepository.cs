using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MailPimp.Templates
{
	public interface ITemplateRepository
	{
		IEnumerable<TemplateLocation> Locations { get; }
	}

	public class TemplateRepository : ITemplateRepository
	{
		readonly static Uri BucketUri = new Uri(BucketConfig.GetValue());
		readonly static Uri TemplatesUri = new Uri(BucketUri, "templates/");

		readonly IStorageClient client;
		IEnumerable<TemplateLocation> locations;

		public TemplateRepository(IStorageClient client)
		{
			this.client = client;
		}

		public IEnumerable<TemplateLocation> Locations
		{
			get
			{
				return locations ?? (locations = GetTemplateLocations(BucketUri, new[] {"spark"}));
//				return GetTemplateLocations(BucketUri, new[] { "spark" });
			}
		}

		IEnumerable<TemplateLocation> GetTemplateLocations(Uri baseUri, IEnumerable<string> supportedExtensions)
		{
			var index = client.Read(baseUri);
			var bucket = XElement.Parse(index);
			var elements =
				from content in bucket.Elements2("Contents")
				where content.Element2("Size").Value != "0"
				select NewTemplateLocationFromBucketElement(content);

			return elements.Where(c =>
				supportedExtensions.Contains(c.Extension));
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