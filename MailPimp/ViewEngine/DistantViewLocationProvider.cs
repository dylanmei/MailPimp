using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Nancy.ViewEngines;

namespace MailPimp.ViewEngine
{
	public class DistantViewLocationProvider : IViewLocationProvider
	{
		readonly Uri BaseUri = new Uri("https://s3.amazonaws.com/mailpimp/");

		class BucketElements
		{
			public Uri Uri { get; set; }
			public DateTime LastModified { get; set; }

			public string FileName
			{
				get
				{
					return Path.GetFileNameWithoutExtension(Uri.LocalPath);
				}
			}

			public string FileExtension
			{
				get
				{
					var extension = Path.GetExtension(Uri.LocalPath);
					if (extension.StartsWith("."))
						extension = extension.Substring(1);
					return extension;
				}
			}
		}

		public IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions)
		{
			return
				from element in GetBucketElements(BaseUri, supportedViewExtensions)
				select new ViewLocationResult(element.Uri.ToString(), element.FileName, element.FileExtension, () => {
					return GetBucketElement(element.Uri);
				});
		}



		static IEnumerable<BucketElements> GetBucketElements(Uri baseUri, IEnumerable<string> supportedViewExtensions)
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
					    select new BucketElements {
							Uri = new Uri(baseUri, content.Element2("Key").Value),
							LastModified = DateTime.Parse(content.Element2("LastModified").Value)
						};

					return elements.Where(c =>
						supportedViewExtensions.Contains(c.FileExtension));
				}
			}

			return new BucketElements[0];
		}

		static TextReader GetBucketElement(Uri elementUri)
		{
			var client = new WebClient();
			var contents = "";
			using (var stream = client.OpenRead(elementUri))
			{
				if (stream != null)
				{
					using (var reader = new StreamReader(stream, Encoding.UTF8))
						contents = reader.ReadToEnd();
				}
			}

			return new StringReader(contents);
		}
	}

	static class NamespacesSuck
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