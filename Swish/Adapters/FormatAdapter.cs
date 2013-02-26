using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FormatAdapter: IAdapter
	{
		public string Name { get { return "format"; } }
		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string outputFileName = splitArguments.OutputFileName(SwishFunctions.DataFileExtension);
			List<string> variableNames = splitArguments.VariableNames();
			string format = splitArguments.String(Arguments.DefaultArgumentPrefix + "format", true);
			Format(inputFileName, outputFileName, variableNames, format);
			Console.Write(outputFileName);
		}

		private static void Format(string inputFileName, string outputFileName, List<string> variableNames, string format)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			if (string.IsNullOrWhiteSpace(format))
			{
				throw new Exception("Format missing");
			}

			string extension = Path.GetExtension(outputFileName);
			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(extension);

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			lines.Add("format " + StataScriptFunctions.VariableList(variableNames) + " " + format);

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
			record.Arguments.Add(new Tuple<string, string>("VariableNames", string.Join(" ", variableNames)));
			record.Arguments.Add(new Tuple<string, string>("format", format));
			if (AdapterFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }

			record.ComputerName = Environment.MachineName;
			record.Operation = "Format";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
		}


	}
}
