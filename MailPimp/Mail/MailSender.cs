using System.Net.Mail;
using System.Text;

namespace MailPimp.Mail
{
	public interface IMailSender
	{
		void Send(Mailbag mailbag);
	}

	public class MailSender : IMailSender
	{
		public void Send(Mailbag mailbag)
		{
			var it = mailbag
				.GetMail()
				.GetEnumerator();

			if (!it.MoveNext()) return;
			using (var client = new SmtpClient())
			{
				do
				{
					Send(client, it.Current);
				} while (it.MoveNext());
			}
		}

		static void Send(SmtpClient client, Mail mail)
		{
			var to = new MailAddress(mail.To.Email, mail.To.Name);
			var from = new MailAddress(mail.From.Email, mail.From.Name);

			client.Send(new MailMessage(from, to) {
				Subject = mail.Subject ?? "",
				IsBodyHtml = true,
				BodyEncoding = Encoding.UTF8,
				Body = mail.Contents
			});
		}
	}
}