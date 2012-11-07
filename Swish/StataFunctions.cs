using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Swish
{
	public static class StataFunctions
	{
		public const string BatchArgument = "/e do ";
		public const string MergeColumnName = "_merge";

		public static void RunScript(List<string> lines, bool useGui)
		{
			string doFileName = Path.GetTempFileName() + ".do";
			File.WriteAllLines(doFileName, lines.ToArray());

			if (!useGui)
			{
				string arguments = BatchArgument + "\"" + doFileName + "\"";

				// there is also the log to deal with
				string workingDirectory = Path.GetDirectoryName(doFileName);

				int exitCode;
				string output;
				SwishFunctions.RunProcess(StataExecutablePath, arguments, workingDirectory, false, out exitCode, out output);
			} else
			{
				List<string> locations = SwishFunctions.Locations();
				string runDoFileName = SwishFunctions.ResloveFileName("rundo.exe", locations, false, false);

				// run Stata
				RunStata();

				int exitCode;
				string output;
				SwishFunctions.RunProcess(runDoFileName, doFileName, Environment.CurrentDirectory, false, out exitCode, out output);
			}

			File.Delete(doFileName);
		}

		private static void RunStata()
		{
			Process[] processes = Process.GetProcesses();
			List<Process> sataProcesses = new List<Process>();
			for (int processIndex = 0; processIndex < processes.Length; processIndex++)
			{
				Process process = processes[processIndex];
				try
				{
					if (process.ProcessName.ToLower().Contains("stata"))
					{
						sataProcesses.Add(process);
						break;

					}
				} catch { }
			}

			// Version 9 is wstata.exe
			if (sataProcesses.Count > 0)
			{
				return;
			}

			string fileName = StataExecutablePath;

			int exitCode;
			string output;
			SwishFunctions.RunProcess(fileName, "", Environment.CurrentDirectory, true, out exitCode, out output);
		}

		private static string _stataExecutablePath = null;
		public static string StataExecutablePath
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_stataExecutablePath))
				{
					List<string> locations = SwishFunctions.Locations();
					locations.Add(@"C:\Program Files\Stata12\");
					locations.Add(@"C:\Program Files\Stata11\");
					locations.Add(@"C:\Program Files\Stata10\");
					locations.Add(@"C:\Program Files\Stata9\");
					locations.Add(@"C:\Program Files\Stata8\");
					locations.Add(@"C:\Stata9\");

					string fileName = SwishFunctions.ResloveFileName("StataMP.exe", locations, false, true);
					if (!string.IsNullOrWhiteSpace(fileName))
					{
						_stataExecutablePath = fileName;
						return fileName;
					}

					fileName = SwishFunctions.ResloveFileName("StataSE.exe", locations, false, true);
					if (!string.IsNullOrWhiteSpace(fileName))
					{
						_stataExecutablePath = fileName;
						return fileName;
					}

					fileName = SwishFunctions.ResloveFileName("wsestata.exe", locations, false, true);
					if (!string.IsNullOrWhiteSpace(fileName))
					{
						_stataExecutablePath = fileName;
						return fileName;
					}

					fileName = SwishFunctions.ResloveFileName("wstata.exe", locations, false, true);
					if (!string.IsNullOrWhiteSpace(fileName))
					{
						_stataExecutablePath = fileName;
						return fileName;
					}

					throw new Exception("Could not find installed version of Stata");

				}
				return _stataExecutablePath;
			}
			set { _stataExecutablePath = value; }
		}

		public static void Merge(string input1FileName, string input2FileName, List<string> variableNames, string outputFileName)
		{
			if (!File.Exists(input1FileName))
			{
				throw new Exception("cannot find file \"" + input1FileName + "\"");
			}

			if (!File.Exists(input2FileName))
			{
				throw new Exception("cannot find file \"" + input2FileName + "\"");
			}

			if (File.Exists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			if (input1FileName == input2FileName)
			{
				throw new Exception("Cannot merge the same tables");
			}

			string doOutputFileName = Path.GetTempFileName() + ".csv";
			if (File.Exists(doOutputFileName))
			{
				// Stata does not overwrite files
				File.Delete(doOutputFileName);
			}

			string intermediateFileName = Path.GetTempFileName() + ".dta";
			if (File.Exists(intermediateFileName))
			{
				File.Delete(intermediateFileName);
			}

			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			List<string> lines = new List<string>();
			lines.Add("clear");

			string line = StataFunctions.LoadFileCommand(input2FileName);
			lines.Add(line);

			line = SortCommand(variableNames);
			lines.Add(line);
			line = StataFunctions.SaveFileCommand(intermediateFileName);
			lines.Add(line);

			lines.Add("clear");
			line = StataFunctions.LoadFileCommand(input1FileName);
			lines.Add(line);

			line = SortCommand(variableNames);
			lines.Add(line);
			lines.Add("merge " + VariableList(variableNames) + " using \"" + intermediateFileName + "\"");
			lines.Add("drop " + MergeColumnName);
			line = StataFunctions.SaveFileCommand(doOutputFileName);
			lines.Add(line);

			StataFunctions.RunScript(lines, false);

			if (!File.Exists(doOutputFileName))
			{
				throw new Exception("Output file was not created");
			}

			/// move the output file
			if (File.Exists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			File.Move(doOutputFileName, outputFileName);

			/// delete all the files not needed
			File.Delete(intermediateFileName);
		}

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

		public static string LoadFileCommand(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension == ".csv")
			{
				return "insheet using \"" + fileName + "\"";
			}
			return "use using \"" + fileName + "\"";
		}


		public static string ConvertToStataFormat(List<string> lines, string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension.ToLower() == ".dta")
			{
				return fileName;
			}

			lines.Add("clear");
			string line = StataFunctions.LoadFileCommand(fileName);
			lines.Add(line);

			string intermediateFileName = Path.GetTempFileName() + ".dta";
			if (File.Exists(intermediateFileName))
			{
				File.Delete(intermediateFileName);
			}

			line = StataFunctions.SaveFileCommand(intermediateFileName);
			lines.Add(line);

			return intermediateFileName;
		}
	}
}

