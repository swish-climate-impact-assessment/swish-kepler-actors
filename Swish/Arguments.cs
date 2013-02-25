using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Swish
{
	public class Arguments
	{
		public const string DefaultArgumentPrefix = ">";

		public const string ErrorArgument = DefaultArgumentPrefix + "SwishError";
		public const string InputArgument = DefaultArgumentPrefix + "input";
		public const string OperationArgument = DefaultArgumentPrefix + "operation";

		private string _argumentPrefix;
		public string ArgumentPrefix { get { return _argumentPrefix; } }

		private List<Tuple<string, string>> _splitArguments;

		public List<Tuple<string, string>> SplitArguments
		{
			get { return _splitArguments; }
			set
			{
				if (value == null)
				{
					_splitArguments = new List<Tuple<string, string>>();
					return;
				}
				_splitArguments = new List<Tuple<string, string>>(value);
			}
		}

		public string ArgumentString
		{
			get
			{
				return ToString();
			}
		}

		public Arguments()
		{
			_argumentPrefix = DefaultArgumentPrefix;
			_splitArguments = new List<Tuple<string, string>>();
		}

		public Arguments(string[] arguments)
		{
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}

			_argumentPrefix = DefaultArgumentPrefix;

			string stringArguments = string.Join(" ", arguments);
			_splitArguments = Split(stringArguments, _argumentPrefix);
		}

		public Arguments(string arguments)
		{
			if (string.IsNullOrWhiteSpace(arguments))
			{
				throw new ArgumentNullException("arguments");
			}
			_argumentPrefix = DefaultArgumentPrefix;
			_splitArguments = Split(arguments, _argumentPrefix);
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

				if (!StringIO.TryRead(argumentPrefix, ref usedArguments))
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
				name = argumentPrefix + name;

				StringIO.SkipWhiteSpace(out buffer, ref usedArguments);
				string value = ReadArgumentValue(ref usedArguments, argumentPrefix);
				splitArguments.Add(new Tuple<string, string>(name, value));
			}

			return splitArguments;
		}

		private static string ReadArgumentValue(ref string arguments, string argumentPrefix)
		{
			string usedArguments = arguments;
			string value;

			if (StringIO.TryReadString(out value, ref usedArguments))
			{
				arguments = usedArguments;
				return value;
			}

			string stopString;
			if (!StringIO.TryReadUntill(out value, out stopString, new string[] { argumentPrefix }, ref usedArguments))
			{
				value = usedArguments.Trim();
				arguments = string.Empty;
				return value;
			}

			value = value.Trim();
			arguments = usedArguments;
			return value;
		}

		private string Decode(string value)
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
						throw new Exception("Expected ### following escape character '<'");
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

		private int IndexOf(string name)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			if (!name.StartsWith(_argumentPrefix))
			{
				throw new Exception("Invalid argument name \"" + name + "\"");
			}
			int listIndex = -1;
			for (int argumentIndex = 0; argumentIndex < _splitArguments.Count; argumentIndex++)
			{
				Tuple<string, string> item = _splitArguments[argumentIndex];
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

		public string String(string name, bool throwOnMissing)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			int listIndex = IndexOf(name);
			if (listIndex < 0)
			{
				if (throwOnMissing)
				{
					SwishFunctions.MessageTextBox("Argument missing \"" + name + "\"" + Environment.NewLine + ToString(), false);
					throw new Exception("Argument missing \"" + name + "\"");
				}
				return string.Empty;
			}
			string value = _splitArguments[listIndex].Item2;
			if (string.IsNullOrWhiteSpace(value) && throwOnMissing)
			{
				SwishFunctions.MessageTextBox("Argument value missing \"" + name + "\"" + Environment.NewLine + ToString(), false);
				throw new Exception("Argument value missing \"" + name + "\"");
			}

			value = Decode(value);

			return value;
		}

		public void String(string name, string value)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			_splitArguments.Add(new Tuple<string, string>(name, value));
		}

		public List<string> StringList(string name, bool throwOnMissing, bool throwOnEmpty)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			string value = String(name, throwOnMissing);
			List<string> values = new List<string>(value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
			if (values.Count == 0 && throwOnEmpty)
			{
				throw new Exception("Argument empty: \"" + name + "\", expected space seperated argument values");
			}
			return values;
		}

		public bool Exists(string name)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			int listIndex = IndexOf(name);
			if (listIndex < 0)
			{
				return false;
			}

			string value = _splitArguments[listIndex].Item2;

			return true;
		}

		public List<T> EnumList<T>(string name, bool throwOnMissing, bool throwOnEmpty)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			List<string> items = StringList(name, throwOnMissing, throwOnEmpty);

			List<T> results = new List<T>();
			for (int itemIndex = 0; itemIndex < items.Count; itemIndex++)
			{
				string item = items[itemIndex].Trim();
				if (string.IsNullOrWhiteSpace(item))
				{
					continue;
				}
				T value = (T)System.Enum.Parse(typeof(T), item, true);
				results.Add(value);
			}

			return results;
		}

		public T Enum<T>(string name, bool throwOnMissing)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			string stringValue = String(name, throwOnMissing);
			T value = (T)System.Enum.Parse(typeof(T), stringValue, true);
			return value;
		}

		public bool Bool(string name, bool throwOnMissing)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			string stringValue = String(name, throwOnMissing);
			if (string.IsNullOrWhiteSpace(stringValue))
			{
				if (throwOnMissing)
				{
					throw new Exception("Argument missing \"" + name + "\"");
				}
				return false;
			}
			bool value = bool.Parse(stringValue.ToLower());
			return value;
		}

		public override string ToString()
		{
			string argumentText = string.Empty;
			for (int argumentIndex = 0; argumentIndex < _splitArguments.Count; argumentIndex++)
			{
				Tuple<string, string> argument = _splitArguments[argumentIndex];
				argumentText += argument.Item1 + " " + argument.Item2 + " ";
			}
			return argumentText;
		}

		public void Remove(string name)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			int listIndex = IndexOf(name);
			if (listIndex < 0)
			{
				return;
			}

			_splitArguments.RemoveAt(listIndex);
		}

		public double Double(string name, bool throwOnMissing)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			string stringValue = String(name, throwOnMissing);
			if (string.IsNullOrWhiteSpace(stringValue))
			{
				if (throwOnMissing)
				{
					throw new Exception("Argument missing \"" + name + "\"");
				}
				return default(double);
			}
			double value = double.Parse(stringValue);
			return value;
		}

		public DateTime Date(string name, bool throwOnMissing)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			string stringValue = String(name, throwOnMissing);
			if (string.IsNullOrWhiteSpace(stringValue))
			{
				if (throwOnMissing)
				{
					throw new Exception("Argument missing \"" + name + "\"");
				}
				return default(DateTime);
			}

			DateTime value = DateTime.Parse(stringValue);
			return value;
		}

		public void Date(string name, DateTime value)
		{
			if (!name.StartsWith(_argumentPrefix))
			{
				name = _argumentPrefix + name;
			}

			string stringValue = value.ToShortDateString();

			String(name, stringValue);
		}
	}
}
