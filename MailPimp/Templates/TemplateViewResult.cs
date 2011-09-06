using System.Collections.Generic;

namespace MailPimp.Templates
{
    class TemplateViewResult
    {
        public TemplateView View { get; set; }

		public TemplateViewResult(TemplateView view)
        {
            View = view;
        }

        public TemplateViewResult(List<string> searchedLocations)
        {
            var locations = string.Empty;
            searchedLocations.ForEach(location =>
				locations += string.Format("{0} ", location));

            if (!string.IsNullOrEmpty(locations))
            {
                throw new Spark.Compiler.CompilerException(
					string.Format("The view was not in any of the following locations: {0}", locations));
            }
        }
    }
}