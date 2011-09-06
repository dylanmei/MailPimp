using System;
using PathHelper = System.IO.Path;

namespace MailPimp.Templates
{
	public class TemplateLocation
	{
		readonly Uri BaseUri;

		public TemplateLocation(Uri baseUri)
		{
			BaseUri = baseUri;
		}

		public string Path { get; set; }
		public DateTime LastModified { get; set; }

		public Uri Uri
		{
			get
			{
				return new Uri(BaseUri, Path);
			}
		}

		public string Name
		{
			get
			{
				return PathHelper.GetFileNameWithoutExtension(Path);
			}
		}

		public string Directory
		{
			get
			{
				return Path.Contains("/") 
					? Path.Replace("/" + PathHelper.GetFileName(Path), "")
					: "";
			}
		}

		public string Extension
		{
			get
			{
				var extension = PathHelper.GetExtension(Path);
				if (extension != null && extension.StartsWith("."))
					extension = extension.Substring(1);
				return extension;
			}
		}
	}
}