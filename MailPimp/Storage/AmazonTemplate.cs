using System;
using MailPimp.Templates;
using PathHelper = System.IO.Path;

namespace MailPimp.Storage.Amazon
{
	public sealed class AmazonTemplate : Template
	{
		readonly Uri BaseUri;
		readonly Func<Uri, string> client;
		string source;

		internal AmazonTemplate(Uri baseUri, Func<Uri, string> client)
		{
			BaseUri = baseUri;
			this.client = client;
		}

		public override string Source
		{
			get
			{
				return source ?? (source = client(Uri));
			}
		}

		public Uri Uri
		{
			get
			{
				return new Uri(BaseUri, Path);
			}
		}

		public override string Name
		{
			get
			{
				return PathHelper.GetFileNameWithoutExtension(Path);
			}
		}

		public override string Folder
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
