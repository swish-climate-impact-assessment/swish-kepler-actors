using System;
using System.IO;
using System.Text;

namespace Swish.Server.IO
{
	static class RawStreamIO
	{
		public static string ReadString(Stream stream)
		{
			if (ReadBool(stream))
			{
				return null;
			}

			StringBuilder text = new StringBuilder();
			int length = ReadInt(stream);
			for (int characterIndex = 0; characterIndex < length; characterIndex++)
			{
				char character = ReadChar(stream);
				text.Append(character);
			}
			string value = text.ToString();
			return value;
		}

		public static void Write(Stream stream, string value)
		{
			if (value == null)
			{
				Write(stream, true);
				return;
			}
			Write(stream, false);

			Write(stream, value.Length);
			for (int characterIndex = 0; characterIndex < value.Length; characterIndex++)
			{
				char character = value[characterIndex];
				Write(stream, character);
			}
		}

		public static void Write(Stream stream, bool value)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		public static bool ReadBool(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			int size = sizeof(bool);
			byte[] buffer = new byte[size];
			StreamFunctions.ReadUntilComplete(stream, buffer, 0, size);
			bool result = BitConverter.ToBoolean(buffer, 0);
			return result;
		}

		public static void Write(Stream stream, char value)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		public static char ReadChar(Stream stream)
		{
			byte[] buffer = new byte[sizeof(char)];
			StreamFunctions.ReadUntilComplete(stream, buffer, 0, sizeof(char));
			char character = BitConverter.ToChar(buffer, 0);
			return character;
		}

		public static void Write(Stream stream, int value)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		public static int ReadInt(Stream stream)
		{
			byte[] buffer = new byte[sizeof(int)];
			StreamFunctions.ReadUntilComplete(stream, buffer, 0, sizeof(int));
			int integer = BitConverter.ToInt32(buffer, 0);
			return integer;
		}

		public static void Write(Stream stream, long value)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		public static long ReadLong(Stream stream)
		{
			byte[] buffer = new byte[sizeof(long)];
			StreamFunctions.ReadUntilComplete(stream, buffer, 0, sizeof(long));
			long integer = BitConverter.ToInt64(buffer, 0);
			return integer;
		}

		public static byte[] ReadByteArray(Stream stream)
		{
			if (ReadBool(stream))
			{
				return null;
			}

			int length = ReadInt(stream);
			byte[] result = new byte[length];
			StreamFunctions.ReadUntilComplete(stream, result, 0, length);
			return result;
		}

		public static void Write(Stream stream, byte[] value)
		{
			if (value == null)
			{
				Write(stream, true);
				return;
			}
			Write(stream, false);

			Write(stream, value.Length);
			stream.Write(value, 0, value.Length);
		}

	}
}