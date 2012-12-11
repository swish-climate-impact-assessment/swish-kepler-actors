using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;

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

		internal static void MessageTextBox(string title, string[] lines, bool returnImediatly)
		{
			TextBox textBox = new TextBox();
			Form form = new Form();

			textBox.Multiline = true;
			int limit = 1000;
			if (lines.Length <= limit)
			{
				textBox.Lines = lines;
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
	}
}
