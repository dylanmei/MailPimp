using System;
using System.IO;
using System.Net;
using System.Text;

namespace MailPimp.Templates
{
	public interface IStorageClient
	{
		string Read(Uri address);
		string Read(Uri address, Encoding encoding);
	}

	public class StorageClient : IStorageClient
	{
		public string Read(Uri address)
		{
			return Read(address, Encoding.UTF8);
		}

		public string Read(Uri address, Encoding encoding)
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