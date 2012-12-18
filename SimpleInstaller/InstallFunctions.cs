using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Swish.SimpleInstaller
{
	internal static class InstallFunctions
	{
		static InstallFunctions()
		{
			_symbols.Add(new Tuple<string, string>("%UserProfile%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
			_symbols.Add(new Tuple<string, string>("%StartupPath%", Application.StartupPath));
		}

		internal static void Install(bool clean, ReportProgressFunction ReportProgress)
		{
			string fileName = Path.Combine(Application.StartupPath, "Install.txt");

			List<string> _pending = new List<string>(File.ReadLines(fileName));
			List<string> _completed = new List<string>();

			if (ReportProgress != null)
			{
				ReportProgress(0, "Installing...");

				for (int symbolIndex = 0; symbolIndex < _symbols.Count; symbolIndex++)
				{
					string symbol = _symbols[symbolIndex].Item1;
					string value = _symbols[symbolIndex].Item2;

					ReportProgress(-1, "Symbol: " + symbol + " -> " + value);
				}
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

				RunLine(line, clean, ReportProgress);

				if (ReportProgress != null)
				{
					ReportProgress(100 * (_completed.Count) / (_completed.Count + _pending.Count), string.Empty);
				}
			}
			if (ReportProgress != null)
			{
				ReportProgress(100, "Done.");
			}

		}

		internal static void RunLine(string _line, bool clean, ReportProgressFunction ReportProgress)
		{
			if (ReportProgress != null)
			{
				ReportProgress(-1, _line);
			}

			string line = ResloveSymbols(_line);

			if (line != _line)
			{
				if (ReportProgress != null)
				{
					ReportProgress(-1, "Changed to: " + line);
				}
			}

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
						FileFunctions.CopyFile(sourceFileName, destinationFileName, ReportProgress);
					} else
					{
						FileFunctions.DeleteFile(destinationFileName, ReportProgress);
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
					FileFunctions.CopyFile(sourceFileName, destinationFileName, ReportProgress);
				} else
				{
					FileFunctions.DeleteFile(destinationFileName, ReportProgress);
				}
			} else if (string.IsNullOrWhiteSpace(line) || StringIO.TryRead("//", ref line))
			{
				// do nothing
			} else
			{
				throw new Exception("could not read line \"" + _line + "\"");
			}
		}

		private static List<Tuple<string, string>> _symbols = new List<Tuple<string, string>>();

		private static string ResloveSymbols(string line)
		{
			for (int symbolIndex = 0; symbolIndex < _symbols.Count; symbolIndex++)
			{
				string symbol = _symbols[symbolIndex].Item1;
				string value = StringIO.Escape(_symbols[symbolIndex].Item2);
				line = line.Replace(symbol, value);
			}

			return line;
		}

	}
}
