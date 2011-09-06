using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.Responses;

namespace MailPimp
{
	public class Configure : DefaultNancyBootstrapper
	{
		static readonly IDictionary<string, string> StaticFileExtensions = new Dictionary<string, string> {
			{ "jpg", "image/jpeg" },
			{ "png", "image/png" },
			{ "gif", "image/gif" },
			{ "css", "text/css" },
			{ "js",  "text/javascript" }
		};

		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal(container);
			ConfigureStaticFiles(container);
		}

		void ConfigureStaticFiles(TinyIoC.TinyIoCContainer container)
		{
            BeforeRequest += ctx =>
            {
                var rootPathProvider =
                    container.Resolve<IRootPathProvider>();

                var requestedExtension =
                    Path.GetExtension(ctx.Request.Path);

                if (!string.IsNullOrEmpty(requestedExtension))
                {
                    var extensionWithoutDot =
                        requestedExtension.Substring(1);

                    if (StaticFileExtensions.Keys.Any(x =>
						x.Equals(extensionWithoutDot, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var fileName = Path.GetFileName(ctx.Request.Path);
                        if (fileName == null)
                            return null;

                        var filePath = Path.Combine(rootPathProvider.GetRootPath(), "content", fileName);
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