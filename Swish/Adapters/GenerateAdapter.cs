using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class GenerateAdapter: IAdapter
	{
		public string Name { get { return "generate"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
			string outputFileName = splitArguments.OutputFileName(SwishFunctions.DataFileExtension);
			string variableName = splitArguments.String(Arguments.DefaultArgumentPrefix + "variable", true);
			string expression = splitArguments.String(Arguments.DefaultArgumentPrefix + "expression", true);
			StataDataType type = splitArguments.Enum<StataDataType>(Arguments.DefaultArgumentPrefix + "type", false);
			Generate(inputFileName, outputFileName, variableName, type, expression);
			Console.Write(outputFileName);
		}

		public static void Generate(string inputFileName, string outputFileName, string variableName, StataDataType type, string expression)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("Variable missing");
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new Exception("Expression missing");
			}

			string extension = Path.GetExtension(outputFileName);
			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(extension);

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			StataScriptFunctions.Generate(lines, type, variableName, expression);

			StataScriptFunctions.SaveFileCommand(lines, intermaidateOutput);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(intermaidateOutput))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			File.Move(intermaidateOutput, outputFileName);

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("Variable", variableName));
			record.Arguments.Add(new Tuple<string, string>("Type", type.ToString()));
			record.Arguments.Add(new Tuple<string, string>("Expression", expression));
			if (AdapterFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }
			record.ComputerName = Environment.MachineName;
			record.Operation = "Generate";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
		}

	}
}