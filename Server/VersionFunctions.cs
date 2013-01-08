using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Swish.Server
{
	public static class VersionFunctions
	{
		private static string Version(string versionBase, int versionNumber)
		{
			if (!versionBase.Contains("" + Path.DirectorySeparatorChar))
			{
				versionBase = Path.Combine(CommonDirectory, versionBase);
			}
			string nextDirectory = versionBase + versionNumber.ToString("d4");
			return nextDirectory;
		}

		private static void Information(string versionDirectory, out string directoryBase, out int number)
		{
			if (versionDirectory.Contains("" + Path.DirectorySeparatorChar))
			{
				versionDirectory = Path.GetFileName(versionDirectory);
			}

			int stringIndex = versionDirectory.Length;
			while (stringIndex > 0 && char.IsDigit(versionDirectory[stringIndex - 1]))
			{
				stringIndex--;
			}

			if (stringIndex < versionDirectory.Length)
			{
				string versionString = versionDirectory.Substring(stringIndex);
				number = int.Parse(versionString);
			} else
			{
				number = -1;
			}

			directoryBase = versionDirectory.Substring(0, stringIndex);
			directoryBase = Path.Combine(CommonDirectory, directoryBase);
		}

		private static string CommonDirectory
		{
			get
			{
				string commonDirectory = Path.GetDirectoryName(Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar));
				return commonDirectory;
			}
		}

		private static string ExecutableFileName
		{
			get
			{
				string executableName = Path.GetFileName(Application.ExecutablePath);
				return executableName;
			}
		}

		public static string CurrentDirectory
		{
			get
			{
				return Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar);
				//string startupPath = Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar);
				//string currentDirectory = Path.GetFileName(startupPath);
				//return currentDirectory;
			}
		}

		public static string NextDirectory
		{
			get
			{
				int versionNumber;
				string versionBase;
				Information(CurrentDirectory, out versionBase, out versionNumber);
				versionNumber++;
				string nextDirectory = Version(versionBase, versionNumber);

				return nextDirectory;
			}
		}

		public static string NextExecutable
		{
			get
			{
				string nextExecutable = Path.Combine(NextDirectory, ExecutableFileName);
				return nextExecutable;
			}
		}

		private static string BestDirectory
		{
			get
			{
				List<string> directories = new List<string>(Directory.GetDirectories(CommonDirectory));

				int maxNumber = -1;
				string maxVersion = string.Empty;
				for (int directoryIndex = 0; directoryIndex < directories.Count; directoryIndex++)
				{
					string directory = directories[directoryIndex];

					string versionBase;
					int number;
					Information(directory, out versionBase, out number);

					if (number > maxNumber)
					{
						maxNumber = number;
						maxVersion = directory;
					}
				}
				return maxVersion;
			}
		}

		public static string BestExecutable
		{
			get
			{
				string bestExecutable = BestDirectory;
				if (string.IsNullOrWhiteSpace(bestExecutable))
				{
					return string.Empty;
				}
				bestExecutable = Path.Combine(bestExecutable, ExecutableFileName);
				return bestExecutable;
			}
		}

	}
}
