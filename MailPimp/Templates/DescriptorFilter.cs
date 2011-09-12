﻿using System.Collections.Generic;

namespace MailPimp.Templates
{
	public interface IDescriptorFilter
	{
		/// <summary>
		/// Called frequently to extract filter-specific parameters from a request viewLocationResult. This call
		/// happens on every request so should be implemented as efficiently as possible.
		/// </summary>
		/// <param name="templateLocation">The current request's action templateLocation</param>
		/// <param name="extra">Dictionary where additional parameters should be added</param>
		void ExtraParameters(Template template, IDictionary<string, object> extra);

		/// <summary>
		/// The DefaultDescriptorBuider calls this method for the filter to return a modified enumerable
		/// ordered list of potential template locations. This is called only when the unique combination of action,
		/// master, view, and extra have not been resolved previously.
		/// </summary>
		/// <param name="locations">incoming ordered list of locations</param>
		/// <param name="extra">extra parameters which have been extracted</param>
		/// <returns>either the original list or a new, augmented, enumerable list</returns>
		IEnumerable<string> PotentialLocations(IEnumerable<string> locations, IDictionary<string, object> extra);
	}
}