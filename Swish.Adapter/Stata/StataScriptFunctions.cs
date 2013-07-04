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

		public static void ResloveSymbols(out string outputFileName, out SortedList<string, string> inputFileNames, out List<string> newLines, out List<Tuple<string, string>> symbols, List<string> lines, string intermediateFileName, OperationArguments adapterArguments)
		{
			newLines = new List<string>();
			symbols = new List<Tuple<string, string>>();
			symbols.Add(new Tuple<string, string>("%UserProfile%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
			symbols.Add(new Tuple<string, string>("%StartupPath%", Application.StartupPath));

			outputFileName = string.Empty;
			inputFileNames = new SortedList<string, string>();
			for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
			{
				string _line = lines[lineIndex];
				string line = _line;
				string buffer;
				StringIO.SkipWhiteSpace(out buffer, ref line);
				if (!StringIO.TryRead("//", ref line))
				{
					newLines.Add(line);
					continue;
				}

				StringIO.SkipWhiteSpace(out buffer, ref line);
				if (!StringIO.TryRead("define", ref line))
				{
					newLines.Add("//" + buffer + line);
					continue;
				}

				StringIO.SkipWhiteSpace(out buffer, ref line);
				string type;
				if (!StringIO.TryReadUntill(out type, out buffer, new string[] { " ", "\t", "\r", "\n" }, ref line))
				{
					throw new Exception("Could not read line: \"" + _line + "\"");
				}

				bool optional;
				StringIO.SkipWhiteSpace(out buffer, ref line);
				optional = StringIO.TryRead("optional", ref line);

				StringIO.SkipWhiteSpace(out buffer, ref line);
				string name;
				if (StringIO.TryReadUntill(out name, out buffer, new string[] { " ", "\t", "\r", "\n" }, ref line))
				{
				} else
				{
					name = line.Trim();
					line = string.Empty;
				}
				if (string.IsNullOrWhiteSpace(name))
				{
					throw new Exception("Could not read line: \"" + _line + "\"");
				}
				if (!name.StartsWith("%") && !name.EndsWith("%"))
				{
					name = "%" + name + "%";
				}

				string defaultValue;
				StringIO.SkipWhiteSpace(out buffer, ref line);
				if (StringIO.TryReadString(out defaultValue, ref line))
				{
				}else if (StringIO.TryReadUntill(out defaultValue, out buffer, new string[] { " ", "\t", "\r", "\n" }, ref line))
				{
				} else 
				{
					defaultValue = line;
				}

				switch (type)
				{
				case TemporaryFileType:
					{
						string fileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
						symbols.Add(new Tuple<string, string>(name, fileName));
					}
					break;

				case OutputType:
					{
						string _outputFileName = adapterArguments.String(OutputFileNameToken, false);
						if (string.IsNullOrWhiteSpace(_outputFileName) || _outputFileName.ToLower() == "none" || _outputFileName.ToLower() == "temp")
						{
							_outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
						} else
						{
							_outputFileName = FileFunctions.AdjustFileName(_outputFileName);
						}

						if (!string.IsNullOrWhiteSpace(outputFileName))
						{
							throw new Exception("outputFile defined multiple times");
						}
						outputFileName = _outputFileName;
						symbols.Add(new Tuple<string, string>(name, intermediateFileName));
					}
					break;

				case InputType:
					{
						if (optional)
						{
							throw new Exception("inputFile cannot be optional");
						}

						string fileName = adapterArguments.String(name, !optional);
						fileName = FileFunctions.AdjustFileName(fileName);
						if (string.IsNullOrWhiteSpace(fileName) || !FileFunctions.FileExists(fileName))
						{
							throw new ArgumentException(fileName + " not found");
						}

						string usedFileName = SwishFunctions.ConvertInput(fileName);
						symbols.Add(new Tuple<string, string>(name, usedFileName));
						inputFileNames.Add(name, fileName);
					}
					break;

				case VariableNamesType:
					{
						string stringValue;
						if (adapterArguments.Exists(name))
						{
							List<string> variableNames = adapterArguments.StringList(name, optional, !optional);
							stringValue = StataScriptFunctions.VariableList(variableNames);
						} else
						{
							stringValue = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(stringValue))
						{
							if (!optional)
							{
								throw new Exception("Variables \"" + name + "\" missing");
							}
							stringValue = defaultValue;
						}

						if (!string.IsNullOrWhiteSpace(stringValue))
						{
							symbols.Add(new Tuple<string, string>(name, stringValue));
						}
					}
					break;

				case StringType:
				case TokenType:
				case VariableNameType:
					{
						string stringValue;
						if (adapterArguments.Exists(name))
						{
							stringValue = adapterArguments.String(name, !optional);
						} else
						{
							stringValue = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(stringValue))
						{
							if (!optional)
							{
								throw new Exception(name + " String, token or variable name missing");
							}
							stringValue = defaultValue;
						}

						if (!string.IsNullOrWhiteSpace(stringValue))
						{
							if (type == TokenType || type == VariableNameType)
							{
								string[] fragments = stringValue.Trim().Split(new char[] { '\t', ' ' });
								if (fragments.Length != 1)
								{
									throw new Exception("Expected variable name or token, found \"" + stringValue + "\"");
								}
								stringValue = stringValue.Trim();
							}
							symbols.Add(new Tuple<string, string>(name, stringValue));
						} else if (optional)
						{
							symbols.Add(new Tuple<string, string>(name, defaultValue));
						}
					}
					break;

				case BoolType:
				case DoubleType:
				case DateType:
					{
						string stringValue;

						if (adapterArguments.Exists(name))
						{
							stringValue = adapterArguments.String(name, !optional);
						} else
						{
							stringValue = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(stringValue))
						{
							if (!optional)
							{
								throw new Exception(name + "Boolean double or date missing");
							}
							stringValue = defaultValue;
						}

						if (type == BoolType)
						{
							if (!string.IsNullOrWhiteSpace(stringValue))
							{
								bool value = bool.Parse(stringValue.ToLower());
								symbols.Add(new Tuple<string, string>(name, stringValue));
							} else
							{
								bool value = false;
								symbols.Add(new Tuple<string, string>(name, value.ToString()));
							}

						} else if (type == DoubleType)
						{
							if (!string.IsNullOrWhiteSpace(stringValue))
							{
								double value = double.Parse(stringValue);
								symbols.Add(new Tuple<string, string>(name, stringValue));
							} else
							{
								double value = 0;
								symbols.Add(new Tuple<string, string>(name, value.ToString()));
							}
						} else if (type == DateType)
						{
							if (!string.IsNullOrWhiteSpace(stringValue))
							{
								DateTime value = DateTime.Parse(stringValue);
								symbols.Add(new Tuple<string, string>(name, stringValue));
							} else
							{
								DateTime value = new DateTime();
								symbols.Add(new Tuple<string, string>(name, value.ToShortDateString()));
							}
						}
					}
					break;

				default:
					throw new Exception("Unknown argument type \"" + type + "\" ");
				}
			}
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

				line += variable;
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

