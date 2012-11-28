using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Swish
{
	public static class KeplerFunctions
	{
		public static void Run()
		{
			string fileName = ExecutablePath;

			int exitCode;
			string output;
			SwishFunctions.RunProcess(fileName, "", Environment.CurrentDirectory, true, TimeSpan.Zero, out exitCode, out output);
		}

		public static void RunWorkflow(string worflowFileName, List<Tuple<string, string>> parameters)
		{
			string arguments = "-runkar -nogui " + worflowFileName;
			for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
			{
				Tuple<string, string> parameter = parameters[parameterIndex];
				arguments += " -" + parameter.Item1 + " " + parameter.Item2;
			}

			string workingDirectory = System.IO.Path.GetDirectoryName(ExecutablePath);
			int exitCode;
			string output;
			SwishFunctions.RunProcess(ExecutablePath, arguments, workingDirectory, false, new TimeSpan(0, 0, 0, 8, 0), out exitCode, out output);
		}

		private static List<string> PotentialKeplerPaths
		{
			get
			{
				List<string> locations = SwishFunctions.Locations();
				locations.Add(@"C:\Program Files\Kepler-2.3\");
				locations.Add(@"C:\Program Files (x86)\Kepler-2.3\");
				locations.Add(@"C:\Kepler-2.3\");
				locations.Add(@"C:\Program Files\Kepler\");
				locations.Add(@"C:\Program Files (x86)\Kepler\");
				locations.Add(@"C:\Kepler\");
				return locations;
			}
		}
		private static string _cmdExecutablePath = null;
		public static string CmdExecutablePath
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_cmdExecutablePath))
				{
					List<string> fileNames = new List<string>();
					fileNames.Add("kepler.bat");

					for (int fileIndex = 0; fileIndex < fileNames.Count; fileIndex++)
					{
						string file = fileNames[fileIndex];
						string fileName = SwishFunctions.ResloveFileName(file, PotentialKeplerPaths, false, true);
						if (!string.IsNullOrWhiteSpace(fileName))
						{
							_cmdExecutablePath = fileName;
							return fileName;
						}
					}

					throw new Exception("Could not find installed version of Kepler");
				}

				return _cmdExecutablePath;
			}
			set { _cmdExecutablePath = value; }
		}

		private static string _executablePath = null;
		public static string ExecutablePath
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_executablePath))
				{
					List<string> fileNames = new List<string>();
					fileNames.Add("kepler.exe");

					for (int fileIndex = 0; fileIndex < fileNames.Count; fileIndex++)
					{
						string file = fileNames[fileIndex];
						string fileName = SwishFunctions.ResloveFileName(file, PotentialKeplerPaths, false, true);
						if (!string.IsNullOrWhiteSpace(fileName))
						{
							_executablePath = fileName;
							return fileName;
						}
					}

					throw new Exception("Could not find installed version of Kepler");

				}
				return _executablePath;
			}
			set { _executablePath = value; }
		}

	}
}

