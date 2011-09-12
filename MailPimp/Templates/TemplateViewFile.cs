using System.IO;
using System.Text;
using Spark.FileSystem;

namespace MailPimp.Templates
{
	public class TemplateViewFile : IViewFile
	{
		readonly Template template;

		public TemplateViewFile(Template template)
		{
			this.template = template;
		}

		public Stream OpenViewStream()
		{
			var contents = template.Source;
			return new MemoryStream(Encoding.UTF8.GetBytes(contents));
		}

		public long LastModified
		{
			get { return template.LastModified.Ticks; }
		}
	}
}