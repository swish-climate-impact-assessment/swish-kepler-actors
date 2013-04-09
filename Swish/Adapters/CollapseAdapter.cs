using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class CollapseAdapter
	{

		public static double Collapse(string inputFileName, string variable, CollapseOpperation operation)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			string doOutputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(doOutputFileName))
			{
				// Stata does not overwrite files
				File.Delete(doOutputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			// collapse (mean) mean=head4 (median) medium=head4, by(head6)

			lines.Add("collapse " + "(" + StataScriptFunctions.Write(operation) + ") " + variable + "_" + StataScriptFunctions.Write(operation) + "=" + variable);

			StataScriptFunctions.SaveFileCommand(lines, doOutputFileName);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(doOutputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			string[] resultLines = File.ReadAllLines(doOutputFileName);
			if (resultLines.Length < 2)
			{
				throw new Exception("Unexpected format");
			}

			double result = double.Parse(resultLines[1]);

			/// delete all the files not needed
			File.Delete(doOutputFileName);

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("Variable", variable));
			record.Arguments.Add(new Tuple<string, string>("Operation", operation.ToString()));
			if (AdapterFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }
			record.ComputerName = Environment.MachineName;
			record.Operation = "Collapse";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(inputFileName + "out", metadata);

			return result;
		}

	}
}
