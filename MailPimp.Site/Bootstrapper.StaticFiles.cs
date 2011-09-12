using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.Responses;

namespace MailPimp
{
	public partial class Bootstrapper
	{
		static readonly IDictionary<string, string> StaticFileExtensions = new Dictionary<string, string> {
			{ "jpg", "image/jpeg" },
			{ "png", "image/png" },
			{ "gif", "image/gif" },
			{ "css", "text/css" },
			{ "js",  "text/javascript" }
		};

		void ConfigureStaticFiles()
		{
            BeforeRequest += ctx =>
            {
				var pathProvider = Resolve<IRootPathProvider>();
				var extension = Path.GetExtension(ctx.Request.Path);

				if (!string.IsNullOrEmpty(extension))
				{
				    var extensionWithoutDot =
				        extension.Substring(1);

				    if (StaticFileExtensions.Keys.Any(x =>
				        x.Equals(extensionWithoutDot, StringComparison.InvariantCultureIgnoreCase)))
				    {
				        var fileName = Path.GetFileName(ctx.Request.Path);
				        if (fileName == null)
				            return null;

				        var filePath = Path.Combine(pathProvider.GetRootPath(), "content", fileName);
				        return !File.Exists(filePath)
				            ? null
				            : new GenericFileResponse(filePath, StaticFileExtensions[extensionWithoutDot]);
				    }
				}

                return null;
            };
		}
	}
}