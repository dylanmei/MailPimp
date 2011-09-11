using System;
using System.IO;
using System.Text;

namespace MailPimp
{
	class TextStream : Stream
	{
		readonly StringBuilder builder;
		readonly Encoding encoding = new UTF8Encoding();

		public TextStream()
		{
			builder = new StringBuilder();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			builder.Append(encoding.GetString(buffer, offset, count));
		}
		
		public override string ToString()
		{
			return builder.ToString();
		}

		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get { return builder.Length;  }
		}
       
		public override long Position
		{
			get { return 0; }
			set { }
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}
}