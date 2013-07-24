using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Swish.IO;

namespace Swish.Stata
{
	public static class StataScriptFunctions
	{
		public const string InputFileNameToken = "%Input%";
		public const string Input1FileNameToken = "%Input1%";
		public const string Input2FileNameToken = "%Input2%";
		public const string OutputFileNameToken = "%Output%";
		public const string IntermediateFileNameToken = "%TemporaryFile%";

		public const string ExpressionToken = "%Expression%";

		public const string ValueToken = "%Value%";

		public const string LeftVariableNameToken = "%LeftVariableName%";
		public const string RightVariableNameToken = "%RightVariableName%";
		public const string VariableNameToken = "%VariableName%";
		public const string ResultVariableNameToken = "%ResultVariableName%";
		public const string VariableNamesToken = "%VariableNames%";

		public const string OutputType = "output";
		public const string InputType = "input";
		public const string TemporaryFileType = "temporaryFile";

		public const string VariableNamesType = "variableNames";
		public const string VariableNameType = "variableName";

		public const string StringType = "string";
		public const string TokenType = "token";

		public const string BoolType = "bool";
		public const string DoubleType = "double";
		public const string DateType = "date";

		internal static SortedList<string, ScriptSymbol> ReadSymbols(List<string> lines)
		{
			SortedList<string, ScriptSymbol> symbols = new SortedList<string, ScriptSymbol>();
			for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
			{
				string line = lines[lineIndex];

				ScriptSymbol symbol;
				if (!TryReadSymbol(line, out symbol))
				{
					continue;
				}

				symbols.Add(symbol.Name, symbol);
			}
			return symbols;
		}

		private static bool TryReadSymbol(string line, out ScriptSymbol symbol)
		{
			bool optional;
			string name;
			string defaultValue;

			string buffer;
			StringIO.SkipWhiteSpace(out buffer, ref line);
			if (!StringIO.TryRead("//", ref line))
			{
				symbol = null;
				return false;
			}

			StringIO.SkipWhiteSpace(out buffer, ref line);
			if (!StringIO.TryRead("define", ref line))
			{
				symbol = null;
				return false;
			}

			StringIO.SkipWhiteSpace(out buffer, ref line);
			SymbolType type = SymbolType.Unknown;
			if (StringIO.TryRead(TemporaryFileType, ref line))
			{
				type = SymbolType.TemporaryFile;
			} else if (StringIO.TryRead(OutputType, ref line))
			{
				type = SymbolType.Output;
			} else if (StringIO.TryRead(InputType, ref line))
			{
				type = SymbolType.Input;
			} else if (StringIO.TryRead(VariableNamesType, ref line))
			{
				type = SymbolType.VariableNameList;
			} else if (StringIO.TryRead(VariableNameType, ref line))
			{
				type = SymbolType.VariableName;
			} else if (StringIO.TryRead(StringType, ref line))
			{
				type = SymbolType.String;
			} else if (StringIO.TryRead(TokenType, ref line))
			{
				type = SymbolType.Token;
			} else if (StringIO.TryRead(BoolType, ref line))
			{
				type = SymbolType.Bool;
			} else if (StringIO.TryRead(DoubleType, ref line))
			{
				type = SymbolType.Double;
			} else if (StringIO.TryRead(DateType, ref line))
			{
				type = SymbolType.Date;
			} else
			{
				symbol = null;
				return false;
			}

			StringIO.SkipWhiteSpace(out buffer, ref line);
			optional = StringIO.TryRead("optional", ref line);

			if (type == SymbolType.Input && optional)
			{
				throw new Exception("inputFile cannot be optional");
			}

			StringIO.SkipWhiteSpace(out buffer, ref line);
			if (StringIO.TryReadUntill(out name, out buffer, new string[] { " ", "\t", "\r", "\n" }, ref line))
			{
			} else
			{
				name = line.Trim();
				line = string.Empty;
			}
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new Exception("Could not read line: \"" + line + "\"");
			}
			if (!name.StartsWith("%") && !name.EndsWith("%"))
			{
				name = "%" + name + "%";
			}

			StringIO.SkipWhiteSpace(out buffer, ref line);
			if (StringIO.TryReadString(out defaultValue, ref line))
			{
			} else if (StringIO.TryReadUntill(out defaultValue, out buffer, new string[] { " ", "\t", "\r", "\n" }, ref line))
			{
			} else
			{
				defaultValue = line;
			}

			symbol = new ScriptSymbol(type, optional, name, defaultValue);
			return true;
		}

		internal static List<ScriptArguments> SortInputs(SortedList<string, ScriptSymbol> symbols, OperationArguments adapterArguments)
		{
			List<ScriptArguments> argumentList = new List<ScriptArguments>();

			// test if the arguments have multiple sets of inputs


			ScriptArguments arguments = new ScriptArguments();
			arguments.Symbols = symbols;

			for (int symbolIndex = 0; symbolIndex < symbols.Count; symbolIndex++)
			{
				string name = symbols.Keys[symbolIndex];
				ScriptSymbol symbol = symbols[name];

				switch (symbol.Type)
				{
				case SymbolType.TemporaryFile:
					{
						string fileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
						arguments.Value(symbol.Name, fileName);
					}
					break;

				case SymbolType.Output:
					{
						string _outputFileName = adapterArguments.String(OutputFileNameToken, false);
						if (string.IsNullOrWhiteSpace(_outputFileName) || _outputFileName.ToLower() == "none" || _outputFileName.ToLower() == "temp")
						{
							_outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
						} else
						{
							_outputFileName = FileFunctions.AdjustFileName(_outputFileName);
						}
						string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
						arguments.Value(symbol.Name, intermediateFileName);
						if (!string.IsNullOrWhiteSpace(arguments.FinalOutputFileName))
						{
							throw new Exception("outputFile defined multiple times");
						}
						arguments.FinalOutputFileName = _outputFileName;
					}
					break;

				case SymbolType.Input:
					{
						string fileName = adapterArguments.String(symbol.Name, true);
						fileName = FileFunctions.AdjustFileName(fileName);
						if (!FileFunctions.FileExists(fileName))
						{
							throw new ArgumentException(fileName + " not found");
						}

						string usedFileName = SwishFunctions.ConvertInput(fileName);
						arguments.Value(symbol.Name, usedFileName);
					}
					break;

				case SymbolType.VariableNameList:
					{
						List<string> variableNameList = adapterArguments.StringList(symbol.Name, symbol.Optional, !symbol.Optional);
						string variableNames = StataScriptFunctions.VariableList(variableNameList);

						if (string.IsNullOrWhiteSpace(variableNames))
						{
							variableNames = symbol.DefaultValue;
						}
						arguments.Value(symbol.Name, variableNames);
					}
					break;

				case SymbolType.VariableName:
					{
						string variableName = adapterArguments.String(symbol.Name, !symbol.Optional);

						if (string.IsNullOrWhiteSpace(variableName))
						{
							variableName = symbol.DefaultValue;
						}

						if (!string.IsNullOrWhiteSpace(variableName))
						{
							string[] fragments = variableName.Trim().Split(new char[] { '\t', ' ' });
							if (fragments.Length != 1)
							{
								throw new Exception("Expected variable symbol.Name, found \"" + variableName + "\"");
							}
						}
						arguments.Value(symbol.Name, variableName.ToLower());
					}
					break;

				case SymbolType.String:
				case SymbolType.Token:
					{
						string stringValue = adapterArguments.String(symbol.Name, !symbol.Optional);

						if (string.IsNullOrWhiteSpace(stringValue))
						{
							stringValue = symbol.DefaultValue;
						}

						if (!string.IsNullOrWhiteSpace(stringValue))
						{
							if (symbol.Type == SymbolType.Token)
							{
								string[] fragments = stringValue.Trim().Split(new char[] { '\t', ' ' });
								if (fragments.Length != 1)
								{
									throw new Exception("Expected variable symbol.Name or token, found \"" + stringValue + "\"");
								}
								stringValue = stringValue.Trim();
							}
						}
						arguments.Value(symbol.Name, stringValue);
					}
					break;

				case SymbolType.Bool:
				case SymbolType.Double:
				case SymbolType.Date:
					{
						string stringValue = adapterArguments.String(symbol.Name, !symbol.Optional);

						if (string.IsNullOrWhiteSpace(stringValue))
						{
							stringValue = symbol.DefaultValue;
						}

						if (symbol.Type == SymbolType.Bool)
						{
							if (!string.IsNullOrWhiteSpace(stringValue))
							{
								bool value = bool.Parse(stringValue.ToLower());
								arguments.Value(symbol.Name, stringValue);
							} else
							{
								bool value = false;
								arguments.Value(symbol.Name, value.ToString());
							}
						} else if (symbol.Type == SymbolType.Double)
						{
							if (!string.IsNullOrWhiteSpace(stringValue))
							{
								double value = double.Parse(stringValue);
								arguments.Value(symbol.Name, stringValue);
							} else
							{
								double value = 0;
								arguments.Value(symbol.Name, value.ToString());
							}
						} else if (symbol.Type == SymbolType.Date)
						{
							if (!string.IsNullOrWhiteSpace(stringValue))
							{
								DateTime value = DateTime.Parse(stringValue);
								arguments.Value(symbol.Name, stringValue);
							} else
							{
								DateTime value = new DateTime();
								arguments.Value(symbol.Name, value.ToShortDateString());
							}
						}
					}
					break;

				case SymbolType.Unknown:
				default:
					throw new Exception("Unknown argument symbol.Type \"" + symbol.Type + "\" ");
				}
			}
			argumentList.Add(arguments);

			return argumentList;
		}

		public const string MergeColumnName = "_merge";

		public static void SortCommand(List<string> lines, string variableNames)
		{
			if (!string.IsNullOrWhiteSpace(variableNames))
			{
				lines.Add("sort " + variableNames + ", stable");
			} else
			{
				lines.Add("sort *, stable");
			}
		}

		public static string VariableList(List<string> variableNames)
		{
			string line = string.Empty;

			for (int variableIndex = 0; variableIndex < variableNames.Count; variableIndex++)
			{
				string variable = variableNames[variableIndex];

				line += variable.ToLower();
				if (variableIndex + 1 < variableNames.Count)
				{
					line += " ";
				}
			}

			return line;
		}

		public static void SaveFileCommand(List<string> lines, string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension == SwishFunctions.CsvFileExtension)
			{
				lines.Add("outsheet using \"" + fileName + "\", comma");
				return;
			}
			lines.Add("save \"" + fileName + "\"");
		}

		public static void LoadFileCommand(List<string> lines, string fileName)
		{
			lines.Add("clear");
			string extension = Path.GetExtension(fileName);
			if (extension == SwishFunctions.CsvFileExtension)
			{
				//lines.Add("insheet using \"" + fileName + "\", names clear comma case");
				lines.Add("insheet using \"" + fileName + "\", names clear comma");
				return;
			}
			lines.Add("use \"" + fileName + "\"");
		}

		public static string ConvertToStataFormat(List<string> lines, string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension.ToLower() == SwishFunctions.DataFileExtension)
			{
				return fileName;
			}

			LoadFileCommand(lines, fileName);

			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			if (FileFunctions.FileExists(intermediateFileName))
			{
				File.Delete(intermediateFileName);
			}

			SaveFileCommand(lines, intermediateFileName);

			return intermediateFileName;
		}

		public static string Write(CollapseOpperation operation)
		{
			switch (operation)
			{
			case CollapseOpperation.IQR:
				return "iqr";

			case CollapseOpperation.Min:
				return "min";

			case CollapseOpperation.Max:
				return "max";

			case CollapseOpperation.Count:
				return "count";

			case CollapseOpperation.Median:
				return "median";

			case CollapseOpperation.Sd:
				return "sd";

			case CollapseOpperation.Sum:
				return "sum";

			case CollapseOpperation.Mean:
				return "mean";

			case CollapseOpperation.Unknown:
			default:
				throw new Exception("Unknown CollapseOpperation");
			}
		}

		public static void WriteHeadder(List<string> lines)
		{
			lines.Add("set more off");
			//lines.Add("noisily {");
			lines.Add("set output proc");
			//lines.Add("set mem 1g");
		}

		public static void WriteFooter(List<string> lines)
		{
			//lines.Add("}");
		}

		public static void DropVariable(List<string> lines, string variable)
		{
			lines.Add("capture confirm variable " + variable);
			lines.Add("if (_rc == 0){");
			lines.Add("\tdrop " + variable);
			lines.Add("}");
		}

		public static void RenameVariable(List<string> lines, string oldName, string newName)
		{
			lines.Add("rename " + oldName + " " + newName);
		}

		public static string TemporaryVariableName()
		{
			string variableName = "variable" + SwishFunctions.TemporaryVariableId().ToString();
			return variableName;
		}

		public static void Generate(List<string> lines, StataDataType type, string variableName, string expression)
		{
			string intermediateName = TemporaryVariableName();
			if (type == StataDataType.Unknown)
			{
				lines.Add(" generate " + intermediateName + " = " + expression);
			} else
			{
				lines.Add(" generate " + type.ToString().ToLower() + " " + intermediateName + " = " + expression);
			}

			DropVariable(lines, variableName);
			RenameVariable(lines, intermediateName, variableName);
		}

		public static void Generate(List<string> lines, string type, string variableName, string expression)
		{
			string intermediateName = TemporaryVariableName();
			if (string.IsNullOrWhiteSpace(type))
			{
				lines.Add(" generate " + intermediateName + " = " + expression);
			} else
			{
				lines.Add(" generate " + type.ToString() + " " + intermediateName + " = " + expression);
			}

			DropVariable(lines, variableName);
			RenameVariable(lines, intermediateName, variableName);
		}

		public static void Format(List<string> lines, string variable, string format)
		{
			lines.Add("format " + variable + " " + format);
		}

		public static void Replace(List<string> lines, string value, string expression)
		{
			lines.Add("replace " + value + " if " + expression);
		}

	}
}

