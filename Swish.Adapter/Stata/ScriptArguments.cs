using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Swish.IO;

namespace Swish.Stata
{
	public class ScriptArguments
	{
		private SortedList<string, ScriptSymbol> _symbols = new SortedList<string, ScriptSymbol>();
		public SortedList<string/* Name*/, ScriptSymbol> Symbols
		{
			get { return _symbols; }
			set
			{
				_symbols = new SortedList<string, ScriptSymbol>();
				if (value == null)
				{
					return;
				}
				for (int index = 0; index < value.Count; index++)
				{
					string name = value.Keys[index];
					ScriptSymbol symbol = value[name];
					if (symbol.Name != name)
					{
						throw new Exception();
					}
					_symbols.Add(symbol.Name, symbol);
				}
			}
		}

		private SortedList<string, string> _values = new SortedList<string, string>();

		private string _outputFileName = string.Empty;
		public string FinalOutputFileName
		{
			get { return _outputFileName; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_outputFileName = string.Empty;
					return;
				}
				_outputFileName = value;
			}
		}

		public string IntermediateOutputFileName
		{
			get
			{
				if (_symbols == null || !_values.ContainsKey(StataScriptFunctions.OutputFileNameToken))
				{
					return string.Empty;
				}

				return _values[StataScriptFunctions.OutputFileNameToken];
			}
		}

		public SortedList<string, string> InputFileNames
		{
			get
			{
				SortedList<string, string> inputs = new SortedList<string, string>();
				for (int index = 0; index < _symbols.Count; index++)
				{
					string name = _symbols.Keys[index];
					ScriptSymbol symbol = _symbols[name];
					if (symbol.Type != SymbolType.Input)
					{
						continue;
					}


					inputs.Add(name, Value(name));
				}
				return inputs;
			}
		}

		internal void Value(string symbol, string value)
		{
			if (string.IsNullOrWhiteSpace(symbol))
			{
				throw new ArgumentNullException("symbol");
			}

			if (_values.ContainsKey(symbol))
			{
				_values.Remove(symbol);
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				return;
			}
			_values.Add(symbol, value);
		}

		internal string Value(string symbolName)
		{
			if (string.IsNullOrWhiteSpace(symbolName))
			{
				throw new ArgumentNullException("symbol");
			}

			if (!_values.ContainsKey(symbolName))
			{
				if (!_symbols.ContainsKey(symbolName))
				{
					throw new Exception("Undefined symbol \"" + symbolName + "\"");
				}
				ScriptSymbol symbol = _symbols[symbolName];
				if (!symbol.Optional)
				{
					throw new Exception("Symbol \"" + symbolName + "\" value undefined");
				}

				return symbol.DefaultValue;
			}

			return _values[symbolName];
		}

		internal List<Tuple<string, string>> GetTranslations()
		{
			List<Tuple<string, string>> translations = new List<Tuple<string, string>>();
			for (int index = 0; index < _symbols.Count; index++)
			{
				string name = _symbols.Keys[index];
				string value = Value(name);
				translations.Add(new Tuple<string, string>(name, value));
			}

			return translations;
		}
	}
}
