using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy.ViewEngines;
using Spark;
using Spark.Parser;
using Spark.Parser.Syntax;

namespace MailPimp.ViewEngine
{
	public interface IDescriptorBuilder
    {
        /// <summary>
        /// Implemented by custom descriptor builder to quickly extract additional parameters needed
        /// to locate templates, like the theme or language in effect for the request
        /// </summary>
        /// <param name="viewLocationResult">Context information for the current request</param>
        /// <returns>An in-order array of values which are meaningful to BuildDescriptor on the same implementation class</returns>
        IDictionary<string, object> GetExtraParameters(ViewLocationResult viewLocationResult);

        /// <summary>
        /// Given a set of MVC-specific parameters, a descriptor for the target view is created. This can
        /// be a bit more expensive because the existence of files is tested at various candidate locations.
        /// </summary>
        /// <param name="buildDescriptorParams">Contains all of the standard and extra parameters which contribute to a descriptor</param>
        /// <param name="searchedLocations">Candidate locations are added to this collection so an information-rich error may be returned</param>
        /// <returns>The descriptor with all of the detected view locations in order</returns>
        SparkViewDescriptor BuildDescriptor(DescriptorParameters buildDescriptorParams, ICollection<string> searchedLocations);
    }

	public interface IDescriptorFilter
    {
        /// <summary>
        /// Called frequently to extract filter-specific parameters from a request viewLocationResult. This call
        /// happens on every request so should be implemented as efficiently as possible.
        /// </summary>
        /// <param name="viewLocationResult">The current request's action viewLocationResult</param>
        /// <param name="extra">Dictionary where additional parameters should be added</param>
        void ExtraParameters(ViewLocationResult viewLocationResult, IDictionary<string, object> extra);

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

    public class DescriptorBuilder : IDescriptorBuilder
    {
        private ISparkViewEngine engine;
        private MasterGrammar grammar;

        public DescriptorBuilder()
            : this((string)null)
        {
        }

        public DescriptorBuilder(ISparkViewEngine engine)
            : this((string)null)
        {
            this.engine = engine;
            grammar = new MasterGrammar(this.engine.Settings.Prefix);
        }

        public DescriptorBuilder(string prefix)
        {
            Filters = new List<IDescriptorFilter>();
            grammar = new MasterGrammar(prefix);
        }

        public IList<IDescriptorFilter> Filters { get; set; }

        public ParseAction<string> ParseUseMaster
        {
            get { return grammar.ParseUseMaster; }
        }

        public virtual void Initialize(ISparkServiceContainer container)
        {
            engine = container.GetService<ISparkViewEngine>();
            grammar = new MasterGrammar(engine.Settings.Prefix);
        }

        public virtual IDictionary<string, object> GetExtraParameters(ViewLocationResult viewLocationResult)
        {
            var extra = new Dictionary<string, object>();
            foreach (var filter in Filters)
            {
                filter.ExtraParameters(viewLocationResult, extra);
            }

            return extra;
        }

        public virtual SparkViewDescriptor BuildDescriptor(
			DescriptorParameters descriptorParameters, ICollection<string> searchedLocations)
        {
            var descriptor = new SparkViewDescriptor {
                TargetNamespace = descriptorParameters.ViewPath
            };

        	IEnumerable<string> potentialLocations = PotentialViewLocations(
				descriptorParameters.ViewPath, descriptorParameters.ViewName, descriptorParameters.Extra);
            if (!LocatePotentialTemplate(potentialLocations, descriptor.Templates, searchedLocations))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(descriptorParameters.MasterName))
            {
				potentialLocations = PotentialMasterLocations(
					descriptorParameters.MasterName, descriptorParameters.Extra);
                if (!LocatePotentialTemplate(potentialLocations, descriptor.Templates, searchedLocations))
                {
                    return null;
                }
            }
            else if (descriptorParameters.FindDefaultMaster && TrailingUseMasterName(descriptor) == null /*empty is a valid value*/)
            {
				potentialLocations = PotentialDefaultMasterLocations(
					descriptorParameters.ViewPath, descriptorParameters.Extra);
                LocatePotentialTemplate(potentialLocations, descriptor.Templates, null);
            }

            var trailingUseMaster = TrailingUseMasterName(descriptor);
            while (descriptorParameters.FindDefaultMaster && !string.IsNullOrEmpty(trailingUseMaster))
            {
                if (!LocatePotentialTemplate(PotentialMasterLocations(trailingUseMaster, descriptorParameters.Extra),
                    descriptor.Templates, searchedLocations))
                {
                    return null;
                }
                trailingUseMaster = TrailingUseMasterName(descriptor);
            }

            return descriptor;
        }

        public string TrailingUseMasterName(SparkViewDescriptor descriptor)
        {
            var lastTemplate = descriptor.Templates.Last();
            var sourceContext = AbstractSyntaxProvider.CreateSourceContext(lastTemplate, engine.ViewFolder);

            if (sourceContext == null)
            {
                return null;
            }

            var result = ParseUseMaster(new Position(sourceContext));

            return result == null ? null : result.Value;
        }

        private bool LocatePotentialTemplate(
            IEnumerable<string> potentialTemplates,
            ICollection<string> descriptorTemplates,
            ICollection<string> searchedLocations)
        {
            var template = potentialTemplates.FirstOrDefault(t => engine.ViewFolder.HasView(t));
            if (template != null)
            {
                descriptorTemplates.Add(template);
                return true;
            }

            if (searchedLocations != null)
            {
                foreach (var potentialTemplate in potentialTemplates)
                {
                    searchedLocations.Add(potentialTemplate);
                }
            }

            return false;
        }

        /// <remarks>Apply all of the filters PotentialLocations in order</remarks>
        private IEnumerable<string> ApplyFilters(IEnumerable<string> locations, IDictionary<string, object> extra)
        {
            return Filters.Aggregate(locations, (aggregate, filter) => filter.PotentialLocations(aggregate, extra));
        }

        protected virtual IEnumerable<string> PotentialViewLocations(string viewPath, string viewName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[] {
                Path.Combine(viewPath, viewName + ".spark"),
                Path.Combine("Shared", viewName + ".spark")
            }, extra);
        }

        protected virtual IEnumerable<string> PotentialMasterLocations(string masterName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[] {
				Path.Combine("Layouts", masterName + ".spark"),
				Path.Combine("Shared", masterName + ".spark")
			}, extra);
        }

        protected virtual IEnumerable<string> PotentialDefaultMasterLocations(string controllerName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[] {
                Path.Combine("Layouts", "Application.spark"),
                Path.Combine("Shared", "Application.spark")
            }, extra);
        }
    }
}