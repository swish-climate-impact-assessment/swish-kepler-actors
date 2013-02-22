using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class MergeAdapter
	{
		public string Name { get { return "merge"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string input1FileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument + "1", true));
			string input2FileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument + "2", true));
			List<string> variableNames = splitArguments.StringList(Arguments.DefaultArgumentPrefix + "variables", true, true);
			string outputFileName = splitArguments.OutputFileName();
			List<MergeRecordResult> keep = splitArguments.EnumList<MergeRecordResult>(Arguments.DefaultArgumentPrefix + "keep", false, false);

			string keepMergeString = FileFunctions.AdjustFileName(splitArguments.String(Arguments.DefaultArgumentPrefix + "keepMerge", false));
			bool keepMerge;
			if (!string.IsNullOrWhiteSpace(keepMergeString))
			{
				keepMerge = bool.Parse(keepMergeString.ToLower());
			} else
			{
				keepMerge = false;
			}

			Merge(input1FileName, input2FileName, variableNames, outputFileName, keepMerge, keep);
			Console.Write(outputFileName);
		}

		public static void Merge(string input1FileName, string input2FileName, List<string> variableNames, string outputFileName, bool keepMergeColumn, List<MergeRecordResult> keep)
		{
			//if (!FileFunctions.FileExists(input1FileName))
			//{
			//    throw new Exception("cannot find file \"" + input1FileName + "\"");
			//}

			//if (!FileFunctions.FileExists(input2FileName))
			//{
			//    throw new Exception("cannot find file \"" + input2FileName + "\"");
			//}

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

			if (input1FileName == input2FileName)
			{
				throw new Exception("Cannot merge the same tables");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			string doOutputFileName = FileFunctions.TempoaryOutputFileName(Path.GetExtension(outputFileName));
			if (FileFunctions.FileExists(doOutputFileName))
			{
				// Stata does not overwrite files
				File.Delete(doOutputFileName);
			}

			string intermediateFileName = FileFunctions.TempoaryOutputFileName(".dta");
			if (FileFunctions.FileExists(intermediateFileName))
			{
				File.Delete(intermediateFileName);
			}

			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, input2FileName);

			string line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);
			line = StataScriptFunctions.SaveFileCommand(intermediateFileName);
			lines.Add(line);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, input1FileName);

			line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);

			//line = "merge 1:1 " + StataScriptFunctions.VariableList(variableNames) + " using \"" + intermediateFileName + "\", force ";
			//if (keep != null && keep.Count > 0)
			//{
			//    line += "keep(";

			//    if (keep.Contains(MergeRecordResult.Match))
			//    {
			//        line += "match ";
			//    }
			//    if (keep.Contains(MergeRecordResult.MatchConflict))
			//    {
			//        line += "match_conflict ";
			//    }
			//    if (keep.Contains(MergeRecordResult.MatchUpdate))
			//    {
			//        line += "match_update ";
			//    }
			//    if (keep.Contains(MergeRecordResult.Using))
			//    {
			//        line += "using ";
			//    }

			//    line += ") ";
			//}
			//lines.Add(line);

			line = "merge " + StataScriptFunctions.VariableList(variableNames) + ", using \"" + intermediateFileName + "\"";
			lines.Add(line);

			if (!keepMergeColumn)
			{
				lines.Add("drop " + StataScriptFunctions.MergeColumnName);
			}
			line = StataScriptFunctions.SaveFileCommand(doOutputFileName);
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(doOutputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			/// move the output file
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			File.Move(doOutputFileName, outputFileName);

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
			record.Arguments.Add(new Tuple<string, string>("VariableNames", string.Join(" ", variableNames)));
			record.Arguments.Add(new Tuple<string, string>("KeepMergeColumn", keepMergeColumn.ToString()));
			if (AdapterFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }
			if (keep != null)
			{
				string keepString = string.Join(" ", keep);
				record.Arguments.Add(new Tuple<string, string>("Keep", keepString));
			}
			record.ComputerName = Environment.MachineName;
			record.Operation = "Merge";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
		}

	}
}