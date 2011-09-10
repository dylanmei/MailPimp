using System;
using System.Text;

namespace MailPimp.Templates
{
	public interface IStorageClient
	{
		string Read(Uri address);
		string Read(Uri address, Encoding encoding);
	}
}