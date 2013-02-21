using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class ReplaceAdapter
	{
		public string Name { get { return "replace"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
			string outputFileName = splitArguments.OutputFileName();
			string condition = splitArguments.String(Arguments.DefaultArgumentPrefix + "condition", true);
			string value = splitArguments.String(Arguments.DefaultArgumentPrefix + "value", true);
			Replace(inputFileName, outputFileName, condition, value);
			Console.Write(outputFileName);
		}

		public static void Replace(string inputFileName, string outputFileName, string condition, string value)
		{
			//SwishFunctions.MessageTextBox(""
			//+ "inputFileName: " + inputFileName + Environment.NewLine
			//+ "outputFileName: " + outputFileName + Environment.NewLine
			//+ "condition: " + condition + Environment.NewLine
			//+ "value: " + value + Environment.NewLine, false);

			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(condition))
			{
				throw new Exception("condition missing");
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new Exception("value missing");
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
			//replace oldvar =exp [if] [in] [, nopromote]

			lines.Add("replace " + value + " if " + condition);
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

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
			record.Arguments.Add(new Tuple<string, string>("Condition", condition));
			record.Arguments.Add(new Tuple<string, string>("Value", value));
			if (AdapterFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }
			record.ComputerName = Environment.MachineName;
			record.Operation = "Replace";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);

		}

	}
}
