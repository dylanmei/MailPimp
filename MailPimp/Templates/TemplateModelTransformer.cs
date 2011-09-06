using System.Collections.Generic;
using System.Dynamic;

namespace MailPimp.Templates
{
	public class TemplateModelTransformer : IDynamicModelTransformer
	{
		public object Transform(dynamic source)
		{
			var model = new TemplateModel();
			if (!HasMember(source, "Model")) model.Model = model;
			else
			{
				BindModel(source, model);
				BindHeaders(source, model);
			}

			return model;
		}

		static void BindModel(dynamic source, TemplateModel model)
		{
			model.Model = source.Model;
		}

		static void BindHeaders(dynamic source, TemplateModel model)
		{
			if (HasMember(source, "Subject"))
				model.Subject = source.Subject;

			if (HasMember(source, "From"))
			{
				var from = source.From;
				if (HasMember(from, "Name")) model.From.Name = from.Name;
				if (HasMember(from, "Email")) model.From.Email = from.Email;
			}

			if (HasEnumeration(source, "To"))
			{
				foreach (var address in source.To)
				{
					var name = ""; var email = "";
					if (HasMember(address, "Name")) name = address.Name;
					if (HasMember(address, "Email")) email = address.Email;
					model.To.Add(new Address {
						Name = name,
						Email = email
					});
				}
			}
		}

		static bool HasMember(dynamic @object, string name)
		{
			if (@object is ExpandoObject)
			{
				return ((IDictionary<string, object>) @object).ContainsKey(name);
			}
			return false;
		}

		static bool HasEnumeration(dynamic @object, string name)
		{
			if (HasMember(@object, name))
			{
				return ((IDictionary<string, object>) @object)[name] is IEnumerable<object>;
			}
			return false;
		}
	}
}