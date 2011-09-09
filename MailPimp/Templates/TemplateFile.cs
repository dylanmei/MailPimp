using System.IO;
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
			var client = new StorageClient();
			var contents = client.Read(location.Uri, Encoding.UTF8);
			return new MemoryStream(Encoding.UTF8.GetBytes(contents));
		}

		public long LastModified
		{
			get { return location.LastModified.Ticks; }
		}
	}
}