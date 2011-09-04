using System.Collections.Generic;

namespace MailPimp.ViewEngine
{
    public class ViewResult
    {
        public View View { get; set; }

		public ViewResult(View view)
        {
            View = view;
        }

        public ViewResult(List<string> searchedLocations)
        {
            var locations = string.Empty;
            searchedLocations.ForEach(location =>
				locations += string.Format("{0} ", location));

            if (!string.IsNullOrEmpty(locations))
            {
                throw new Spark.Compiler.CompilerException(
					string.Format("The view could not be in any of the following locations: {0}", locations));
            }
        }
    }
}