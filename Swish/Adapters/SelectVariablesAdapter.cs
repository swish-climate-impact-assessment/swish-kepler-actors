using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class SelectVariablesAdapter: IAdapter
	{
		public string Name { get { return "selectColumns"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
			List<string> variableNames = splitArguments.StringList(Arguments.DefaultArgumentPrefix + "variables", true, true);
			string outputFileName = splitArguments.OutputFileName();
			Select(inputFileName, outputFileName, variableNames);
			Console.Write(outputFileName);
		}

		public static void Select(string inputFileName, string outputFileName, List<string> variableNames)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			string line = "keep " + StataScriptFunctions.VariableList(variableNames);
			lines.Add(line);
			StataScriptFunctions.SaveFileCommand(lines, outputFileName);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

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
			if (AdapterFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }
			record.ComputerName = Environment.MachineName;
			record.Operation = "SelectColumns";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
		}

	}
}