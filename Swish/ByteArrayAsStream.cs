using System;
using System.IO;

namespace Swish
{
	public class ByteArrayAsStream: Stream
	{
		private byte[] _array;
		private int _position;

		public ByteArrayAsStream(byte[] array)
		{
			_array = array;
		}

		~ByteArrayAsStream()
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
				if (_array == null)
				{
					throw new Exception("Stream closed");
				}
				return _array.Length;
			}
		}

		public override long Position
		{
			get
			{
				if (_array == null)
				{
					throw new Exception("Stream closed");
				}
				return _position;
			}
			set
			{
				if (_array == null)
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
			if (_array == null)
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
			if (_array == null)
			{
				throw new Exception("Stream closed");
			}
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (value < Length)
			{
				Position = value;
			}
			if (value > int.MaxValue)
			{
				throw new ArgumentException("value");
			}
			Array.Resize(ref _array, (int)value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (offset < 0 || count < 0 || offset + count > buffer.Length)
			{
				throw new Exception(" Argument invalid");
			}
			if (_array == null)
			{
				throw new Exception("Stream closed");
			}

			throw new Exception(" Not supported at the moment");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_array == null)
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
				if (_position >= _array.Length)
				{
					break;
				}

				byte character = _array[_position++];
				buffer[offset] = (byte)character;

				offset++;
				count--;
			}
			return startCount - count;
		}

		public override void Close()
		{
			if (_array == null)
			{
				return;
			}
			_position = 0;
			_array = null;
		}
	}
}
