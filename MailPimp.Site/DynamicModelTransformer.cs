namespace MailPimp
{
	public interface IDynamicModelTransformer
	{
		object Transform(dynamic model);
	}

	class DynamicModelTransformer : IDynamicModelTransformer
	{
		public static readonly DynamicModelTransformer Default = new DynamicModelTransformer();
		public object Transform(dynamic model)
		{
			return model;
		}
	}
}