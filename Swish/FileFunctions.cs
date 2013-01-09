using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;

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
				throw new Exception("Could not find location of \"" + fileName + "\"" + Environment.NewLine + "Paths searched:" + Environment.NewLine + string.Join(Environment.NewLine, attempts) + "End paths searched:" + Environment.NewLine);
			}

			return string.Empty;
		}

		public static string AdjustFileName(string fileName)
		{
			bool monoEnvironment = !ProcessFunctions.MonoEnvironment;
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

		public static void DeleteFile(string fileName, ReportProgressFunction ReportMessage)
		{
			try
			{
				if (!FileExists(fileName))
				{
					return;
				}
				File.SetAttributes(fileName, FileAttributes.Normal);
				_ReportMessage(ReportMessage, -1, "delete file: " + fileName);
				File.Delete(fileName);
				string destinationDirectory = Path.GetDirectoryName(fileName);
				DeleteDirectory(destinationDirectory, ReportMessage);
			} catch { }
		}

		public static void DeleteDirectory(string directory, ReportProgressFunction ReportMessage)
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

				_ReportMessage(ReportMessage, -1, "delete directory: " + directory);
				Directory.Delete(directory);
				string baseDirectory = Path.GetDirectoryName(directory);
				DeleteDirectory(baseDirectory, ReportMessage);
			} catch { }

		}

		public static void CopyFile(string sourceFileName, string destinationFileName, ReportProgressFunction ReportMessage)
		{
			try
			{
				string destinationDirectory = Path.GetDirectoryName(destinationFileName);
				CreateDirectory(destinationDirectory, ReportMessage);
				if (FileExists(destinationFileName))
				{
					_ReportMessage(ReportMessage, -1, "delete: " + destinationFileName);
					File.Delete(destinationFileName);
				}

				_ReportMessage(ReportMessage, -1, "copy: " + sourceFileName + " -> " + destinationFileName);
				File.Copy(sourceFileName, destinationFileName);
			} catch (Exception error)
			{
				string errorMessage = "Failed copying file \"" + sourceFileName + "\" -> \"" + destinationFileName + "\"";
				_ReportMessage(ReportMessage, -1, errorMessage);
				throw new Exception(errorMessage, error);
			}
		}

		public static void CreateDirectory(string directory, ReportProgressFunction ReportMessage)
		{
			try
			{
				if (Directory.Exists(directory))
				{
					return;
				}

				string baseDirectory = Path.GetDirectoryName(directory);
				CreateDirectory(baseDirectory, ReportMessage);

				_ReportMessage(ReportMessage, -1, "Create directory: " + directory);
				Directory.CreateDirectory(directory);
			} catch (Exception error)
			{
				string errorMessage = "Failed create directory \"" + directory + "\"";
				_ReportMessage(ReportMessage, -1, errorMessage);
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

		public static string TempoaryDirectory()
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

		private static void _ReportMessage(ReportProgressFunction ReportMessage, int progress, string message)
		{
			if (ReportMessage != null)
			{
				ReportMessage(progress, message);
			} else if (progress >= 0)
			{
				Console.WriteLine(progress + "%" + message);
			} else if (ExceptionFunctions.ForceVerbose)
			{
				Console.WriteLine(message);
			}
		}

		public static void GetFilesAndDirectories(out List<string> directoryList, out List<string> fileList, string directory, List<string> excludeDirectories)
		{
			directoryList = new List<string>();
			fileList = new List<string>();

			directoryList.Add(directory);
			for (int index = 0; index < directoryList.Count; index++)
			{
				string directoryName = directoryList[index];
				string name = Path.GetFileName(directoryName).ToLower();
				if (excludeDirectories != null && excludeDirectories.Contains(name))
				{
					continue;
				}

				try
				{
					directoryList.AddRange(Directory.GetDirectories(directoryName));
					fileList.AddRange(Directory.GetFiles(directoryName));
				} catch (Exception error)
				{
					if (ExceptionFunctions.ForceVerbose)
					{
						Console.Write(ExceptionFunctions.Write(error, true));
					}
				}
			}
		}

		public static string GetRelativePath(string destinationPath, string sourceDirectory)
		{
			//string fullDestination = Path.GetFullPath(destinationPath);
			//string fullSource = Path.GetFullPath(sourceDirectory);

			//List<string> destinationFragments = GetPathFragments(fullDestination);
			//List<string> sourceFragments = GetPathFragments(fullSource);

			List<string> destinationFragments = GetPathFragments(destinationPath);
			List<string> sourceFragments = GetPathFragments(sourceDirectory);

			if (destinationFragments.Count == 0 || sourceFragments.Count == 0 || destinationFragments[0].ToLower() != sourceFragments[0].ToLower())
			{
				return destinationPath;
			}

			string FileExtensionSeparatorChar = ".";
			if (destinationPath.ToLower() == sourceDirectory.ToLower())
			{
				string fileNameFragment = destinationFragments[destinationFragments.Count - 1];
				if (File.Exists(destinationPath) || fileNameFragment.Contains(FileExtensionSeparatorChar))
				{
					return fileNameFragment;
				}
			}

			int index;
			for (index = 0; index < destinationFragments.Count && index < sourceFragments.Count; index++)
			{
				if (string.Compare(destinationFragments[index], sourceFragments[index], true) != 0)
				{
					break;
				}
			}

			List<string> fragments = new List<string>();
			string previousDirectorySymbol = "..";

			for (int sourceIndex = index; sourceIndex < sourceFragments.Count; sourceIndex++)
			{
				fragments.Add(previousDirectorySymbol);
			}

			for (int destinationIndex = index; destinationIndex < destinationFragments.Count; destinationIndex++)
			{
				fragments.Add(destinationFragments[destinationIndex]);
			}

			string result = Path.Combine(fragments.ToArray());

			return result;
		}

		internal static List<string> GetPathFragments(string path)
		{
			if (path == Path.DirectorySeparatorChar.ToString() || path == Path.AltDirectorySeparatorChar.ToString())
			{
				return new List<string>();
			}

			bool rooted = Path.IsPathRooted(path);
			string root;
			if (rooted)
			{
				root = Path.GetPathRoot(path);

				if (path.ToLower() == root.ToLower())
				{
					return new List<string>() { root };
				}
			} else
			{
				root = string.Empty;
			}

			if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) || path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
			{
				path = Path.GetDirectoryName(path);
			}

			List<string> fragments = new List<string>();
			while (true)
			{
				if (string.IsNullOrEmpty(path))
				{
					break;
				}
				if (rooted && path.ToLower() == root.ToLower())
				{
					fragments.Add(root);
					break;
				}

				string fragment = Path.GetFileName(path);
				fragments.Add(fragment);
				path = Path.GetDirectoryName(path);
			}
			fragments.Reverse();
			return fragments;
		}

		public static byte[] GetMd5(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			using (BufferedStream input = new BufferedStream(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
			using (MD5CryptoServiceProvider encoder = new MD5CryptoServiceProvider())
			{
				return encoder.ComputeHash(input);
			}
		}

	}
}
