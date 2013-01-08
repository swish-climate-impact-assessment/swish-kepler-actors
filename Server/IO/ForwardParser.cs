using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Swish.Server.IO
{
	/// <summary>
	/// this is rather like a text reader/writer
	/// </summary>
	class ForwardParser: IDisposable
	{
		public ForwardParser(Stream stream, bool leaveOpen)
		{
			if (!stream.CanRead)
			{
				throw new Exception();
			}

			_stream = stream;
			_leaveStreamOpen = leaveOpen;
		}

		~ForwardParser()
		{
			Dispose(false);
		}

		public void Close()
		{
			Dispose(true);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				if (_stream != null)
				{
					throw new Exception("Ascii stream not closed ");
				}
				return;
			}
			GC.SuppressFinalize(this);
			if (_stream == null)
			{
				return;
			}
			if (!_leaveStreamOpen)
			{
				_stream.Flush();
				_stream.Close();
			}
			_stream = null;
		}

		Stream _stream;
		private bool _leaveStreamOpen;

		public bool CanWrite
		{
			get { return false; }
		}

		public bool CanRead
		{
			get
			{
				return _stream != null;
			}
		}

		public bool CanSeek
		{
			get { return false; }
		}

		public void Flush()
		{
		}

		public long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public long Length
		{
			get { throw new NotSupportedException(); }
		}

		public void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public Stream Stream
		{
			get { return _stream; }
		}

		public bool LeaveOpen
		{
			get { return _leaveStreamOpen; }
			set { _leaveStreamOpen = value; }
		}

		public long ReadLongCRLF()
		{
			string longString = string.Empty;
			char character;
			while (true)
			{
				character = ReadChar();
				if (!char.IsDigit(character))
				{
					break;
				}

				longString += character;
			}

			if (longString.Length == 0)
			{
				throw new Exception();
			}
			if (character != '\r')
			{
				throw new Exception();
			}

			character = ReadChar();
			if (character != '\n')
			{
				throw new Exception();
			}

			long result = long.Parse(longString);
			return result;
		}

		private char ReadChar()
		{
			byte[] buffer = new byte[1];
			StreamFunctions.ReadUntilComplete(_stream, buffer, 0, 1);
			string stringBuffer = System.Text.ASCIIEncoding.ASCII.GetString(buffer);
			if (stringBuffer.Length != 1)
			{
				throw new Exception();
			}
			char character = stringBuffer[0];
			return character;
		}

		public void Read(string searchString)
		{
			AsciiIO.Read(_stream, searchString);
		}

		public long ReadHexCRLF()
		{
			// ['-'] 1*('0'-'1') CRLF
			bool negative = false;
			char character = ReadChar();
			if (character == '-')
			{
				negative = true;
				character = '\0';
			} else
			{
				negative = false;
			}

			long value = 0;
			bool loop = true;
			while (true)
			{
				if (character == '\0')
				{
					character = ReadChar();
				}

				switch (character)
				{
				case '0':
					value <<= 4;
					break;

				case '1':
					value = (value << 4) + 1;
					break;

				case '2':
					value = (value << 4) + 2;
					break;

				case '3':
					value = (value << 4) + 3;
					break;

				case '4':
					value = (value << 4) + 4;
					break;

				case '5':
					value = (value << 4) + 5;
					break;

				case '6':
					value = (value << 4) + 6;
					break;

				case '7':
					value = (value << 4) + 7;
					break;

				case '8':
					value = (value << 4) + 8;
					break;

				case '9':
					value = (value << 4) + 9;
					break;

				case 'A':
				case 'a':
					value = (value << 4) + 10;
					break;

				case 'B':
				case 'b':
					value = (value << 4) + 11;
					break;

				case 'C':
				case 'c':
					value = (value << 4) + 12;
					break;

				case 'D':
				case 'd':
					value = (value << 4) + 13;
					break;

				case 'E':
				case 'e':
					value = (value << 4) + 14;
					break;

				case 'F':
				case 'f':
					value = (value << 4) + 15;
					break;

				case '\r':
					loop = false;
					break;

				default:
					throw new Exception("Expected hex digit, found (" + ((int)character).ToString() + ") \'" + character + "\'");
				}
				if (!loop)
				{
					break;
				}
				character = '\0';
			}

			if (character != '\r')
			{
				throw new Exception("Unexpected carriage return found (" + ((int)character).ToString() + ") \'" + character + "\'");
			}

			character = ReadChar();
			if (character != '\n')
			{
				throw new Exception("Unexpected line feed found (" + ((int)character).ToString() + ") \'" + character + "\'");
			}

			if (negative)
			{
				value = -value;
			}
			return value;
		}
	}
}
