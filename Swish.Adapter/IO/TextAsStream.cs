using System;
using System.IO;

namespace Swish.IO
{
	public class TextAsStream: Stream
	{
		private string _text;
		private int _position;

		public TextAsStream(string text)
		{
			_text = text;
		}

		~TextAsStream()
		{
			Close();
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get
			{
				if (_text == null)
				{
					throw new Exception("Stream closed");
				}
				return _text.Length;
			}
		}

		public override long Position
		{
			get
			{
				if (_text == null)
				{
					throw new Exception("Stream closed");
				}
				return _position;
			}
			set
			{
				if (_text == null)
				{
					throw new Exception("Stream closed");
				}
				if (value < 0 || value > Length)
				{
					throw new Exception("Position invalid");
				}
				_position = (int)value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (_text == null)
			{
				throw new Exception("Stream closed");
			}
			long position;
			switch (origin)
			{
			case SeekOrigin.Begin:
				position = (int)offset;
				break;

			case SeekOrigin.Current:
				position = Position + offset;
				break;

			case SeekOrigin.End:
				position = Length + offset;
				break;

			default:
				throw new Exception("Origin invalid");
			}

			Position = position;
			return position;
		}

		public override void SetLength(long value)
		{
			if (_text == null)
			{
				throw new Exception("Stream closed");
			}
			if (value < 0)
			{
				throw new ArgumentException("value < 0");
			}
			if (value < Length)
			{
				Position = value;
			}
			if (value < Length)
			{
				_text = _text.Substring(0, (int)value * 2);
			} else
			{
				_text = _text.PadRight((int)value * 2, ' ');
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (offset < 0 || count < 0 || offset + count > buffer.Length)
			{
				throw new Exception(" Argument invalid");
			}
			if (_text == null)
			{
				throw new Exception("Stream closed");
			}

			throw new Exception(" Not supported at the moment");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_text == null)
			{
				throw new Exception("Stream closed");
			}
			if (offset < 0 || count < 0 || offset + count > buffer.Length)
			{
				throw new Exception(" Argument invalid");
			}

			int startCount = count;
			while (count > 0)
			{
				if (_position >= _text.Length)
				{
					break;
				}
				char character = _text[_position++];

				if (character < 0xff)
				{
					buffer[offset] = (byte)character;
				} else
				{
					char[] thing = new char[1] { character };
					byte[] array = System.Text.ASCIIEncoding.ASCII.GetBytes(thing);
					buffer[offset] = array[0];
				}
				offset++;
				count--;
			}
			return startCount - count;
		}

		public override void Close()
		{
			if (_text == null)
			{
				return;
			}
			_position = 0;
			_text = null;
		}
	}
}
