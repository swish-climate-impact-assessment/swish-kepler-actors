using System;
using System.Collections.Generic;
using System.IO;

namespace Swish
{
	public static class StataScriptFunctions
	{
		public const string MergeColumnName = "_merge";

		public static string SortCommand(List<string> variableNames)
		{
			string line;
			if (variableNames.Count > 0)
			{
				line = "sort " + VariableList(variableNames) + ", stable";
			} else
			{
				line = "sort *, stable";
			}
			return line;
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
				lines.Add("insheet using \"" + fileName + "\", names clear comma case");
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

		internal static string Write(CollapseOpperation operation)
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

		public static void TryDropVariable(List<string> lines, string variable)
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

		private static int _variableNameCount;
		public static string TemporaryVariableName()
		{
			string variableName = "Variable" + _variableNameCount.ToString();
			_variableNameCount++;
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

			TryDropVariable(lines, variableName);
			RenameVariable(lines, intermediateName, variableName);
		}

	}
}

