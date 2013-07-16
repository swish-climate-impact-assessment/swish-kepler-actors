using System;
using System.Collections.Generic;
using System.IO;
using Swish.IO;
using Swish.Stata;

namespace Swish.Adapters
{
	public class SansInputStataCommandAdapter: IOperation
	{
		public string Name { get { return "StataCommandSansInput"; } }

		public string Run(OperationArguments splitArguments)
		{
			string command = splitArguments.String(ArgumentParser.DefaultArgumentPrefix + "command", true);
			string outputFileName = splitArguments.OutputFileName(SwishFunctions.DataFileExtension);
			StataCommand(outputFileName, command);
			return outputFileName;
		}

		public static string StataCommand(string outputFileName, string command)
		{
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			/// create the do file
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			lines.Add(command);

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

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("Command", command));
			if (OperationFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }
			record.ComputerName = Environment.MachineName;
			record.Operation = "StataCommand";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);

			return log;
		}

	}
}
