using System;

namespace MailPimp.Templates
{
	public abstract class Template
	{
		public virtual string Name { get; protected set; }
		public virtual string Path { get; set; }
		public virtual string Folder { get; set; }
		public virtual DateTime LastModified { get; set; }
		public abstract string Source { get; }
	}
}