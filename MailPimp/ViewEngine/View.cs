using System.IO;
using Nancy.ViewEngines;
using Spark;

namespace MailPimp.ViewEngine
{
    public abstract class View : SparkViewBase
    {
        public dynamic Model { get; protected set; }

		public IRenderContext RenderContext { get; set; }

		public virtual void Bind(dynamic model)
		{
			Model = model;
		}

        public void Execute(TextWriter writer)
        {
            RenderView(writer);
        }

        public string SiteResource(string path)
        {
            return RenderContext.ParsePath(path);
        }
    }
}