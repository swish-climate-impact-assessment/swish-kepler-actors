using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Swish
{
	public static class FileFunctions
	{
		public static List<string> Locations()
		{
			List<string> binaryLocations = new List<string>(new string[]{
					@"C:\Swish\bin"
				});

			string fileName = ResloveFileName("DevelopmentLocations.txt", binaryLocations, false, true);
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return binaryLocations;
			}

			List<string> locations = new List<string>(File.ReadAllLines(fileName));
			locations.AddRange(binaryLocations);
			return locations;
		}

		public static string ResloveFileName(string fileName, List<string> possibleLocations, bool ignoreApplicationDirectory, bool ignoreError)
		{
			List<string> attempts = new List<string>();

			for (int locationIndex = 0; locationIndex < possibleLocations.Count; locationIndex++)
			{
				string location = possibleLocations[locationIndex];

				string path = Path.Combine(location, fileName);
				attempts.Add(path);
			}

			attempts.Add(fileName);

			string file = Path.Combine(Environment.CurrentDirectory, fileName);
			attempts.Add(file);

			file = Path.Combine(Application.StartupPath, fileName);
			attempts.Add(file);

			file = Path.Combine(Application.StartupPath, fileName);
			attempts.Add(file);

			file = Path.Combine(Path.GetTempPath(), fileName);
			attempts.Add(file);

			for (int attemptIndex = 0; attemptIndex < attempts.Count; attemptIndex++)
			{
				string path = attempts[attemptIndex];
				if (FileExists(path))
				{
					string realFileName = Path.GetFullPath(path);
					if (!ignoreApplicationDirectory || realFileName != Application.ExecutablePath)
					{
						return realFileName;
					}
				}
			}

			if (!ignoreError)
			{
				throw new Exception("Could not find location of \"" + fileName + "\"" + Environment.NewLine + string.Join(Environment.NewLine, attempts));
			}

			return string.Empty;
		}

		public static string AdjustFileName(string fileName)
		{
			bool monoEnvironment = !SwishFunctions.MonoEnvironment;
			if (monoEnvironment)
			{
				fileName = fileName.Replace('/', '\\');
			} else
			{
				fileName = fileName.Replace('\\', '/');
			}
			string str = fileName;
			return str;
		}

		public static bool FileExists(string fileName)
		{
			if (File.Exists(fileName))
			{
				return true;
			}

			if (File.Exists(fileName + ".dta"))
			{
				return true;
			}

			return false;
		}

		public static void DeleteFile(string fileName)
		{
			try
			{
				if (!FileExists(fileName))
				{
					return;
				}
				File.SetAttributes(fileName, FileAttributes.Normal);
				File.Delete(fileName);
				string destinationDirectory = Path.GetDirectoryName(fileName);
				DeleteDirectory(destinationDirectory);
			} catch { }
		}

		public static void DeleteDirectory(string directory)
		{
			try
			{
				if (!Directory.Exists(directory))
				{
					return;
				}
				string[] files = Directory.GetFiles(directory);
				if (files.Length > 0)
				{
					return;
				}

				Directory.Delete(directory);
				string baseDirectory = Path.GetDirectoryName(directory);
				DeleteDirectory(baseDirectory);
			} catch { }

		}

		public static void CopyFile(string sourceFileName, string destinationFileName)
		{
			string destinationDirectory = Path.GetDirectoryName(destinationFileName);
			CreateDirectory(destinationDirectory);
			if (FileExists(destinationFileName))
			{
				File.Delete(destinationFileName);
			}
			File.Copy(sourceFileName, destinationFileName);
		}

		public static void CreateDirectory(string directory)
		{
			if (Directory.Exists(directory))
			{
				return;
			}

			string baseDirectory = Path.GetDirectoryName(directory);
			CreateDirectory(baseDirectory);

			Directory.CreateDirectory(directory);
		}

		public static string CreateTempoaryDirectory()
		{
			string tempFileName = Path.GetTempFileName();
			if (File.Exists(tempFileName))
			{
				File.Delete(tempFileName);
			}
			string directory = Path.Combine(Path.GetTempPath(), tempFileName);
			Directory.CreateDirectory(directory);
			return directory;
		}

		public static string TempoaryOutputFileName(string extension)
		{
			string tempOutputFileName = Path.GetTempFileName();
			if (FileFunctions.FileExists(tempOutputFileName))
			{
				File.Delete(tempOutputFileName);
			}
			string outputFileName = tempOutputFileName + extension;
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			return outputFileName;
		}

	}
}
