using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Swish
{
	public static class SwishFunctions
	{
		public static void RunProcess(string fileName, string arguments, string workingDirectory, bool returnBeforeExit, out int exitCode, out string output)
		{
			Process run = new Process();
			run.StartInfo.Arguments = arguments;
			if (!string.IsNullOrWhiteSpace(workingDirectory))
			{
				run.StartInfo.WorkingDirectory = workingDirectory;
			}
			run.StartInfo.FileName = fileName;
			run.StartInfo.RedirectStandardOutput = true;
			run.StartInfo.UseShellExecute = false;
			run.Start();

			if (returnBeforeExit)
			{
				exitCode = -1;
				output = null;
				return;
			}

			while (!run.HasExited)
			{
				Thread.Sleep(1);
			}
			exitCode = run.ExitCode;
			output = run.StandardOutput.ReadToEnd();
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
				if (SwishFunctions.FileExists(path))
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

		public static List<string> Locations()
		{
			List<string> binaryLocations = new List<string>(new string[]{
					@"C:\Swish\bin"
				});

			string fileName = SwishFunctions.ResloveFileName("DevelopmentLocations.txt", binaryLocations, false, true);
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return binaryLocations;
			}

			List<string> locations = new List<string>(File.ReadAllLines(fileName));
			locations.AddRange(binaryLocations);
			return locations;
		}

		internal static void MessageTextBox(string message)
		{
			string title = "Message";

			using (TextBox textBox = new TextBox())
			using (Form form = new Form())
			{
				try
				{
					textBox.Multiline = true;
					textBox.Text = message;
					textBox.SelectionStart = 0;
					textBox.SelectionLength = 0;
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

		public static string TempoaryOutputFileName(string extension)
		{
			string tempOutputFileName = Path.GetTempFileName();
			if (SwishFunctions.FileExists(tempOutputFileName))
			{
				File.Delete(tempOutputFileName);
			}
			string outputFileName = tempOutputFileName + extension;
			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			return outputFileName;
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

		public static string AdjustFileName(string fileName)
		{
			bool monoEnvironment = !MonoEnvironment;
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
	}
}
