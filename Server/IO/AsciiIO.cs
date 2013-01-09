using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Swish.Server.IO
{
	public static class AsciiIO
	{
		public static void WriteHex(Stream stream, long value, bool write0x, int minimumDigits)
		{
			if (value < 0)
			{
				Write(stream, '-');
				value = -value;
			}

			if (write0x)
			{
				Write(stream, "0x");
			}

			List<char> buffer = new List<char>();
			do
			{
				long characterValue = value & 0xF;
				switch (characterValue)
				{
				case 0:
					buffer.Add('0');
					break;

				case 1:
					buffer.Add('1');
					break;

				case 2:
					buffer.Add('2');
					break;

				case 3:
					buffer.Add('3');
					break;

				case 4:
					buffer.Add('4');
					break;

				case 5:
					buffer.Add('5');
					break;

				case 6:
					buffer.Add('6');
					break;

				case 7:
					buffer.Add('7');
					break;

				case 8:
					buffer.Add('8');
					break;

				case 9:
					buffer.Add('9');
					break;

				case 0xA:
					buffer.Add('A');
					break;

				case 0xB:
					buffer.Add('B');
					break;

				case 0xC:
					buffer.Add('C');
					break;

				case 0xD:
					buffer.Add('D');
					break;

				case 0xE:
					buffer.Add('E');
					break;

				case 0xF:
					buffer.Add('F');
					break;

				default:
					throw new Exception("");
				}

				value >>= 4;
			} while (value > 0);

			while (buffer.Count < minimumDigits)
			{
				buffer.Add('0');
			}
			for (int characterIndex = buffer.Count - 1; characterIndex >= 0; characterIndex--)
			{
				Write(stream, buffer[characterIndex]);
			}
		}

		public static void Write(Stream stream, string buffer)
		{
			byte[] bytes = ASCIIEncoding.ASCII.GetBytes(buffer);
			stream.Write(bytes, 0, bytes.Length);
		}

		public static void Write(Stream stream, char character)
		{
			char[] chars = new char[] { character };
			byte[] bytes = ASCIIEncoding.ASCII.GetBytes(chars);
			stream.WriteByte(bytes[0]);
		}

		internal static void Write(Stream stream, string buffer, int offset, int count)
		{
			string subString = buffer.Substring(offset, count);
			byte[] bytes = ASCIIEncoding.ASCII.GetBytes(subString);
			stream.Write(bytes, 0, bytes.Length);
		}

		public static void Read(Stream stream, string searchString)
		{
			byte[] bytes = ASCIIEncoding.ASCII.GetBytes(searchString);
			byte[] buffer = new byte[bytes.Length];
			StreamFunctions.ReadUntilComplete(stream, buffer, 0, buffer.Length);
			if (!EqualFunctions.Equal(buffer, bytes, buffer.Length))
			{
				throw new Exception();
			}
		}
	}
}
