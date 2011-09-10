using System;
using System.IO;
using System.Net;
using System.Text;

namespace MailPimp.Templates
{
	public class BucketClient : IStorageClient
	{
		public string Read(Uri address)
		{
			return Read(address, Encoding.UTF8);
		}

		public string Read(Uri address, Encoding encoding)
		{
			return ReadResource(address, encoding);
		}

		protected virtual string ReadResource(Uri address, Encoding encoding)
		{
			var contents = "";
			var client = new WebClient();
			using (var stream = client.OpenRead(address))
			{
				if (stream != null)
				{
					using (var reader = new StreamReader(stream, encoding))
						contents = reader.ReadToEnd();
				}
			}

			return contents;
		}
	}
}