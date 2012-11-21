using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Swish
{
	public static class StataFunctions
	{
		public const string BatchArgument = "/e do ";

		public static string RunScript(List<string> lines, bool useGui)
		{
			string tempFileName = Path.GetTempFileName();
			string doFileName = tempFileName + ".do";
			File.WriteAllLines(doFileName, lines.ToArray());

			string log = RunScript(doFileName, useGui);

			if (File.Exists(tempFileName))
			{
				File.Delete(tempFileName);
			}
			File.Delete(doFileName);
			return log;
		}

		public static string RunScript(string doFileName, bool useGui)
		{
			string log;
			if (!useGui)
			{
				string arguments = BatchArgument + "\"" + doFileName + "\"";

				// there is also the log to deal with
				string workingDirectory = Path.GetDirectoryName(doFileName);

				int exitCode;
				string output;
				SwishFunctions.RunProcess(StataExecutablePath, arguments, workingDirectory, false, out exitCode, out output);

				string logFileName = Path.GetFileName(doFileName);
				int index = logFileName.IndexOf('.');
				if (index >= 0)
				{
					logFileName = logFileName.Substring(0, index);
				}

				logFileName = Path.Combine(workingDirectory, logFileName + ".log");

				if (File.Exists(logFileName))
				{
					log = File.ReadAllText(logFileName);
					File.Delete(logFileName);
				} else
				{
					log = string.Empty;
				}
			} else
			{
				List<string> locations = SwishFunctions.Locations();
				string runDoFileName = SwishFunctions.ResloveFileName("rundo.exe", locations, false, false);

				// run Stata
				RunStata();

				int exitCode;
				string output;
				SwishFunctions.RunProcess(runDoFileName, doFileName, Environment.CurrentDirectory, false, out exitCode, out output);
				log = string.Empty;
			}
			return log;
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
					locations.Add(@"C:\Program Files (x86)\Stata12\");
					locations.Add(@"C:\Stata12\");
					locations.Add(@"C:\Program Files\Stata11\");
					locations.Add(@"C:\Program Files (x86)\Stata11\");
					locations.Add(@"C:\Stata11\");
					locations.Add(@"C:\Program Files\Stata10\");
					locations.Add(@"C:\Program Files (x86)\Stata10\");
					locations.Add(@"C:\Stata10\");
					locations.Add(@"C:\Program Files\Stata9\");
					locations.Add(@"C:\Program Files (x86)\Stata9\");
					locations.Add(@"C:\Stata9\");
					locations.Add(@"C:\Program Files\Stata8\");
					locations.Add(@"C:\Program Files (x86)\Stata8\");
					locations.Add(@"C:\Stata8\");

					List<string> fileNames = new List<string>();
					fileNames.Add("StataSE-64.exe");
					fileNames.Add("StataSE-32.exe");
					fileNames.Add("StataSE.exe");
					fileNames.Add("StataMP-64.exe");
					fileNames.Add("StataMP-32.exe");
					fileNames.Add("StataMP.exe");
					fileNames.Add("wsestata-64.exe");
					fileNames.Add("wsestata-32.exe");
					fileNames.Add("wsestata.exe");
					fileNames.Add("wstata-64.exe");
					fileNames.Add("wstata-32.exe");
					fileNames.Add("wstata.exe");

					for (int fileIndex = 0; fileIndex < fileNames.Count; fileIndex++)
					{
						string file = fileNames[fileIndex];
						string fileName = SwishFunctions.ResloveFileName(file, locations, false, true);
						if (!string.IsNullOrWhiteSpace(fileName))
						{
							_stataExecutablePath = fileName;
							return fileName;
						}
					}

					throw new Exception("Could not find installed version of Stata");

				}
				return _stataExecutablePath;
			}
			set { _stataExecutablePath = value; }
		}

	}
}

