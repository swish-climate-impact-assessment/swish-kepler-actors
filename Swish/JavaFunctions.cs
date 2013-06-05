using System;
using System.Collections.Generic;
using System.IO;

namespace Swish
{
	public static class JavaFunctions
	{
		private static string _executablePath = null;
		public static string ExecutablePath
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_executablePath))
				{
					_executablePath = GetExecutablePath(true);
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

		public static bool Installed
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_executablePath))
				{
					_executablePath = GetExecutablePath(false);
				}
				return !string.IsNullOrWhiteSpace(_executablePath);
			}
		}

		private static List<string> PotentialPaths
		{
			get
			{

				// C:\Program Files\Java\jdk1.6.0_25\bin\java.exe
				// C:\Program Files\Java\jre6\bin\java.exe
				List<string> programLocations = new List<string>(new string[]{
					"Program Files",
					"Program Files (x86)",
					"Java",
					"Bin",
					"Swish",
					"",
				});

				List<string> javaDirectories = new List<string>(new string[]{
					"Java",
					"",
				});

				List<string> versionDirectories = new List<string>(new string[]{
					"jdk1.6.0_25",
					"jdk*",
					"jre6",
					"jre*",
					"",
				});

				List<string> binDirectories = new List<string>(new string[]{
					"Bin",
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
						for (int javaDirectoryIndex = 0; javaDirectoryIndex < javaDirectories.Count; javaDirectoryIndex++)
						{
							string rDirectory = javaDirectories[javaDirectoryIndex];
							testDirectory = Path.Combine(driveLetter, programFiles, rDirectory);
							if (!Directory.Exists(testDirectory))
							{
								continue;
							}
							for (int versionIndex = 0; versionIndex < versionDirectories.Count; versionIndex++)
							{
								string versionDirectory = versionDirectories[versionIndex];
								testDirectory = Path.Combine(driveLetter, programFiles, rDirectory, versionDirectory);
								if (!Directory.Exists(testDirectory))
								{
									continue;
								}
								for (int binIndex = 0; binIndex < binDirectories.Count; binIndex++)
								{
									string binDirectory = binDirectories[binIndex];
									testDirectory = Path.Combine(driveLetter, programFiles, rDirectory, versionDirectory, binDirectory);
									if (!Directory.Exists(testDirectory))
									{
										continue;
									}
									testDirectory = Path.Combine(driveLetter, programFiles, rDirectory, versionDirectory, binDirectory);
									if (!Directory.Exists(testDirectory))
									{
										continue;
									}
									locations.Add(testDirectory);
								}
							}
						}
					}
				}

				locations.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
				locations.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

				return locations;
			}
		}

		private static string GetExecutablePath(bool errorOnMissing)
		{
			List<string> fileNames = new List<string>();
			fileNames.Add("java.exe");

			for (int fileIndex = 0; fileIndex < fileNames.Count; fileIndex++)
			{
				string file = fileNames[fileIndex];
				string fileName = FileFunctions.ResloveFileName(file, PotentialPaths, false, true);
				if (!string.IsNullOrWhiteSpace(fileName))
				{
					if (string.IsNullOrWhiteSpace(_executablePath))
					{
						_executablePath = fileName;
					}
					return fileName;
				}
			}
			if (errorOnMissing)
			{
				throw new Exception("Could not find installed version of Java");
			}

			return string.Empty;
		}

	}
}

