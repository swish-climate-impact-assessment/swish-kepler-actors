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

		public static void DeleteFile(string fileName, ReportProgressFunction ReportProgress)
		{
			try
			{
				if (!FileExists(fileName))
				{
					return;
				}
				File.SetAttributes(fileName, FileAttributes.Normal);
				if (ReportProgress != null)
				{
					ReportProgress(-1, "delete file: " + fileName);
				}
				File.Delete(fileName);
				string destinationDirectory = Path.GetDirectoryName(fileName);
				DeleteDirectory(destinationDirectory, ReportProgress);
			} catch { }
		}

		public static void DeleteDirectory(string directory, ReportProgressFunction ReportProgress)
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

				if (ReportProgress != null)
				{
					ReportProgress(-1, "delete directory: " + directory);
				}
				Directory.Delete(directory);
				string baseDirectory = Path.GetDirectoryName(directory);
				DeleteDirectory(baseDirectory, ReportProgress);
			} catch { }

		}

		public static void CopyFile(string sourceFileName, string destinationFileName, ReportProgressFunction ReportProgress)
		{
			try
			{
				string destinationDirectory = Path.GetDirectoryName(destinationFileName);
				CreateDirectory(destinationDirectory, ReportProgress);
				if (FileExists(destinationFileName))
				{
					if (ReportProgress != null)
					{
						ReportProgress(-1, "delete: " + destinationFileName);
					}
					File.Delete(destinationFileName);
				}

				if (ReportProgress != null)
				{
					ReportProgress(-1, "copy: " + sourceFileName + " -> " + destinationFileName);
				}
				File.Copy(sourceFileName, destinationFileName);
			} catch (Exception error)
			{
				string errorMessage = "Failed copying file \"" + sourceFileName + "\" -> \"" + destinationFileName + "\"";
				if (ReportProgress != null)
				{
					ReportProgress(-1, errorMessage);
				}
				throw new Exception(errorMessage, error);
			}
		}

		public static void CreateDirectory(string directory, ReportProgressFunction ReportProgress)
		{
			try
			{
				if (Directory.Exists(directory))
				{
					return;
				}

				string baseDirectory = Path.GetDirectoryName(directory);
				CreateDirectory(baseDirectory, ReportProgress);

				if (ReportProgress != null)
				{
					ReportProgress(-1, "Create directory: " + directory);
				}
				Directory.CreateDirectory(directory);
			} catch (Exception error)
			{
				string errorMessage = "Failed create directory \"" + directory + "\"";
				if (ReportProgress != null)
				{
					ReportProgress(-1, errorMessage);
				}
				throw new Exception(errorMessage, error);
			}
		}

		private static string _tempoaryFileBase = string.Empty;
		private static int _tempoaryFileCount = 0;
		private static string TempoaryFileName()
		{
			if (string.IsNullOrWhiteSpace(_tempoaryFileBase))
			{
				_tempoaryFileBase = Path.GetTempFileName();
				if (File.Exists(_tempoaryFileBase))
				{
					File.Delete(_tempoaryFileBase);
				}
			}
			_tempoaryFileCount++;
			return _tempoaryFileBase + _tempoaryFileCount;
		}

		public static string CreateTempoaryDirectory()
		{
			string tempFileName = TempoaryFileName();
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
			string tempOutputFileName = TempoaryFileName();
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
