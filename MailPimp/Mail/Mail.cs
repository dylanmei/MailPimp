namespace MailPimp.Mail
{
	public class Mail
	{
		public Address From { get; set; }
		public Address To { get; set; }
		public string Subject { get; set; }
		public string Contents { get; set; }
	}
}