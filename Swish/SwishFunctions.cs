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
				run.StartInfo.RedirectStandardOutput = true;
				run.StartInfo.RedirectStandardError = true;
				run.StartInfo.UseShellExecute = false;
				run.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				run.Start();

				if (returnBeforeExit)
				{
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
					run.WaitForExit();
				}

				exitCode = run.ExitCode;
				output = run.StandardError.ReadToEnd();
				output += run.StandardOutput.ReadToEnd();
				run.Close();
			}
		}

		public static void MessageTextBox(string message)
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

	}
}
