using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Swish
{
	public static class SwishFunctions
	{
		public static void RunProcess(string fileName, string arguments, string workingDirectory, bool returnBeforeExit, TimeSpan timeout, out int exitCode, out string output)
		{
			using (Process run = new Process())
			{
				run.StartInfo.Arguments = arguments;
				if (!string.IsNullOrWhiteSpace(workingDirectory))
				{
					run.StartInfo.WorkingDirectory = workingDirectory;
				}
				run.StartInfo.FileName = fileName;

				run.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				if (!returnBeforeExit)
				{
					run.StartInfo.RedirectStandardOutput = true;
					run.StartInfo.RedirectStandardError = true;
					run.StartInfo.UseShellExecute = false;
					run.Start();
				} else
				{
					run.Start();
					exitCode = -1;
					output = null;
					run.Close();
					return;
				}

				if (timeout != null && timeout > TimeSpan.Zero)
				{
					run.WaitForExit((int)timeout.TotalMilliseconds);
					if (!run.HasExited)
					{
						run.Kill();
						run.WaitForExit();
						exitCode = run.ExitCode;
						output = run.StandardOutput.ReadToEnd();
						output += run.StandardError.ReadToEnd();
						run.Close();
						throw new Exception("Process timmed out");
					}
				} else
				{
					run.WaitForExit(Int32.MaxValue - 1);
				}

				exitCode = run.ExitCode;
				output = run.StandardError.ReadToEnd();
				output += run.StandardOutput.ReadToEnd();
				run.Close();
			}
		}

		public static void MessageTextBox(string message, bool returnImediatly)
		{
			MessageTextBox("Message", message.Split('\r', '\n'), returnImediatly);
		}

		internal static void MessageTextBox(string title, IList<string> lines, bool returnImediatly)
		{
			TextBox textBox = new TextBox();
			Form form = new Form();

			textBox.Multiline = true;
			int limit = 1000;
			if (lines.Count <= limit)
			{
				textBox.Lines = new List<string>(lines).ToArray();
			} else
			{
				string[] shortLines = new string[limit + 2];
				for (int lineIndex = 0; lineIndex < limit; lineIndex++)
				{
					shortLines[lineIndex] = lines[lineIndex];
				}
				shortLines[limit] = string.Empty;
				shortLines[limit + 1] = "Note, text truncated at 1000 lines.";

				textBox.Lines = shortLines;
			}
			textBox.SelectionStart = 0;
			textBox.SelectionLength = 0;
			textBox.ScrollBars = ScrollBars.Both;
			textBox.Size = new Size(400, 400);
			textBox.Dock = DockStyle.Fill;
			Size size = textBox.Size;
			int controlHeight = size.Height;

			form.ControlBox = true;

			form.Text = title;
			size = textBox.Size;
			form.ClientSize = new Size(size.Width, controlHeight);
			form.Controls.Add(textBox);
			textBox.BringToFront();

			if (returnImediatly)
			{
				form.Show();
				Application.DoEvents();
				Thread.Sleep(100);
			} else
			{
				using (textBox)
				using (form)
				{
					try
					{
						form.ShowDialog();
					} finally
					{
						bool flag = form == null;
						if (!flag)
						{
							form.Dispose();
						}
					}
				}
			}
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
					_monoEnvironment = Type.GetType("Mono.Runtime") != null;
					_environmentDetected = true;
				}
				bool flag1 = _monoEnvironment;
				return flag1;
			}
		}

		public static Form GetForm(object sender)
		{
			Control parent = (Control)sender;
			while (true)
			{
				if (parent == null)
				{
					return null;
				}
				if (parent is Form)
				{
					return parent as Form;
				}
				parent = parent.Parent;
			}
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

		internal static string GeneratePasswordFileName(string prompt, Process process)
		{
			int encodeLength = 8;
			byte[] encodedBytes = new byte[encodeLength];

			byte[] promptBytes = ASCIIEncoding.ASCII.GetBytes(prompt);
			byte[] idBytes = BitConverter.GetBytes((short)process.Id);

			for (int byteIndex = 0; byteIndex < promptBytes.Length; byteIndex++)
			{
				byte value = (byte)(promptBytes[byteIndex] ^ idBytes[byteIndex % idBytes.Length] ^ encodedBytes[byteIndex % encodeLength]);
				encodedBytes[byteIndex % encodeLength] = value;
			}

			string encodedFileName;
			using (MemoryStream _stream = new MemoryStream())
			{
				using (TextCodedStream stream = new TextCodedStream(_stream, true, true))
				{
					stream.Write(encodedBytes, 0, encodeLength);
					stream.Flush();
				}
				_stream.Position = 0;
				encodedFileName = StreamFunctions.ToString(_stream);
			}

			return encodedFileName;
		}

		public static string EncodePassword(string password, Process process)
		{
			byte[] passwordBytes = ASCIIEncoding.ASCII.GetBytes(password);
			byte[] encodedBytes = MangleBytes(passwordBytes, process);

			string encodedPassword;
			using (MemoryStream _stream = new MemoryStream())
			{
				using (TextCodedStream stream = new TextCodedStream(_stream, true, true))
				{
					stream.Write(encodedBytes, 0, encodedBytes.Length);
					stream.Flush();
				}
				_stream.Position = 0;
				encodedPassword = StreamFunctions.ToString(_stream);
			}

			return encodedPassword;
		}

		public static byte[] MangleBytes(byte[] bytes, Process process)
		{
			byte[] nameBytes = ASCIIEncoding.ASCII.GetBytes(process.ProcessName);
			byte[] timeBytes = BitConverter.GetBytes(process.StartTime.Ticks);

			byte[] mangledBytes = new byte[bytes.Length];

			for (int byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
			{
				byte value = (byte)(bytes[byteIndex] ^ nameBytes[byteIndex % nameBytes.Length] ^ timeBytes[byteIndex % timeBytes.Length]);
				mangledBytes[byteIndex] = value;
			}

			return mangledBytes;
		}

		public static string DecodePassword(string encodedPassword, Process process)
		{
			byte[] encodedBytes;
			using (TextAsStream _stream = new TextAsStream(encodedPassword))
			using (TextCodedStream stream = new TextCodedStream(_stream, false, false))
			{
				List<byte> _encodedBytes = new List<byte>();

				int bufferSize = 1;
				byte[] buffer = new byte[bufferSize];
				while (true)
				{
					int countRead = stream.Read(buffer, 0, bufferSize);
					if (countRead != bufferSize)
					{
						break;
					}
					_encodedBytes.Add(buffer[0]);
				}

				encodedBytes = _encodedBytes.ToArray();
			}
			byte[] passwordBytes = MangleBytes(encodedBytes, process);

			string password = ASCIIEncoding.ASCII.GetString(passwordBytes);
			return password;
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
						Process parent = SwishFunctions.ParentProcess(current);
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

		public static string WriteProcessInformation(Process process)
		{
			string message = string.Empty;
			try { string value = process.Id.ToString(); if (!string.IsNullOrWhiteSpace(value)) { message += "Process id: " + value + Environment.NewLine; } } catch { }
			try { string value = process.ProcessName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tProcess name: " + value + Environment.NewLine; } } catch { }
			try { string value = process.StartTime.ToShortTimeString(); if (!string.IsNullOrWhiteSpace(value)) { message += "\tStart time: " + value + Environment.NewLine; } } catch { }
			try { string value = process.StartInfo.FileName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tFile name: " + value + Environment.NewLine; } } catch { }
			try { string value = process.StartInfo.Arguments; if (!string.IsNullOrWhiteSpace(value)) { message += "\tArguments: " + value + Environment.NewLine; } } catch { }
			try { string value = process.StartInfo.WorkingDirectory; if (!string.IsNullOrWhiteSpace(value)) { message += "\tWorking directory: " + value + Environment.NewLine; } } catch { }
			try { string value = process.MachineName; if (!string.IsNullOrWhiteSpace(value)) { message += "\tMachine name: " + value + Environment.NewLine; } } catch { }
			try { string value = process.SessionId.ToString(); if (!string.IsNullOrWhiteSpace(value)) { message += "\tSession id: " + value + Environment.NewLine; } } catch { }

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
				message += SwishFunctions.WriteProcessInformation(process);
				process = SwishFunctions.ParentProcess(process);
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
					lines.Add(specialFolderNames[index2] + ": " + Environment.GetFolderPath(folder, option));
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
	}
}
