using System;
using System.IO;

namespace Swish.Server.IO
{
	class ChunkStream: Stream, IDisposable
	{
		internal static string CRLFString = "\r\n";
		// Chunked transfer coding
		// Chunked-Body = *chunk \
		// last-chunk \
		// trailer \
		// CRLF

		// chunk = chunk-size [ chunk-extension ] CRLF \
		// chunk-data CRLF
		// chunk-size = 1*HEX
		// last-chunk = 1*("0") [ chunk-extension ] CRLF

		// chunk-extension= *( ";" chunk-ext-name [ "=" chunk-ext-val ] )
		// chunk-ext-name = token
		// chunk-ext-val = token | quoted-string
		// chunk-data = chunk-size(OCTET)

		// trailer = *(entity-header CRLF)

		private ForwardParser _readStream;
		private Stream _writeStream;
		private bool _leaveOpen;
		private bool _readMode;

		public ChunkStream(Stream stream, bool readMode, bool leaveOpen)
		{
			if (readMode)
			{
				if (!stream.CanRead)
				{
					throw new Exception("cannot read from stream");
				}
				_readStream = new ForwardParser(stream, leaveOpen);
			} else
			{
				if (!stream.CanWrite)
				{
					throw new Exception("cannot write from stream");
				}
				_writeStream = stream;
			}

			_leaveOpen = leaveOpen;
			_readMode = readMode;
		}

		~ChunkStream()
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

		protected override void Dispose(bool disposing)
		{
			if (!disposing)
			{
				_readStream = null;
				_writeStream = null;
				return;
			}
			if (!_readMode)
			{
				Flush();
				AsciiIO.WriteHex(_writeStream, 0, false, -1);
				AsciiIO.Write(_writeStream, CRLFString);
				AsciiIO.Write(_writeStream, CRLFString);
			}
			using (_readStream) { }
			if (!_leaveOpen)
			{
				using (_writeStream) { }
			}
			_readStream = null;
			_writeStream = null;
			GC.SuppressFinalize(this);
		}

		public override bool CanRead
		{
			get { return _readMode; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return !_readMode; }
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("cannot determin length untill fully read");
			}
		}

		public override long Position
		{
			get { throw new NotSupportedException("cannot seek"); }
			set { throw new NotSupportedException("cannot seek"); }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("cannot seek");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("cannot change length");
		}

		private bool _reachedEnd;
		private long _chunkLength;

		private byte[] _writeBuffer = new byte[DefaultBufferSize];
		internal const int DefaultBufferSize = 8192;
		private int _bufferSize = DefaultBufferSize;
		private int _bufferPosition;

		public int BufferSize
		{
			get { return _bufferSize; }
			set
			{

				if (_bufferSize <= 0)
				{
					throw new Exception();
				}
				if (_readMode || value == _bufferSize)
				{
					return;
				}
				Flush();
				_bufferSize = value;
				_writeBuffer = new byte[_bufferSize];
			}
		}

		public override void Flush()
		{
			if (_readMode)
			{
				return;
			}

			if (_bufferPosition == 0)
			{
				return;
			}

			AsciiIO.WriteHex(_writeStream, _bufferPosition, false, -1);
			AsciiIO.Write(_writeStream, CRLFString);
			_writeStream.Write(_writeBuffer, 0, _bufferPosition);
			AsciiIO.Write(_writeStream, CRLFString);
			_bufferPosition = 0;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!_readMode)
			{
				throw new NotSupportedException("cannot read when stream opned in write mode");
			}

			if (_reachedEnd)
			{
				// maxed out stream
				return 0;
			}

			int startCount = count;
			while (count > 0)
			{
				if (_chunkLength == 0)
				{
					_chunkLength = _readStream.ReadHexCRLF();

					if (_chunkLength == 0)
					{
						// CRLF last CRLF not related to a block, after everything else
						_readStream.Read(CRLFString);
						_reachedEnd = true;
						break;
					}
				}

				int readLength;
				if (_chunkLength > count)
				{
					readLength = count;
				} else
				{
					readLength = (int)_chunkLength;
				}

				StreamFunctions.ReadUntilComplete(_readStream.Stream, buffer, offset, readLength);
				_chunkLength -= readLength;
				count -= readLength;
				offset += readLength;

				if (_chunkLength == 0)
				{
					// CRLF at the end of a data block
					_readStream.Read(CRLFString);
				}
			}

			return startCount - count;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_readMode)
			{
				throw new NotSupportedException("cannot write when stream opened in read mode");
			}

			while (count > 0)
			{
				while (_bufferPosition < _bufferSize && count > 0)
				{
					_writeBuffer[_bufferPosition] = buffer[offset];
					_bufferPosition++;
					offset++;
					count--;
				}
				if (_bufferPosition == _bufferSize)
				{
					Flush();
				}
			}
		}

	}
}
