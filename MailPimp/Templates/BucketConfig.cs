using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace MailPimp.Templates
{
	static class BucketConfig
	{
		static class Keys
		{
			public const string S3_BUCKET_NAME = "S3_BUCKET_NAME";
		}

		public static string GetValue()
		{
			string value;
			if ((value = ConfigurationManager.AppSettings[Keys.S3_BUCKET_NAME]) == "")
			{
				if ((value = GetHiddenValue()) == null)
					throw new ConfigurationErrorsException("Missing S3 bucket name.");
			}
			return string.Format("https://s3.amazonaws.com/{0}/", value);
		}

		static string GetHiddenValue()
		{
			var path = GetHiddenConfigPath();
			if (!string.IsNullOrEmpty(path))
			{
				using (var stream = new FileStream(path, FileMode.Open))
				{
					var config = XElement.Load(stream);
					var name = config.Element("name");
					if (name != null) return name.Value;
				}
			}
			return null;
		}

		static string GetHiddenConfigPath()
		{
			var directory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
			if (directory != null)
			{
				var location = Path.Combine(directory, @"Bucket.config");
				if (File.Exists(location))
					return location;
			}

			return null;
		}
	}
}