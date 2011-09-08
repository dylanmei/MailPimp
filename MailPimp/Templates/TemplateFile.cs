using System.IO;
using System.Net;
using System.Text;
using Spark.FileSystem;

namespace MailPimp.Templates
{
	public class TemplateFile : IViewFile
	{
		readonly TemplateLocation location;

		public TemplateFile(TemplateLocation location)
		{
			this.location = location;
		}

		public Stream OpenViewStream()
		{
			var client = new WebClient();
			var contents = "";
			using (var stream = client.OpenRead(location.Uri))
			{
				if (stream != null)
				{
					using (var reader = new StreamReader(stream, Encoding.UTF8))
						contents = reader.ReadToEnd();
				}
			}

			return new MemoryStream(Encoding.UTF8.GetBytes(contents));
		}

		public long LastModified
		{
			get { return location.LastModified.Ticks; }
		}
	}
}