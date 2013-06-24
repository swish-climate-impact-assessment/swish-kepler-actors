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
			string doFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DoFileExtension);
			File.WriteAllLines(doFileName, lines.ToArray());

			//SwishFunctions.MessageTextBox(string.Join(Environment.NewLine, lines));

			string log = RunScript(doFileName, useGui);

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
				string workingDirectory = Path.GetDirectoryName(Path.GetFullPath(doFileName));
				//string workingDirectory = Environment.CurrentDirectory;

				ProcessFunctions.Run(ExecutablePath, arguments, workingDirectory, false, TimeSpan.Zero, false, false, false);

				string logFileName = Path.GetFileName(doFileName);
				int index = logFileName.IndexOf('.');
				if (index >= 0)
				{
					logFileName = logFileName.Substring(0, index);
				}

				logFileName = Path.Combine(workingDirectory, logFileName + ".log");

				if (FileFunctions.FileExists(logFileName))
				{
					log = File.ReadAllText(logFileName);
					File.Delete(logFileName);
				} else
				{
					log = string.Empty;
				}
			} else
			{
				List<string> locations = FileFunctions.Locations();
				string runDoFileName = FileFunctions.ResloveFileName("rundo.exe", locations, false, false);

				// run Stata
				Run();

				ProcessFunctions.Run(runDoFileName, doFileName, Environment.CurrentDirectory, false, TimeSpan.Zero, false, false, false);
				log = string.Empty;
			}
			return log;
		}

		private static void Run()
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

			string fileName = ExecutablePath;

			ProcessFunctions.Run(fileName, "", Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, false);
		}

		public static string ExecutablePath
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_executablePath))
				{
					FindExecutablePath();
					if (string.IsNullOrWhiteSpace(_executablePath))
					{
						throw new Exception("Could not find installed version of Stata");
					}
				}
				return _executablePath;
			}
			set { _executablePath = value; }
		}


		private static string _binDirectory = null;
		public static string BinDirectory
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_binDirectory))
				{
					_binDirectory = Path.GetDirectoryName(ExecutablePath);
				}
				return _binDirectory;
			}
			set { _binDirectory = value; }
		}

		private static string _executablePath = null;
		private static void FindExecutablePath()
		{
			List<string> locations = FileFunctions.Locations();
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
				string fileName = FileFunctions.ResloveFileName(file, locations, false, true);
				if (!string.IsNullOrWhiteSpace(fileName))
				{
					_executablePath = fileName;
					return;
				}
			}

			_executablePath = null;
		}

		public static bool Installed
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_executablePath))
				{
					FindExecutablePath();
					if (string.IsNullOrWhiteSpace(_executablePath))
					{
						return false;
					}
				}
				return true;
			}
		}


	}
}

