using System;
using MailPimp.Templates;

namespace MailPimp
{
	public class TemplateModelBinder : DynamicModelBinder
	{
		static readonly IDynamicModelTransformer Transformer = new TemplateModelTransformer();

		protected override IDynamicModelTransformer GetModelTransformer()
		{
			return Transformer;
		}

		public override bool CanBind(Type modelType)
		{
			return typeof(TemplateModel) == modelType || base.CanBind(modelType);
		}
	}
}