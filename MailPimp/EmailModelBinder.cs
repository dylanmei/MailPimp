using System;
using System.Collections.Generic;
using System.Dynamic;
using KeyValueDictionary = System.Collections.Generic.IDictionary<string, object>;

namespace MailPimp
{
	public class EmailModelBinder : DynamicModelBinder
	{
		static readonly IDynamicModelTransformer Transformer = new EmailModelTransformer();

		protected override IDynamicModelTransformer GetModelTransformer()
		{
			return Transformer;
		}

		public override bool CanBind(Type modelType)
		{
			return typeof(EmailModel) == modelType || base.CanBind(modelType);
		}
	}

	public class EmailModelTransformer : IDynamicModelTransformer
	{
		public object Transform(dynamic source)
		{
			var model = new EmailModel();
			if (!HasMember(source, "Model")) model.Model = model;
			else
			{
				BindModel(source, model);
				BindHeaders(source, model);
			}

			return model;
		}

		static void BindModel(dynamic source, EmailModel model)
		{
			model.Model = source.Model;
		}

		static void BindHeaders(dynamic source, EmailModel model)
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
				return ((KeyValueDictionary) @object).ContainsKey(name);
			}
			return false;
		}

		static bool HasEnumeration(dynamic @object, string name)
		{
			if (HasMember(@object, name))
			{
				return ((KeyValueDictionary) @object)[name] is IEnumerable<object>;
			}
			return false;
		}
	}
}