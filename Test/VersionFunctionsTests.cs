using System;
using System.IO;
using System.Windows.Forms;
using Swish.Server;

namespace Swish.Tests
{
	public class VersionFunctionsTests
	{
		public void Test()
		{

			string directory = Path.GetDirectoryName(Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar));
			directory = Path.Combine(directory, "Version9999");

			string fileName = Path.Combine(directory, Path.GetFileName(Application.ExecutablePath));

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			if (Directory.Exists(directory))
			{
				Directory.Delete(directory);
			}

			try
			{
				//if (!string.IsNullOrWhiteSpace(VersionFunctions.BestExecutable))
				//{
				//    throw new Exception("BestExecutable");
				//}

				if (VersionFunctions.CurrentDirectory != Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar))
				{
					throw new Exception("CurrentDirectory: \"" + VersionFunctions.CurrentDirectory + "\" Expected: \"" + Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar) + "\"");
				}

				//if (VersionFunctions.NextDirectory != Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar) + "0000")
				//{
				//    throw new Exception("NextDirectory");
				//}

				//if (VersionFunctions.NextExecutable != Path.Combine(Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar) + "0000",
				//    Path.GetFileName(Application.ExecutablePath)))
				//{
				//    throw new Exception("NextExecutable");
				//}

				///////////////////


				Directory.CreateDirectory(directory);
				File.WriteAllText(fileName, "");


				if (VersionFunctions.BestExecutable != fileName)
				{
					throw new Exception("BestExecutable2");
				}

				if (VersionFunctions.CurrentDirectory != Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar))
				{
					throw new Exception("CurrentDirectory2");
				}

				//if (VersionFunctions.NextDirectory != Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar) + "0000")
				//{
				//    throw new Exception("NextDirectory2");
				//}

				//if (VersionFunctions.NextExecutable != Path.Combine(Application.StartupPath.TrimEnd(Path.DirectorySeparatorChar) + "0000",
				//    Path.GetFileName(Application.ExecutablePath)))
				//{
				//    throw new Exception("NextExecutable2");
				//}
			} finally
			{
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				if (Directory.Exists(directory))
				{
					Directory.Delete(directory);
				}
			}
		}
	}
}
