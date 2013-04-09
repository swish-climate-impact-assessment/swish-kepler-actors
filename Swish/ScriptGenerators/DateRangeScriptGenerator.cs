using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class DateRangeScriptGenerator: IScriptGenerator
	{
		public const string NameString = "DateRange";
		public string Name { get { return NameString; } }

		public const string StartDateToken = "%StartDate%";
		public const string EndDateToken = "%EndDate%";

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + StataScriptFunctions.ResultVariableNameToken + " " + AdapterFunctions.WorkingVariableName);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.DateType + " " + StartDateToken);
			lines.Add("// define " + StataScriptFunctions.DateType + " " + EndDateToken);

			string expression = "1";
			StataScriptFunctions.Generate(lines, StataDataType.Unknown, StataScriptFunctions.ResultVariableNameToken, expression);
			StataScriptFunctions.Format(lines, StataScriptFunctions.ResultVariableNameToken, "%td");

			AddObservation(lines);

			string value = StataScriptFunctions.ResultVariableNameToken + " = date(\"" + StartDateToken + "\", \"DMY\")";
			expression = "missing(" + StataScriptFunctions.ResultVariableNameToken + ")";
			StataScriptFunctions.Replace(lines, value, expression);

			AddObservation(lines);

			value = StataScriptFunctions.ResultVariableNameToken + " = date(\"" + EndDateToken + "\", \"DMY\")";
			expression = "missing(" + StataScriptFunctions.ResultVariableNameToken + ")";
			StataScriptFunctions.Replace(lines, value, expression);

			lines.Add("tsset " + StataScriptFunctions.ResultVariableNameToken);
			lines.Add("tsfill");

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

		private static void AddObservation(List<string> lines)
		{
			string localVariable = StataScriptFunctions.TemporaryVariableName();
			lines.Add("local " + localVariable + " = _N + 1");
			lines.Add("set obs `" + localVariable + "'");
		}
	}
}


