using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using MailPimp.Templates;

namespace MailPimp.Storage.Amazon
{
	public class AmazonRepository : ITemplateRepository
	{
		readonly Uri bucket;
		IEnumerable<Template> locations;

		public AmazonRepository(string bucketName)
		{
			bucket = new Uri(string.Format("https://s3.amazonaws.com/{0}/", bucketName));
		}

		public IEnumerable<Template> Templates
		{
			get
			{
				return locations ?? (locations = GetTemplateLocations(bucket, new[] {"spark"}));
			}
		}

		IEnumerable<Template> GetTemplateLocations(Uri baseUri, IEnumerable<string> supportedExtensions)
		{
			var index = ReadResource(baseUri);
			var document = XElement.Parse(index);
			var elements =
			    from content in document.Elements2("Contents")
			    where content.Element2("Size").Value != "0"
			    select NewTemplateLocationFromBucketElement(content);

			return elements.Where(c =>
			    supportedExtensions.Contains(c.Extension));
		}

		static string ReadResource(Uri address)
		{
			var contents = "";
			var client = new WebClient();
			using (var stream = client.OpenRead(address))
			{
				if (stream != null)
				{
					using (var reader = new StreamReader(stream, Encoding.UTF8))
						contents = reader.ReadToEnd();
				}
			}

			return contents;
		}

		AmazonTemplate NewTemplateLocationFromBucketElement(XElement element)
		{
			var rootPath = new Uri(bucket, "templates/");
			return new AmazonTemplate(rootPath, ReadResource) {
			    Path = GetLocalTemplatePathFromBucketKey(element.Element2("Key").Value),
			    LastModified = DateTime.Parse(element.Element2("LastModified").Value)
			};
		}

		static string GetLocalTemplatePathFromBucketKey(string key)
		{
			return key.Replace("templates/", "");
		}

		public bool IsTemplateInFolder(Template template, string path)
		{
			if (path.Contains(@"\")) path = path.Replace(@"\", "/");
			return string.Compare(path, template.Folder, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public bool IsTemplateInPath(Template template, string path)
		{
			if (path.Contains(@"\")) path = path.Replace(@"\", "/");
			return string.Compare(path, template.Path, StringComparison.InvariantCultureIgnoreCase) == 0;
		}
	}
}
