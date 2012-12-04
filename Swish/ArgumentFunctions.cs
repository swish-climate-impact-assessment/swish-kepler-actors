﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Swish
{
	public static class ArgumentFunctions
	{
		public const string ArgumentCharacter = ">";

		public const string ErrorArgument = ArgumentCharacter + "SwishError";
		public const string InputArgument = ArgumentCharacter + "input";

		public static string GetArgument(string name, List<Tuple<string, string>> splitArguments, bool throwOnMissing)
		{
			int listIndex = IndexOf(name, splitArguments);
			if (listIndex < 0)
			{
				if (throwOnMissing)
				{
					throw new Exception("Argument missing \"" + name + "\"");
				}
				return string.Empty;
			}
			string value = splitArguments[listIndex].Item2;
			if (string.IsNullOrWhiteSpace(value) && throwOnMissing)
			{
				throw new Exception("Argument missing \"" + name + "\"");
			}

			value = Decode(value);

			return value;
		}

		private static string Decode(string value)
		{
			string decodedValue = string.Empty;

			while (true)
			{
				if (value.Length == 0)
				{
					break;
				}

				if (StringIO.TryRead("<", ref value))
				{
					if (value.Length < 3)
					{
						throw new Exception();
					}

					string code = value.Substring(0, 3);
					value = value.Substring(3, value.Length - 3);
					char character = (char)uint.Parse(code);
					decodedValue += character;
					continue;
				}

				string buffer;
				string stopString;
				if (!StringIO.TryReadUntill(out buffer, out stopString, new string[] { "<" }, ref value))
				{
					decodedValue += value;
					value = string.Empty;
					continue;
				}
				decodedValue += buffer;
			}

			return decodedValue;
		}

		private static int IndexOf(string name, List<Tuple<string, string>> splitArguments)
		{
			if (!name.StartsWith(ArgumentCharacter))
			{
				throw new Exception("Invalid argument name \"" + name + "\"");
			}
			int listIndex = -1;
			for (int argumentIndex = 0; argumentIndex < splitArguments.Count; argumentIndex++)
			{
				Tuple<string, string> item = splitArguments[argumentIndex];
				if (item.Item1 == name)
				{
					if (listIndex != -1)
					{
						throw new Exception("Duplicate argument: \"" + name + "\"");
					}
					listIndex = argumentIndex;
				}
			}
			return listIndex;
		}

		public static List<string> GetArgumentItems(string name, List<Tuple<string, string>> splitArguments, bool throwOnMissing, bool throwOnEmpty)
		{
			string value = GetArgument(name, splitArguments, throwOnMissing);
			List<string> values = new List<string>(value.Split(' '));
			if (values.Count == 0 && throwOnEmpty)
			{
				throw new Exception("Argument empty: \"" + name + "\", expected space seperated argument values");
			}
			return values;
		}

		public static List<Tuple<string, string>> SplitArguments(string[] arguments)
		{
			string stringArguments = string.Join(" ", arguments);
			return SplitArguments(stringArguments);
		}

		public static List<Tuple<string, string>> SplitArguments(string arguments)
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

				if (!StringIO.TryRead(ArgumentCharacter, ref usedArguments))
				{
					throw new Exception("Malformed arguments 1: \"" + arguments + "\"");
				}

				string name;
				string stopString;
				if (!StringIO.TryReadUntill(out name, out stopString, new string[] { " " }, ref usedArguments))
				{
					name = usedArguments;
					usedArguments = string.Empty;
				}
				name = ArgumentCharacter + name;

				StringIO.SkipWhiteSpace(out buffer, ref usedArguments);
				string value = ReadArgumentValue(ref usedArguments);
				splitArguments.Add(new Tuple<string, string>(name, value));
			}

			return splitArguments;
		}

		private static string ReadArgumentValue(ref string arguments)
		{
			string usedArguments = arguments;
			string value;

			if (StringIO.TryReadString(out value, ref usedArguments))
			{
				arguments = usedArguments;
				return value;
			}

			string stopString;
			if (!StringIO.TryReadUntill(out value, out stopString, new string[] { ArgumentCharacter }, ref usedArguments))
			{
				value = usedArguments.Trim();
				arguments = string.Empty;
				return value;
			}

			value = value.Trim();
			arguments = usedArguments;
			return value;
		}

		public static bool GetSwitch(string name, List<Tuple<string, string>> splitArguments)
		{
			int listIndex = IndexOf(name, splitArguments);
			if (listIndex < 0)
			{
				return false;
			}

			string value = splitArguments[listIndex].Item2;

			if (!string.IsNullOrWhiteSpace(value))
			{
				throw new Exception();
			}

			return true;
		}


		public static string GetOutputFileName(List<Tuple<string, string>> splitArguments)
		{
			string outputFileName = ArgumentFunctions.GetArgument(ArgumentCharacter + "output" + "", splitArguments, false);
			outputFileName = FileFunctions.AdjustFileName(outputFileName);
			if (string.IsNullOrWhiteSpace(outputFileName) || outputFileName.ToLower() == "none" || outputFileName.ToLower() == "temp")
			{
				outputFileName = SwishFunctions.TempoaryOutputFileName(".dta");
			}
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			return outputFileName;
		}
	}
}
