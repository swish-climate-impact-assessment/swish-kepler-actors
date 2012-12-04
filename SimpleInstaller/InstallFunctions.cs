using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Swish.SimpleInstaller
{
	internal static class InstallFunctions
	{
		internal delegate void ReportProgressFunction(int progress, string line);

		internal static void Install(bool clean, ReportProgressFunction ReportProgress)
		{
			string fileName = Path.Combine(Application.StartupPath, "Install.txt");

			List<string> _pending = new List<string>(File.ReadLines(fileName));
			List<string> _completed = new List<string>();

			if (ReportProgress != null)
			{
				ReportProgress(0, "");
			}

			while (_pending.Count > 0)
			{
				string line;
				if (clean)
				{
					line = _pending[0];
					_pending.RemoveAt(0);
				} else
				{
					line = _pending[_pending.Count - 1];
					_pending.RemoveAt(_pending.Count - 1);
				}
				_completed.Add(line);

				RunLine(line, clean);

				if (ReportProgress != null)
				{
					ReportProgress(100 * (_completed.Count) / (_completed.Count + _pending.Count), line);
				}
			}
		}

		internal static void RunLine(string _line, bool clean)
		{
			string line = _line;

			line = line.Replace("%UserProfile%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			line = line.Replace("%StartupPath%", Application.StartupPath);

			string whiteSpace;
			StringIO.SkipWhiteSpace(out whiteSpace, ref line);
			if (StringIO.TryRead("CopyFiles", ref line))
			{
				string sourceDirectory;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out sourceDirectory, ref line))
				{
					throw new Exception("could not read line \"" + _line + "\"");
				}

				string destinationDirectory;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out destinationDirectory, ref line))
				{
					throw new Exception("could not read line \"" + _line + "\"");
				}

				string[] files = Directory.GetFiles(sourceDirectory, "*");

				for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
				{
					string sourceFileName = files[fileIndex];
					string fileName = Path.GetFileName(sourceFileName);
					string destinationFileName = Path.Combine(destinationDirectory, fileName);

					if (!clean)
					{
						FileFunctions.CopyFile(sourceFileName, destinationFileName);
					} else
					{
						FileFunctions.DeleteFile(destinationFileName);
					}
				}
			} else if (StringIO.TryRead("Copy", ref line))
			{
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);

				string sourceFileName;
				if (!StringIO.TryReadString(out sourceFileName, ref line))
				{
					throw new Exception("could not read line \"" + _line + "\"");
				}

				string destinationFileName;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out destinationFileName, ref line))
				{
					throw new Exception("could not read line \"" + _line + "\"");
				}

				if (!clean)
				{
					FileFunctions.CopyFile(sourceFileName, destinationFileName);
				} else
				{
					FileFunctions.DeleteFile(destinationFileName);
				}
			} else if (string.IsNullOrWhiteSpace(line) || StringIO.TryRead("//", ref line))
			{
				// do nothing
			} else
			{
				throw new Exception("could not read line \"" + _line + "\"");
			}
		}

	}
}
