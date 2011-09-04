using System;
using System.Dynamic;
using System.IO;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;

namespace MailPimp
{
	public class DynamicModelBinder : IModelBinder
	{
		public object Bind(NancyContext context, Type modelType, params string[] blackList)
		{
			var request = context.Request;
			var content = GetContent(request.Body);
			var transformer = GetModelTransformer();
			var deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(content);
			return transformer.Transform(deserializedObject);
		}

		static string GetContent(Stream bodyStream)
		{
			string bodyText;
			bodyStream.Position = 0;
			using (var bodyReader = new StreamReader(bodyStream))
				bodyText = bodyReader.ReadToEnd();
			return bodyText;
		}

		protected virtual IDynamicModelTransformer GetModelTransformer()
		{
			return DynamicModelTransformer.Default;
		}

		public virtual bool CanBind(Type modelType)
		{
			return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(modelType);
		}
	}
}