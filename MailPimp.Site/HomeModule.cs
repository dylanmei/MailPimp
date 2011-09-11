using Nancy;

namespace MailPimp
{
	public class HomeModule :  NancyModule
	{
		public HomeModule()
		{
			Get["/"] = parameters => {
				return View["index"];
			};
		}
	}
}