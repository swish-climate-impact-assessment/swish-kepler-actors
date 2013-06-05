using System;
using System.Collections.Generic;
using System.IO;

namespace Swish
{
	public static class KeplerFunctions
	{
		public static void Run()
		{
			string fileName = ExecutablePath;

			ProcessFunctions.Run(fileName, "", Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, false);
		}

		public static void RunWorkflow(string workflowFileName, List<Tuple<string, string>> parameters)
		{
			if (!FileFunctions.FileExists(workflowFileName))
			{
				throw new Exception("cannot find workflow file: \"" + workflowFileName + "\"");
			}

			string arguments = "-runkar -nogui " + workflowFileName;
			if (parameters != null)
			{
				for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
				{
					Tuple<string, string> parameter = parameters[parameterIndex];
					arguments += " -" + parameter.Item1 + " " + parameter.Item2;
				}
			}

			string workingDirectory = Path.GetDirectoryName(ExecutablePath);
			ProcessFunctions.Run(ExecutablePath, arguments, workingDirectory, false, new TimeSpan(0, 0, 0, 8, 0), false, false, false);
		}

		private static List<string> PotentialKeplerPaths
		{
			get
			{
				List<string> programLocations = new List<string>(new string[]{
					"Program Files",
					"Program Files (x86)",
					"Kepler",
					"Bin",
					"",
				});

				List<string> keplerDirectories = new List<string>(new string[]{
					"Kepler-2.3",
					"Kepler-2.4",
					"Kepler-*",
					"Kepler*",
					"",
				});

				List<string> locations = FileFunctions.Locations();
				for (int letter = 0; letter < 26; letter++)
				{
					string driveLetter = (char)('A' + letter) + ":\\";
					if (!Directory.Exists(driveLetter))
					{
						continue;
					}

					for (int programLocationIndex = 0; programLocationIndex < programLocations.Count; programLocationIndex++)
					{
						string programFiles = programLocations[programLocationIndex];
						string testDirectory = Path.Combine(driveLetter, programFiles);
						if (!Directory.Exists(testDirectory))
						{
							continue;
						}

						for (int keplerDirectoryIndex = 0; keplerDirectoryIndex < keplerDirectories.Count; keplerDirectoryIndex++)
						{
							string keplerDirectory = keplerDirectories[keplerDirectoryIndex];

							string testKeplerDirectory = Path.Combine(driveLetter, programFiles, keplerDirectory);
							if (Directory.Exists(testKeplerDirectory))
							{
								locations.Add(testKeplerDirectory);
							}
						}
					}
				}

				locations.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
				locations.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

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
						string fileName = FileFunctions.ResloveFileName(file, PotentialKeplerPaths, false, true);
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
					if (!Directory.Exists(BinDirectory))
					{
						throw new Exception("Could not find installed version of Kepler");
					}
					_executablePath = Path.Combine(BinDirectory, "kepler.exe");
				}
				return _executablePath;
			}
			set { _executablePath = value; }
		}

		private static string _keplerBin = null;
		public static string BinDirectory
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_keplerBin))
				{
					List<string> fileNames = new List<string>();
					fileNames.Add("kepler.exe");

					for (int fileIndex = 0; fileIndex < fileNames.Count; fileIndex++)
					{
						string file = fileNames[fileIndex];
						string fileName = FileFunctions.ResloveFileName(file, PotentialKeplerPaths, false, true);
						if (!string.IsNullOrWhiteSpace(fileName))
						{
							if (string.IsNullOrWhiteSpace(_executablePath))
							{
								_executablePath = fileName;
							}
							_keplerBin = Path.GetDirectoryName(fileName);
							return _keplerBin;
						}
					}
					throw new Exception("Could not find installed version of Kepler");
				}
				return _keplerBin;
			}
			set { _keplerBin = value; }
		}

		public static bool Installed
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_keplerBin))
				{
					return true;
				}
				List<string> fileNames = new List<string>();
				fileNames.Add("kepler.exe");

				for (int fileIndex = 0; fileIndex < fileNames.Count; fileIndex++)
				{
					string file = fileNames[fileIndex];
					string fileName = FileFunctions.ResloveFileName(file, PotentialKeplerPaths, false, true);
					if (!string.IsNullOrWhiteSpace(fileName))
					{
						if (string.IsNullOrWhiteSpace(_executablePath))
						{
							_executablePath = fileName;
						}
						_keplerBin = Path.GetDirectoryName(fileName);
						return true;
					}
				}
				return false;
			}
		}
	}
}

