using System.Collections.Generic;
using System.Linq;
using KeyValueDictionary = System.Collections.Generic.IDictionary<string, object>;

namespace MailPimp.ViewEngine
{
    public class DescriptorParameters
    {
        readonly bool findDefaultMaster;
        readonly int hashCode;
        readonly string masterName;
        readonly string viewName;
        readonly string viewPath;
        readonly KeyValueDictionary extra;

        public DescriptorParameters(string viewPath, string viewName, string masterName, bool findDefaultMaster, KeyValueDictionary extra)
        {
            this.viewPath = viewPath;
            this.viewName = viewName;
            this.masterName = masterName;
            this.findDefaultMaster = findDefaultMaster;
            this.extra = extra ?? new Dictionary<string, object>();

            hashCode = CalculateHashCode();
        }

        public string ViewPath
        {
            get { return viewPath; }
        }

        public string ViewName
        {
            get { return viewName; }
        }

        public string MasterName
        {
            get { return masterName; }
        }

        public bool FindDefaultMaster
        {
            get { return findDefaultMaster; }
        }

        public IDictionary<string, object> Extra
        {
            get { return extra; }
        }

        private static int Hash(object str)
        {
            return str == null ? 0 : str.GetHashCode();
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        private int CalculateHashCode()
        {
            return Hash(viewName) ^ Hash(viewPath) ^ Hash(masterName) ^ findDefaultMaster.GetHashCode() ^
                   extra.Aggregate(0, (hash, kv) => hash ^ Hash(kv.Key) ^ Hash(kv.Value));
        }

        public override bool Equals(object obj)
        {
            var other = obj as DescriptorParameters;
            if (other == null || other.GetType() != GetType())
                return false;

            if (!string.Equals(viewName, other.viewName) ||
                !string.Equals(viewPath, other.viewPath) ||
                !string.Equals(masterName, other.masterName) ||
                findDefaultMaster != other.findDefaultMaster)
            {
                return false;
            }
            return DictionariesEqual(extra, other.extra);
        }

		static bool DictionariesEqual(KeyValueDictionary a, KeyValueDictionary b)
		{
			if (a.Count != b.Count) return false;
            foreach (var pair in a)
            {
                object value;
                if (!b.TryGetValue(pair.Key, out value) || !Equals(pair.Value, value))
                {
                    return false;
                }
            }
			return true;
		}
    }
}