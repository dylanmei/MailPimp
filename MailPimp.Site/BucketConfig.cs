using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace MailPimp
{
	static class BucketConfig
	{
		static class Keys
		{
			public const string S3_BUCKET_NAME = "S3_BUCKET_NAME";
		}

		public static string GetName()
		{
			string value;
			if ((value = ConfigurationManager.AppSettings[Keys.S3_BUCKET_NAME]) == "")
			{
				if ((value = GetHiddenValue("name")) == null)
					throw new ConfigurationErrorsException("Missing S3 bucket name.");
			}
			return value;
		}

		static string GetHiddenValue(string key)
		{
			var path = GetHiddenConfigPath();
			if (!string.IsNullOrEmpty(path))
			{
				using (var stream = new FileStream(path, FileMode.Open))
				{
					var config = XElement.Load(stream);
					var name = config.Element(key);
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