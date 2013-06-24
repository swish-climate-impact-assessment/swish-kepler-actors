using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Swish.SimpleInstaller
{
	internal static class InstallFunctions
	{
		static InstallFunctions()
		{
			try { _symbols.Add(new Tuple<string, string>("%UserProfile%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))); } catch { }
			try { _symbols.Add(new Tuple<string, string>("%StartupPath%", Application.StartupPath)); } catch { }
			try { _symbols.Add(new Tuple<string, string>("%StartMenu%", Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms))); } catch { }
			try { _symbols.Add(new Tuple<string, string>("%SystemStartup%", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup))); } catch { }
			try { _symbols.Add(new Tuple<string, string>("%KeplerBin%", KeplerFunctions.BinDirectory)); } catch { }
			try { _symbols.Add(new Tuple<string, string>("%RBin%", RFunctions.BinDirectory)); } catch { }
			try { _symbols.Add(new Tuple<string, string>("%JavaBin%", JavaFunctions.BinDirectory)); } catch { }
		}

		internal static void Install(bool clean, ReportProgressFunction ReportMessage)
		{
			string fileName = Path.Combine(Application.StartupPath, "Install.txt");

			List<string> _pending = new List<string>(File.ReadLines(fileName));
			List<string> _completed = new List<string>();

			SwishFunctions._ReportMessage(ReportMessage, 0, "Installing...");
			SwishFunctions._ReportMessage(ReportMessage, -1, "");

			SwishFunctions._ReportMessage(ReportMessage, -1, "Symbols");
			for (int symbolIndex = 0; symbolIndex < _symbols.Count; symbolIndex++)
			{
				string symbol = _symbols[symbolIndex].Item1;
				string value = _symbols[symbolIndex].Item2;

				SwishFunctions._ReportMessage(ReportMessage, -1, "Symbol: " + symbol + " -> " + value);
			}
			SwishFunctions._ReportMessage(ReportMessage, -1, "");

			List<string> newPending = SwishFunctions.ConvertLines(_pending, _symbols, ReportMessage, false, true, true);
			_pending = newPending;

			while (_pending.Count > 0)
			{
				string line;
				if (!clean)
				{
					line = _pending[0];
					_pending.RemoveAt(0);
				} else
				{
					line = _pending[_pending.Count - 1];
					_pending.RemoveAt(_pending.Count - 1);
				}
				_completed.Add(line);

				SwishFunctions._ReportMessage(ReportMessage, -1, "Run line: \"" + line + "\"");

				RunLine(line, clean, ReportMessage);

				SwishFunctions._ReportMessage(ReportMessage, 100 * (_completed.Count) / (_completed.Count + _pending.Count), string.Empty);
			}
			SwishFunctions._ReportMessage(ReportMessage, -1, "");
			SwishFunctions._ReportMessage(ReportMessage, 100, "Done.");
		}

		internal static void RunLine(string line, bool clean, ReportProgressFunction ReportMessage)
		{
			SwishFunctions._ReportMessage(ReportMessage, -1, line);

			string whiteSpace;
			StringIO.SkipWhiteSpace(out whiteSpace, ref line);
			if (StringIO.TryRead("DeleteFilesAndDirectories", ref line))
			{
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				string directory;
				if (!StringIO.TryReadString(out directory, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				FileFunctions.DeleteDirectoryAndContents(directory, ReportMessage);
			} else if (StringIO.TryRead("Delete", ref line))
			{
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				string fileName;
				if (!StringIO.TryReadString(out fileName, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				FileFunctions.DeleteFile(fileName, ReportMessage);
			} else if (StringIO.TryRead("DateAfter", ref line))
			{
				DateTime date = ReadDate(ref line);
				if (DateTime.Now.Date > date)
				{
					RunLine(line, clean, null);
				}
			} else if (StringIO.TryRead("DateBefore", ref line))
			{
				DateTime date = ReadDate(ref line);
				if (DateTime.Now.Date < date)
				{
					RunLine(line, clean, null);
				}
			} else if (StringIO.TryRead("RunProcess", ref line))
			{
				string process;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out process, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}
				string arguments = line;

				using (Process.Start(process, arguments)) { }
			} else if (StringIO.TryRead("CopyFilesAndDirectories", ref line))
			{
				string sourceDirectory;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out sourceDirectory, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				string destinationDirectory;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out destinationDirectory, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				if (!Directory.Exists(sourceDirectory))
				{
					throw new Exception("could not find directory: \"" + sourceDirectory + "\"");
				}

				CopyDirectory(sourceDirectory, destinationDirectory, clean, ReportMessage);
			} else if (StringIO.TryRead("CopyFiles", ref line))
			{
				string sourceDirectory;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out sourceDirectory, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				string destinationDirectory;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out destinationDirectory, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				if (!Directory.Exists(sourceDirectory))
				{
					throw new Exception("could not find directory: \"" + sourceDirectory + "\"");
				}

				CopyFiles(sourceDirectory, destinationDirectory, clean, ReportMessage);
			} else if (StringIO.TryRead("Copy", ref line))
			{
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);

				string sourceFileName;
				if (!StringIO.TryReadString(out sourceFileName, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				string destinationFileName;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out destinationFileName, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				if (!clean)
				{
					FileFunctions.CopyFile(sourceFileName, destinationFileName, ReportMessage);
				} else
				{
					FileFunctions.DeleteFile(destinationFileName, ReportMessage);
				}
			} else if (StringIO.TryRead("MakeShortCut", ref line))
			{
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);

				string fileName;
				if (!StringIO.TryReadString(out fileName, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				string targetFileName;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out targetFileName, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				string arguments;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out arguments, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				string description;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out description, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				if (!clean)
				{
					string destinationDirectory = Path.GetDirectoryName(fileName);

					FileFunctions.CreateDirectory(destinationDirectory, ReportMessage);
					string icon = KeplerFunctions.ExecutablePath + ",0";
					Shortcut.Create(fileName, targetFileName, arguments, description, icon);
				} else
				{
					FileFunctions.DeleteFile(fileName, ReportMessage);
				}
			} else if (StringIO.TryRead("AddPath", ref line))
			{
				string binPath;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out binPath, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				if (!clean)
				{
					string keySystemPath = @"HKEY_CURRENT_USER\Environment";
					string keyUserPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

					try
					{
						AddPath(keySystemPath, binPath);
						return;
					} catch (Exception error)
					{
						ReportMessage(-1, "Failed add to system path" + Environment.NewLine + ExceptionFunctions.Write(error, false));
					}

					AddPath(keyUserPath, binPath);
				}
			} else if (StringIO.TryRead("AddRegistryKey", ref line))
			{
				string path;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out path, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				string key;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out key, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				string value;
				StringIO.SkipWhiteSpace(out whiteSpace, ref line);
				if (!StringIO.TryReadString(out value, ref line))
				{
					throw new Exception("could not read line \"" + line + "\"");
				}

				if (!clean)
				{
					Registry.SetValue(path, key, value, RegistryValueKind.String);
				} else
				{
					Registry.SetValue(path, key, null, RegistryValueKind.String);
				}
			} else if (string.IsNullOrWhiteSpace(line) || StringIO.TryRead("//", ref line))
			{
				// do nothing
			} else
			{
				throw new Exception("could not read line \"" + line + "\"");
			}
		}

		private static DateTime ReadDate(ref string line)
		{
			int year;
			int month;
			int day;
			string whiteSpace;
			StringIO.SkipWhiteSpace(out whiteSpace, ref line);
			if (!StringIO.TryRead(out year, ref line))
			{
				throw new Exception("could not read line \"" + line + "\"");
			}

			StringIO.SkipWhiteSpace(out whiteSpace, ref line);
			if (!StringIO.TryRead(out month, ref line))
			{
				throw new Exception("could not read line \"" + line + "\"");
			}

			StringIO.SkipWhiteSpace(out whiteSpace, ref line);
			if (!StringIO.TryRead(out day, ref line))
			{
				throw new Exception("could not read line \"" + line + "\"");
			}

			DateTime date = new DateTime(year, month, day);
			return date;
		}

		private static void AddPath(string keyPath, string binPath)
		{
			string keyName = "Path";
			string path = (string)Registry.GetValue(keyPath, keyName, null);
			List<string> paths = new List<string>(path.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

			binPath = Path.GetFullPath(binPath);
			if (!binPath.EndsWith("\\"))
			{
				binPath += "\\";
			}
			if (paths.Contains(binPath))
			{
				return;
			}

			paths.Add(binPath);
			path = string.Join(";", paths) + ";";
			Registry.SetValue(keyPath, keyName, path, RegistryValueKind.String);
		}

		private static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool clean, ReportProgressFunction ReportMessage)
		{
			CopyFiles(sourceDirectory, destinationDirectory, clean, ReportMessage);

			string[] directories = Directory.GetDirectories(sourceDirectory);
			for (int fileIndex = 0; fileIndex < directories.Length; fileIndex++)
			{
				string sourceDirectoryName = directories[fileIndex];

				string name = Path.GetFileName(sourceDirectoryName);
				string destinationDirectoryName = Path.Combine(destinationDirectory, name);

				CopyDirectory(sourceDirectoryName, destinationDirectoryName, clean, ReportMessage);
			}
		}

		private static void CopyFiles(string sourceDirectory, string destinationDirectory, bool clean, ReportProgressFunction ReportMessage)
		{
			string[] files = Directory.GetFiles(sourceDirectory, "*");
			for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
			{
				string sourceFileName = files[fileIndex];
				string fileName = Path.GetFileName(sourceFileName);
				string destinationFileName = Path.Combine(destinationDirectory, fileName);

				if (!clean)
				{
					FileFunctions.CopyFile(sourceFileName, destinationFileName, ReportMessage);
				} else
				{
					FileFunctions.DeleteFile(destinationFileName, ReportMessage);
				}
			}
		}

		private static List<Tuple<string, string>> _symbols = new List<Tuple<string, string>>();

		public static void SplitArguments(out string fileName, out string arguments, string command)
		{
			command = command.Trim();
			string stopString;
			if (StringIO.TryReadString(out fileName, ref command))
			{

			} else if (StringIO.TryReadUntill(out fileName, out stopString, new string[] { " " }, ref command))
			{

			} else
			{
				fileName = command;
				command = string.Empty;
			}

			command = command.Trim();
			arguments = command;
		}

	}
}

