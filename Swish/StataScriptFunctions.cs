using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public static string SaveFileCommand(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension == ".csv")
			{
				return "outsheet using \"" + fileName + "\", comma";
			}
			return "save \"" + fileName + "\"";
		}

		public static void LoadFileCommand(List<string> lines, string fileName)
		{
			lines.Add("clear");
			string extension = Path.GetExtension(fileName);
			if (extension == ".csv")
			{
				lines.Add("insheet using \"" + fileName + "\"");
				return;
			}
			lines.Add("use \"" + fileName + "\"");
		}

		public static string ConvertToStataFormat(List<string> lines, string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension.ToLower() == ".dta")
			{
				return fileName;
			}

			LoadFileCommand(lines, fileName);

			string intermediateFileName = SwishFunctions.TempoaryOutputFileName(".dta");
			if (FileFunctions.FileExists(intermediateFileName))
			{
				File.Delete(intermediateFileName);
			}

			string line = SaveFileCommand(intermediateFileName);
			lines.Add(line);

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

	}
}

