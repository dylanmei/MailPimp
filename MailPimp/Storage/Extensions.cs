using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MailPimp.Storage.Amazon
{
	// Ignore namespace on XElements <ListBucketResult xmlns="..."></ListBucketResult>
	static class XElementExtensions
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
