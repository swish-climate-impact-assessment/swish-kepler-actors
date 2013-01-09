using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Swish
{
	public static class ProcessFunctions
	{
		public static string WriteProcessInformation(Process process)
		{
			string message = string.Empty;
			try { string value = process.Id.ToString(); if (!string.IsNullOrWhiteSpace(value)) { message += "Process id: " + value + Environment.NewLine; } } catch { }
			try { string value = process.ProcessName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tProcess name: " + value + Environment.NewLine; } } catch { }
			try { string value = process.StartTime.ToShortTimeString(); if (!string.IsNullOrWhiteSpace(value)) { message += "\tStart time: " + value + Environment.NewLine; } } catch { }
			try { string value = process.StartInfo.FileName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tFile name: " + value + Environment.NewLine; } } catch { }
			try { string value = process.StartInfo.Arguments; if (!string.IsNullOrWhiteSpace(value)) { message += "\tArguments: " + value + Environment.NewLine; } } catch { }
			try { string value = process.StartInfo.WorkingDirectory; if (!string.IsNullOrWhiteSpace(value)) { message += "\tWorking directory: " + value + Environment.NewLine; } } catch { }

#if !MONO
			try { string value = process.MachineName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tMachine name: " + value + Environment.NewLine; } } catch { }
			try { string value = process.SessionId.ToString(); if (!string.IsNullOrWhiteSpace(value)) { message += "\tSession id: " + value + Environment.NewLine; } } catch { }
#endif

			try
			{
				ProcessModule module = process.MainModule;
				try { string value = module.FileName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tFile name: " + value + Environment.NewLine; } } catch { }
				try { string value = module.ModuleName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tModule name: " + value + Environment.NewLine; } } catch { }
				try { string value = module.FileVersionInfo.FileName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tFile name: " + value + Environment.NewLine; } } catch { }
			} catch { }

			return message;
		}

		public static string WriteProcessHeritage()
		{
			string message = string.Empty;
			Process process = Process.GetCurrentProcess();
			while (process != null)
			{
				message += WriteProcessInformation(process);
				process = ParentProcess(process);
			}
			return message;
		}

		public static string WriteSystemVariables()
		{
			//if (DateTime.MinValue < DateTime.MaxValue)
			//{
			//    return "";
			//}

			List<string> lines = new List<string>();

			lines.Add("Paths: ");
			List<string> paths = new List<string>();
			paths.Add("~");
			paths.Add("\\");
			paths.Add("/");
			//paths.Add("\\\\");
			//paths.Add("//");
			paths.Add("..\\");
			paths.Add("../");
			paths.Add(".\\");
			paths.Add("./");

			for (int index = 0; index < paths.Count; index++)
			{
				try
				{
					lines.Add("\"" + paths[index] + "\" -> \"" + Path.GetFullPath(paths[index]) + "\"");
				} catch
				{
					lines.Add("\"" + paths[index] + "\" Failed");
				}
			}

			lines.Add("System directory: " + Environment.SystemDirectory);
			lines.Add("Logical drives: ");
			string[] drives = Environment.GetLogicalDrives();
			for (int index = 0; index < drives.Length; index++)
			{
				lines.Add(index.ToString("d2") + ": " + drives[index]);
			}

			lines.Add("Special folders: ");
			string[] specialFolderNames = Enum.GetNames(typeof(Environment.SpecialFolder));
			string[] specialFolderOptionNames = Enum.GetNames(typeof(Environment.SpecialFolderOption));
			for (int index1 = 0; index1 < specialFolderOptionNames.Length; index1++)
			{
				lines.Add("Special folders option: " + specialFolderOptionNames[index1]);
				Environment.SpecialFolderOption option = (Environment.SpecialFolderOption)Enum.Parse(typeof(Environment.SpecialFolderOption), specialFolderOptionNames[index1], false);
				for (int index2 = 0; index2 < specialFolderNames.Length; index2++)
				{
					Environment.SpecialFolder folder = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), specialFolderNames[index2], false);
#if !MONO
					try { string value = Environment.GetFolderPath(folder, option); if (!string.IsNullOrWhiteSpace(value)) { string line = specialFolderNames[index2] + ": " + value; lines.Add(line); } } catch { }
#endif
				}
			}

			lines.Add("Environment variables: ");
			string[] environmentVariableTargetNames = Enum.GetNames(typeof(EnvironmentVariableTarget));
			for (int index1 = 0; index1 < environmentVariableTargetNames.Length; index1++)
			{
				lines.Add("Environment target: " + environmentVariableTargetNames[index1]);
				EnvironmentVariableTarget target = (EnvironmentVariableTarget)Enum.Parse(typeof(EnvironmentVariableTarget), environmentVariableTargetNames[index1], false);
				System.Collections.IDictionary variables = Environment.GetEnvironmentVariables(target);
				foreach (string key in variables.Keys)
				{
					string value = (string)variables[key];
					lines.Add(key + ": " + value);
				}
			}

			string message = string.Join(Environment.NewLine, lines);

			return message;
		}

		public static Process ParentProcess(Process process)
		{
			if (process == null)
			{
				throw new ArgumentNullException("process");
			}

			if (string.IsNullOrWhiteSpace(process.ProcessName))
			{
				return null;
			}

			string indexdProcessName = string.Empty;
			int processCount = Process.GetProcessesByName(process.ProcessName).Length;
			for (int index = 0; index < processCount; index++)
			{
				string indexdName;
				if (index == 0)
				{
					indexdName = process.ProcessName;
				} else
				{
					indexdName = process.ProcessName + "#" + index;
				}

				PerformanceCounter processAccess = new PerformanceCounter("Process", "ID Process", indexdName);
				int id = (int)processAccess.NextValue();
				if (id == process.Id)
				{
					indexdProcessName = indexdName;
					break;
				}
			}

			if (string.IsNullOrWhiteSpace(indexdProcessName))
			{
				return null;
			}

			PerformanceCounter parentAccess;
			try
			{
				parentAccess = new PerformanceCounter("Process", "Creating Process ID", indexdProcessName);
			} catch
			{
				return null;
			}
			Process parent;
			try
			{
				int parentId = (int)parentAccess.NextValue();
				parent = Process.GetProcessById(parentId);
			} catch
			{
				return null;
			}
			return parent;
		}

		public static ProcessResult Run(string executableName, string arguments, string workingDirectory, bool returnBeforeExit, TimeSpan timeout, bool console, bool readOutput, bool dotNet)
		{
			if (console && readOutput)
			{
				throw new Exception("Cannot read console output");
			}

			ProcessResult result = new ProcessResult();
			using (Process process = new Process())
			{
				if (!dotNet || !MonoEnvironment)
				{
					process.StartInfo.FileName = executableName;
					if (!string.IsNullOrWhiteSpace(arguments))
					{
						process.StartInfo.Arguments = arguments;
					}
				} else
				{
					process.StartInfo.FileName = "mono";
					if (!string.IsNullOrWhiteSpace(arguments))
					{
						process.StartInfo.Arguments = executableName + " " + arguments;
					} else
					{
						process.StartInfo.Arguments = executableName;
					}
				}

				if (!string.IsNullOrWhiteSpace(workingDirectory))
				{
					process.StartInfo.WorkingDirectory = workingDirectory;
				}

				if (returnBeforeExit)
				{
					if (console)
					{
						process.StartInfo.UseShellExecute = true;
					} else
					{
						process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						process.StartInfo.UseShellExecute = false;
					}
					process.Start();
					//WaitProcessActive(process);

					process.Close();
					return result;
				}

				if (!console)
				{
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.StartInfo.UseShellExecute = false;
					if (readOutput)
					{
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.RedirectStandardError = true;
						process.OutputDataReceived += result.OutputDataReceived;
						process.ErrorDataReceived += result.ErrorDataReceived;
					}
				} else
				{
					process.StartInfo.UseShellExecute = true;
				}
				process.Start();
				if (readOutput)
				{
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
				}

				bool exited = WaitProcessEnd(process, timeout);
				if (!exited)
				{
					process.Kill();
					process.WaitForExit();
					process.Close();
					throw new Exception("Process timmed out");
				}

				result.ExitCode = process.ExitCode;
				//if (readOutput)
				//{
				//    result.Output = process.StandardError.ReadToEnd();
				//    result.Output += process.StandardOutput.ReadToEnd();
				//} else
				//{
				//    result.Output = string.Empty;
				//}
				process.Close();
			}
			return result;
		}

		public static bool WaitProcessEnd(Process run, TimeSpan timeout)
		{
			if (timeout != null && timeout > TimeSpan.Zero)
			{
				run.WaitForExit((int)timeout.TotalMilliseconds);
				if (!run.HasExited)
				{
					return false;
				}
			} else
			{
				run.WaitForExit(Int32.MaxValue - 1);
			}
			return true;
		}

		private static bool _environmentDetected;
		private static bool _monoEnvironment;

		public static bool MonoEnvironment
		{
			get
			{
				bool flag = _environmentDetected;
				if (!flag)
				{
					Type mono = Type.GetType("Mono.Runtime");
					_monoEnvironment = !EqualFunctions.Equal(mono, null);
					_environmentDetected = true;
				}
				bool flag1 = _monoEnvironment;
				return flag1;
			}
		}

		private static Process _keplerProcess;
		public static Process KeplerProcess
		{
			get
			{
				if (_keplerProcess == null)
				{
					Process current = Process.GetCurrentProcess();
					while (true)
					{
						Process parent = ParentProcess(current);
						if (parent == null)
						{
							break;
						}
						string parentName = parent.ProcessName.ToLower();
						if (parentName.Contains("java") || parentName.Contains("kepler") || parentName.Contains("vcsexpress"))
						{
							_keplerProcess = parent;
							break;
						}
						current = parent;
					}
				}
				return _keplerProcess;
			}
		}

		public static string TryGetFileName(Process process)
		{
			try { string value = process.ProcessName; if (File.Exists(value)) { return value; } } catch { }
			try { string value = process.StartInfo.FileName; if (File.Exists(value)) { return value; } } catch { }

			try
			{
				ProcessModule module = process.MainModule;
				try { string value = module.FileName; if (File.Exists(value)) { return value; } } catch { }
				try { string value = module.ModuleName; if (File.Exists(value)) { return value; } } catch { }
				try { string value = module.FileVersionInfo.FileName; if (File.Exists(value)) { return value; } } catch { }
			} catch { }

			return string.Empty;
		}

	}
}
