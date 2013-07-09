using System;
using System.Collections;
using System.Collections.Generic;

namespace Swish.IO
{
	public static class StringIO
	{
		public static bool TryRead(string value, ref string line)
		{
			if (!line.StartsWith(value))
			{
				return false;
			}

			line = line.Substring(value.Length);
			return true;
		}

		public static void SkipWhiteSpace(out string buffer, ref string line)
		{
			string line2 = line.TrimStart();
			buffer = line.Substring(0, line.Length - line2.Length);
			line = line2;
		}

		public static bool TryReadString(out string buffer, ref string line)
		{
			string useLine = line;
			if (!TryRead("\"", ref useLine))
			{
				buffer = null;
				return false;
			}

			string useBuffer = string.Empty;
			string subBuffer;
			while (true)
			{
				if (useLine.Length == 0)
				{
					buffer = null;
					return false;
				}

				if (TryRead("\"", ref useLine))
				{
					break;
				}

				if (TryRead("\\", ref useLine))
				{
					if (useLine.Length == 0)
					{
						buffer = null;
						return false;
					}

					char character = useLine[0];
					useLine = useLine.Substring(1);

					switch (character)
					{
					case '{':
					case '/':
					case '}':
					case '\"':
					case '\'':
					case '\\':
					case '=':
						useBuffer += character;
						break;

					case 'r':
						useBuffer += '\r';
						break;

					case 'n':
						useBuffer += '\n';
						break;

					case 't':
						useBuffer += '\t';
						break;

					default:
						useBuffer += '\\';
						useBuffer += character;
						break;
					}
					continue;
				}

				string stopString;
				if (!TryReadUntill(out subBuffer, out stopString, new string[] { "\"", "\\" }, ref useLine))
				{
					buffer = null;
					return false;
				}

				useBuffer += subBuffer;
			}

			buffer = useBuffer;
			line = useLine;
			return true;
		}

		public static bool TryReadUntill(out string buffer, out string stopString, string[] stopStrings, ref string line)
		{
			int minimumCharIndex = int.MaxValue;
			string minimumStopString = null;
			for (int stopIndex = 0; stopIndex < stopStrings.Length; stopIndex++)
			{
				string testStopString = stopStrings[stopIndex];

				int charIndex = line.IndexOf(testStopString);
				if (charIndex >= 0 && (
					(charIndex < minimumCharIndex) ||
					(charIndex == minimumCharIndex && testStopString.Length > minimumStopString.Length)
					))
				{
					minimumCharIndex = charIndex;
					minimumStopString = testStopString;
				}
			}

			if (minimumStopString == null)
			{
				buffer = null;
				stopString = null;
				return false;
			}

			buffer = line.Substring(0, minimumCharIndex);
			stopString = minimumStopString;
			line = line.Substring(minimumCharIndex);
			return true;
		}

		public static string Escape(string value)
		{
			string _value = value.Replace("\\", "\\\\");
			_value = _value.Replace("\"", "\\\"");
			_value = _value.Replace("\'", "\\\'");
			_value = _value.Replace("\r", "\\\r");
			_value = _value.Replace("\n", "\\\n");
			_value = _value.Replace("\t", "\\\t");

			return _value;
		}

		public static bool TryRead(out int value, ref string line)
		{
			string[] parts = line.Split(' ', '\t', ')', '(', ',', '{', '}', '[', ']');
			if (parts.Length == 0)
			{
				value = default(int);
				return false;
			}
			if (!int.TryParse(parts[0], out value))
			{
				value = default(int);
				return false;
			}

			line = line.Substring(parts[0].Length, line.Length - parts[0].Length);
			return true;
		}

		public static bool TryRead(out double value, ref string line)
		{
			string[] parts = line.Split(' ', '\t', ')', '(', ',', '{', '}', '[', ']');
			if (parts.Length == 0)
			{
				value = default(double);
				return false;
			}
			if (!double.TryParse(parts[0], out value))
			{
				value = default(double);
				return false;
			}

			line = line.Substring(parts[0].Length, line.Length - parts[0].Length);
			return true;
		}

		public static bool TryReadToken(out string token, ref string line)
		{
			throw new System.NotImplementedException();
		}

		class StringLengthAlphaComparer: IComparer<string>
		{
			public int Compare(string x, string y)
			{
				// order the largest length first, then alphabetically
				int result = y.Length - x.Length;
				if (result != 0)
				{
					return result;
				}

				return string.Compare(x, y);
			}
		}

		internal static bool TryReadEnum<EnumType>(out EnumType type, ref string line)
		{
			Array _symbols = Enum.GetValues(typeof(EnumType));

			SortedList<string, EnumType> symbols = new SortedList<string, EnumType>(new StringLengthAlphaComparer());
			for (int symbolIndex = 0; symbolIndex < symbols.Count; symbolIndex++)
			{
				EnumType symbol = (EnumType)_symbols.GetValue(symbolIndex);
				symbols.Add(symbol.ToString(), symbol);
			}
			for (int symbolIndex = 0; symbolIndex < symbols.Count; symbolIndex++)
			{
				string symbol = symbols.Keys[symbolIndex];

				if (TryRead(symbol, ref line))
				{
					type = symbols[symbol];
					return true;
				}
			}

			type = default(EnumType);
			return false;
		}
	}
}
