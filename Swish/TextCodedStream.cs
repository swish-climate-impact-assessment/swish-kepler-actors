using System;
using System.IO;

namespace Swish
{
	public class TextCodedStream: Stream, IDisposable
	{
		private bool _encode;
		private int _stage;
		private int _buffer;
		private Stream _stream;
		private bool _leaveStreamOpen;

		public TextCodedStream(Stream stream, bool encode, bool leaveStreamOpen)
		{
			if (encode)
			{
				if (!stream.CanWrite)
				{
					throw new Exception("Encoding requires writable stream");
				}
			} else if (!stream.CanRead)
			{
				throw new Exception("Decoding requires readable stream");
			}

			_encode = encode;
			_stream = stream;
			_leaveStreamOpen = leaveStreamOpen;
		}

		~TextCodedStream()
		{
			Dispose(false);
		}

		public override void Close()
		{
			Dispose(true);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		private new void Dispose(bool disposing)
		{
			if (!disposing)
			{
				if (_stream != null)
				{
					throw new Exception("Stream not closed ");
				}
				return;
			}
			GC.SuppressFinalize(this);
			if (_stream == null)
			{
				return;
			}

			if (_encode)
			{
				if (_stage == 0)
				{
					_stream.WriteByte((byte)33);
				} else
				{
					_stream.WriteByte((byte)46);
					int item = _buffer;
					item = Encode(item);
					_stream.WriteByte((byte)item);
				}
				_stream.Flush();
			}

			if (!_leaveStreamOpen)
			{
				_stream.Close();
			}
			_stream = null;
		}

		public Stream Stream
		{
			get { return _stream; }
		}
		public bool LeaveStreamOpen
		{
			get { return _leaveStreamOpen; }
			set { _leaveStreamOpen = value; }
		}

		public override bool CanRead
		{
			get { return !_encode; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return _encode; }
		}

		public override void Flush()
		{
			if (_stream != null)
			{
				_stream.Flush();
			}
		}

		public override long Length
		{
			get { return (_stream.Length - 1) * 6 / 8; }
		}

		public override long Position
		{
			get { throw new InvalidOperationException(""); }
			set { throw new InvalidOperationException(""); }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Cannot Seek of " + typeof(TextCodedStream).FullName);
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("Cannot SetLength of " + typeof(TextCodedStream).FullName);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (offset < 0 || count < 0 || offset + count > buffer.Length)
			{
				throw new Exception(" Argument invalid");
			}
			if (_stream == null)
			{
				throw new Exception(" Stream closed");
			}
			if (!_encode)
			{
				throw new Exception("Cannot write when stream not open for encodeing");
			}

			int value;
			while (count > 0)
			{
				if (_stage < 3)
				{
					value = buffer[offset];
					offset++;
					count--;
				} else
				{
					value = 0;
				}

				int item = _buffer | (value >> (_stage * 2 + 2));
				item = Encode(item);
				_stream.WriteByte((byte)item);
				_buffer = ((value << (6 - _stage * 2)) & 0xff) >> 2;

				_stage = (_stage + 1) % 4;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (offset < 0 || count < 0 || offset + count > buffer.Length)
			{
				throw new Exception(" Argument invalid");
			}
			if (_stream == null)
			{
				throw new Exception(" Stream closed");
			}
			if (_encode)
			{
				throw new Exception("Cannot read when stream open for encodeing");
			}

			if (_stage >= 4)
			{
				return 0;
			}

			int item;
			int value;
			int startCount = count;

			while (count > 0)
			{
				value = _stream.ReadByte();
				if (value < 0)
				{
					break;
				}

				if (_stage == 0)
				{
					if (value == 33)
					{
						_stage = 4;
						break;
					}
					value = Decode(value);
					_buffer = value << 2;
					_stage = 1;
					continue;
				}

				bool end;
				if (value == 46)
				{
					value = _stream.ReadByte();
					if (value < 0)
					{
						break;
					}
					end = true;
				} else
				{
					end = false;
				}

				value = Decode(value);
				item = _buffer | (value >> (6 - _stage * 2));
				_buffer = value << (2 + _stage * 2);

				buffer[offset] = (byte)item;
				offset++;
				count--;

				if (end)
				{
					_stage = 5;
					break;
				}
				_stage = (_stage + 1) % 4;
			}

			return startCount - count;
		}

		private static int Encode(int value)
		{
			int encodedValue;
			if (value < 12)
			{
				if (value < 2)
				{
					if (value == 0)
					{
						encodedValue = 45;
					} else
					{
						encodedValue = 44;
					}
				} else
				{
					encodedValue = value + 46;
				}
			} else
			{
				if (value < 38)
				{
					encodedValue = value + 53;
				} else
				{
					encodedValue = value + 59;
				}
			}
			return encodedValue;
		}

		private static int Decode(int value)
		{
			if (value < 65)
			{
				if (value < 48)
				{
					if (value == 45)
					{
						return 0;
					}else //if (value == 44)
					{
						return 1;
					}
				}
				return value - 46;
			}
			if (value < 97)
			{
				return value - 53;
			}
			return value - 59;
		}
	}
}
