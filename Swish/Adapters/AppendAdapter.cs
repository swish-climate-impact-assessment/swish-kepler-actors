using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class AppendAdapter
	{
		public string Name { get { return "append"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string input1FileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument + "1", true));
			string input2FileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument + "2", true));
			string outputFileName = splitArguments.OutputFileName();
			outputFileName = Append(input1FileName, input2FileName, outputFileName);
			Console.Write(outputFileName);
		}

		public static string Append(string input1FileName, string input2FileName, string outputFileName)
		{
			if (!FileFunctions.FileExists(input1FileName))
			{
				throw new Exception("cannot find file \"" + input1FileName + "\"");
			}

			if (!FileFunctions.FileExists(input2FileName))
			{
				throw new Exception("cannot find file \"" + input2FileName + "\"");
			}

			if (
			Path.GetFullPath(input1FileName) == Path.GetFullPath(outputFileName)
			|| Path.GetFullPath(input2FileName) == Path.GetFullPath(outputFileName)
			)
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();

			StataScriptFunctions.WriteHeadder(lines);

			string intermediateFileName = StataScriptFunctions.ConvertToStataFormat(lines, input2FileName);
			StataScriptFunctions.LoadFileCommand(lines, input1FileName);
			lines.Add("append using \"" + intermediateFileName + "\"");
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

			/// delete all the files not needed
			File.Delete(intermediateFileName);

			List<MetadataRecord> metadata = new List<MetadataRecord>();

			if (MetadataFunctions.Exists(input1FileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(input1FileName);
				metadata.AddRange(inputMetadata);
			}

			if (MetadataFunctions.Exists(input2FileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(input2FileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", input1FileName));
			record.Arguments.Add(new Tuple<string, string>("InputFileName", input2FileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			if (AdapterFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }
			record.ComputerName = Environment.MachineName;
			record.Operation = "Append";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);

			return outputFileName;
		}


	}
}