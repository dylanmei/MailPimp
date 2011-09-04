using System;
using System.IO;
using System.Text;

namespace MailPimp
{
	class TextStream : Stream
	{
		readonly StringBuilder buffer;
		readonly Encoding encoding;

		public TextStream()
		{
			buffer = new StringBuilder();
			encoding = new UTF8Encoding();
		}

		public override bool CanRead
		{
			get { return true; }
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
			get { return 0 ;  }
		}
       
		public override long Position
		{
			get { return 0; }
			set { }
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.buffer.Append(encoding.GetString(buffer, offset, count));
		}
		
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return buffer.ToString();
		}
	}
}