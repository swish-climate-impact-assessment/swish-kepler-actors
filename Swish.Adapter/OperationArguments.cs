using System;
using System.Collections.Generic;
using Swish.IO;

namespace Swish
{
	public class OperationArguments
	{
		private Arguments _arguments;

		private char _fixingCharacter = '%';
		public char FixingCharacter
		{
			get { return _fixingCharacter; }
			set { _fixingCharacter = value; }
		}

		public OperationArguments(Arguments splitArguments)
		{
			if (splitArguments == null)
			{
				throw new ArgumentNullException("splitArguments");
			}
			_arguments = splitArguments;

			List<Tuple<string, string>> lowerCaseArguments = new List<Tuple<string, string>>();
			for (int argumentIndex = 0; argumentIndex < _arguments.SplitArguments.Count; argumentIndex++)
			{
				string name = _arguments.SplitArguments[argumentIndex].Item1;
				string value = _arguments.SplitArguments[argumentIndex].Item2;
				name = name.Trim(_fixingCharacter);

				lowerCaseArguments.Add(new Tuple<string, string>(name, value));
			}
			_arguments.SplitArguments = lowerCaseArguments;
		}

		public string String(string name, bool throwOnMissing)
		{
			name = name.Trim(_fixingCharacter);
			return _arguments.String(name, throwOnMissing);
		}

		public string OutputFileName(string extension)
		{
			string outputFileName = String("output" + "", false);
			outputFileName = FileFunctions.AdjustFileName(outputFileName);
			if (string.IsNullOrWhiteSpace(outputFileName) || outputFileName.ToLower() == "none" || outputFileName.ToLower() == "temp")
			{
				outputFileName = FileFunctions.TempoaryOutputFileName(extension);
			}
			return outputFileName;
		}

		public void SetOutputFileName(string outputFileName)
		{
			_arguments.String("output", outputFileName);
		}

		public string ArgumentString { get { return _arguments.ArgumentString; } }

		public bool Exists(string name)
		{
			name = name.Trim(_fixingCharacter);
			return _arguments.Exists(name);
		}

		public bool Bool(string name, bool throwOnMissing)
		{
			name = name.Trim(_fixingCharacter);
			return _arguments.Bool(name, throwOnMissing);
		}

		public T Enum<T>(string name, bool throwOnMissing)
		{
			name = name.Trim(_fixingCharacter);
			return _arguments.Enum<T>(name, throwOnMissing);
		}

		public DateTime Date(string name, bool throwOnMissing)
		{
			name = name.Trim(_fixingCharacter);
			return _arguments.Date(name, throwOnMissing);
		}

		public string InputFileName()
		{
			string fileName = _arguments.String(Arguments.InputArgument, true);
			fileName = FileFunctions.AdjustFileName(fileName);
			return fileName;
		}

		public List<string> VariableNames()
		{
			List<string> variables;
			if (Exists("variables"))
			{
				variables = _arguments.StringList(Arguments.DefaultArgumentPrefix + "variables", true, true);
			} else if (Exists("Variables"))
			{
				variables = _arguments.StringList(Arguments.DefaultArgumentPrefix + "Variables", true, true);
			} else if (Exists("variableNames"))
			{
				variables = _arguments.StringList(Arguments.DefaultArgumentPrefix + "variableNames", true, true);
			} else if (Exists("VariableNames"))
			{
				variables = _arguments.StringList(Arguments.DefaultArgumentPrefix + "VariableNames", true, true);
			} else
			{
				throw new Exception("Argument  \"VariableNames\" not found");
			}
			return variables;
		}

		public string InputFileName(int number)
		{
			string variableName = Arguments.InputArgument + number.ToString();
			string fileName = _arguments.String(variableName, true);
			fileName = FileFunctions.AdjustFileName(fileName);
			return fileName;
		}

		public string VariableName()
		{
			string variableName = _arguments.String(Arguments.DefaultArgumentPrefix + "variable", true);
			return variableName;
		}

		public double Double(string name, bool throwOnMissing)
		{
			name = name.Trim(_fixingCharacter);
			return _arguments.Double(name, throwOnMissing);
		}

		public int Int(string name, bool throwOnMissing)
		{
			name = name.Trim(_fixingCharacter);
			return _arguments.Int(name, throwOnMissing);
		}

		public string ResultVariableName()
		{
			string resultVariableName = _arguments.String(Arguments.DefaultArgumentPrefix + "resultVariable", false);
			if (!string.IsNullOrWhiteSpace(resultVariableName))
			{
				return resultVariableName;
			}
			resultVariableName = _arguments.String(Arguments.DefaultArgumentPrefix + "ResultVariableName", false);
			if (!string.IsNullOrWhiteSpace(resultVariableName))
			{
				return resultVariableName;
			}
			return OperationFunctions.WorkingVariableName;

		}

		public List<Tuple<string, string>> SplitArguments
		{
			get { return _arguments.SplitArguments; }
		}


		public void Remove(string name)
		{
			name = name.Trim(_fixingCharacter);
			_arguments.Remove(name);
		}

		internal List<string> StringList(string name, bool throwOnMissing, bool throwOnEmpty)
		{
			name = name.Trim(_fixingCharacter);
			return _arguments.StringList(name, throwOnMissing, throwOnEmpty);
		}
	}
}
