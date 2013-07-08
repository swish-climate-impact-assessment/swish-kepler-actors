using System;
using System.Collections.Generic;
using Swish.IO;

namespace Swish
{
	public static class ArgumentParser
	{
		public const string DefaultArgumentPrefix = ">";

		public const string ErrorArgument = "SwishError";
		public const string InputArgument = "input";
		public const string OperationArgument = "operation";

		public static Arguments Empty
		{
			get { return new Arguments(DefaultArgumentPrefix, new List<Tuple<string, string>>()); }
		}

		public static Arguments Read(string[] arguments)
		{
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}


			string stringArguments = string.Join(" ", arguments);
			List<Tuple<string, string>> splitArguments = Split(stringArguments, DefaultArgumentPrefix);

			return new Arguments(DefaultArgumentPrefix, splitArguments);

		}

		public static Arguments Read(string arguments)
		{
			if (string.IsNullOrWhiteSpace(arguments))
			{
				throw new ArgumentNullException("arguments");
			}

			List<Tuple<string, string>> splitArguments = Split(arguments, DefaultArgumentPrefix);

			return new Arguments(DefaultArgumentPrefix, splitArguments);

		}

		private static List<Tuple<string, string>> Split(string arguments, string argumentPrefix)
		{
			string usedArguments = arguments;
			List<Tuple<string, string>> splitArguments = new List<Tuple<string, string>>();
			while (true)
			{
				string buffer;
				StringIO.SkipWhiteSpace(out buffer, ref usedArguments);
				if (usedArguments.Length == 0)
				{
					break;
				}

				if (!TryReadNormalOrString(argumentPrefix, ref usedArguments))
				{
					throw new Exception("Malformed arguments 1: \"" + arguments + "\"");
				}

				StringIO.SkipWhiteSpace(out buffer, ref usedArguments);
				string name = ReadArgumentName(ref usedArguments, argumentPrefix);

				StringIO.SkipWhiteSpace(out buffer, ref usedArguments);
				string value = ReadArgumentValue(ref usedArguments, argumentPrefix);

				splitArguments.Add(new Tuple<string, string>(name, value));
			}


			return splitArguments;
		}

		private static bool TryReadNormalOrString(string value, ref string line)
		{
			string buffer;
			if (StringIO.TryRead(value, ref line))
			{
			} else if (TryReadWrappedStrings(out buffer, ref line))
			{
				buffer = Clean(buffer);
				if (buffer != value)
				{
					return false;
				}
			} else
			{
				return false;
			}
			return true;
		}

		private static string ReadArgumentName(ref string line, string argumentPrefix)
		{
			string name;
			string stopString;
			if (TryReadWrappedStrings(out name, ref line))
			{
			} else if (StringIO.TryReadUntill(out name, out stopString, new string[] { " ", "\"", argumentPrefix }, ref line))
			{
			} else
			{
				name = line;
				line = string.Empty;
			}

			name = Clean(name).ToLower();
			return name;
		}

		private static string ReadArgumentValue(ref string arguments, string argumentPrefix)
		{
			string usedArguments = arguments;

			string value;
			string stopString;
			if (TryReadWrappedStrings(out value, ref usedArguments))
			{
			} else if (StringIO.TryReadUntill(out value, out stopString, new string[] { argumentPrefix }, ref usedArguments))
			{
			} else
			{
				value = usedArguments;
				usedArguments = string.Empty;
			}

			value = Clean(value);

			arguments = usedArguments;
			return value;
		}

		private static string Clean(string text)
		{
			while (true)
			{
				string startValue = text;

				string buffer;
				StringIO.SkipWhiteSpace(out buffer, ref text);
				if (TryReadWrappedStrings(out buffer, ref text))
				{
					text = buffer;
				}
				text = text.Trim();

				if (text == startValue)
				{
					break;
				}
			}
			return text;
		}

		private static bool TryReadWrappedStrings(out string buffer, ref string line)
		{
			// there may be string inside strings with different encodings

			string usedLine = line;
			string stringBuffer;
			if (StringIO.TryReadString(out stringBuffer, ref usedLine))
			{
			} else if (TryReadEncodedString(out stringBuffer, ref usedLine))
			{
			} else
			{
				buffer = null;
				return false;
			}

			string stringStringBuffer;
			if (TryReadWrappedStrings(out stringStringBuffer, ref stringBuffer))
			{
				stringBuffer = stringStringBuffer;
			}

			buffer = stringBuffer;
			line = usedLine;
			return true;
		}

		private static bool TryReadEncodedString(out string buffer, ref string line)
		{
			string useLine = line;

			char character;
			if (!TryReadEncodedCharacter(out character, ref useLine) || character != '\"')
			{
				buffer = null;
				return false;
			}

			string useBuffer = string.Empty;
			string subBuffer;
			while (true)
			{
				if (useLine.Length < 4)
				{
					buffer = null;
					return false;
				}

				if (TryReadEncodedCharacter(out character, ref useLine))
				{
					if (character == '\"')
					{
						break;
					}

					useBuffer += character;
					continue;
				}

				string stopString;
				if (!StringIO.TryReadUntill(out subBuffer, out stopString, new string[] { "<" }, ref useLine))
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

		private static bool TryReadEncodedCharacter(out char character, ref string line)
		{
			string useLine = line;
			if (!StringIO.TryRead("<", ref useLine))
			{
				character = '\0';
				return false;
			}

			if (useLine.Length < 3)
			{
				throw new Exception("Expected ### following escape character '<'");
			}

			string code = useLine.Substring(0, 3);
			useLine = useLine.Substring(3);
			uint number;
			if (!uint.TryParse(code, out number))
			{
				throw new Exception("Bad escape sequance \"" + line.Substring(4) + "\"");
			}

			character = (char)number;
			line = useLine;
			return true;
		}

	}
}