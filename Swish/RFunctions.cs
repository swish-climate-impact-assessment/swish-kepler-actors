using System;
using System.Collections.Generic;
using System.IO;

namespace Swish
{
	public static class RFunctions
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
				List<string> programLocations = new List<string>(new string[]{
					"Program Files",
					"Program Files (x86)",
					"R",
					"Bin",
					"",
				});

				List<string> rDirectories = new List<string>(new string[]{
					"R",
					"",
				});

				List<string> versionDirectories = new List<string>(new string[]{
					"R-3.0.0",
					"R-2.15.1",
					"R-*",
					"R*",
					"",
				});

				List<string> binDirectories = new List<string>(new string[]{
					"Bin",
					"",
				});

				List<string> buildDirectories = new List<string>(new string[]{
					"x64",
					"x86",
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
						for (int rDirectoryIndex = 0; rDirectoryIndex < rDirectories.Count; rDirectoryIndex++)
						{
							string rDirectory = rDirectories[rDirectoryIndex];
							testDirectory = Path.Combine(driveLetter, programFiles, rDirectory);
							if (!Directory.Exists(testDirectory))
							{
								continue;
							}
							for (int versionIndex = 0; versionIndex < versionDirectories.Count; versionIndex++)
							{
								string _versionDirectory = versionDirectories[versionIndex];

								List<string> actualVersionDirectories = new List<string>();
								if (!_versionDirectory.Contains("*"))
								{
									string actualVersionDirectory = Path.Combine(driveLetter, programFiles, rDirectory, _versionDirectory);
									actualVersionDirectories.Add(actualVersionDirectory);
								} else
								{
									string searchPath = Path.Combine(driveLetter, programFiles, rDirectory);
									string[] _actualVersionDirectories = Directory.GetDirectories(searchPath, _versionDirectory);
									actualVersionDirectories.AddRange(_actualVersionDirectories);
								}
								for (int actualVersionDirectoryIndex = 0; actualVersionDirectoryIndex < actualVersionDirectories.Count; actualVersionDirectoryIndex++)
								{
									string versionDirectory = actualVersionDirectories[actualVersionDirectoryIndex];
									if (!Directory.Exists(versionDirectory))
									{
										continue;
									}
									for (int binIndex = 0; binIndex < binDirectories.Count; binIndex++)
									{
										string binDirectory = binDirectories[binIndex];
										testDirectory = Path.Combine(versionDirectory, binDirectory);
										if (!Directory.Exists(testDirectory))
										{
											continue;
										}
										for (int buildIndex = 0; buildIndex < binDirectories.Count; buildIndex++)
										{
											string buildDirectory = buildDirectories[buildIndex];
											testDirectory = Path.Combine(versionDirectory, binDirectory, buildDirectory);
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
			fileNames.Add("R.exe");

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
				throw new Exception("Could not find installed version of R");
			}

			return string.Empty;
		}

	}
}

