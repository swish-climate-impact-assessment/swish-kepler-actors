using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Swish.Adapters
{
	public class PasswordAdapter: IAdapter
	{
		public string Name { get { return "password"; } }

		public string Run(AdapterArguments splitArguments)
		{
			string prompt = splitArguments.String(Arguments.DefaultArgumentPrefix + "prompt", false);
			bool requireEntry = splitArguments.Bool(Arguments.DefaultArgumentPrefix + "ignoreCache", false);
			string password = Password(prompt, requireEntry);
			return password;
		}

		/// <summary>
		/// force means ignore any cache and require the user to enter a password
		/// </summary>
		public static string Password(string prompt, bool requireEntry)
		{
			string passwordFileName;
			if (!string.IsNullOrWhiteSpace(prompt))
			{
				if (ProcessFunctions.KeplerProcess != null)
				{
					passwordFileName = SwishFunctions.GeneratePasswordFileName(prompt, ProcessFunctions.KeplerProcess);
					passwordFileName = Path.Combine(Path.GetTempPath(), passwordFileName);

					if (!requireEntry && FileFunctions.FileExists(passwordFileName))
					{
						string _encodedPassword = File.ReadAllText(passwordFileName);
						string _password = SwishFunctions.DecodePassword(_encodedPassword, ProcessFunctions.KeplerProcess);
						return _password;
					}
				} else
				{
					passwordFileName = string.Empty;
				}
			} else
			{
				prompt = "Please enter password";
				passwordFileName = string.Empty;
			}

			string password;

			using (MaskedTextBox textBox = new MaskedTextBox())
			using (Panel panel = new Panel())
			using (Button buton = new Button())
			using (Form form = new Form())
			{
				textBox.SuspendLayout();
				buton.SuspendLayout();
				panel.SuspendLayout();
				form.SuspendLayout();

				textBox.UseSystemPasswordChar = true;
				textBox.Multiline = true;
				textBox.SelectionStart = 0;
				textBox.SelectionLength = 0;
				textBox.Size = new Size(300, textBox.Height);
				textBox.Dock = DockStyle.Top;
				textBox.Font = new Font(textBox.Font, FontStyle.Bold);
				textBox.TabIndex = 0;

				buton.Click += new EventHandler(buton_Click);
				buton.Dock = DockStyle.Left;
				buton.Text = "Ok";
				buton.Size = new Size(75, 23);
				buton.TabIndex = 0;

				panel.Height = 23;
				panel.Controls.Add(buton);
				panel.Dock = DockStyle.Fill;
				panel.TabIndex = 1;

				form.ControlBox = false;
				form.Text = prompt;
				form.ClientSize = new Size(300, 43);
				form.Controls.Add(panel);
				form.Controls.Add(textBox);
				form.AcceptButton = buton;

				textBox.ResumeLayout();
				buton.ResumeLayout();
				panel.ResumeLayout();
				form.ResumeLayout();

				textBox.Focus();
				form.ShowDialog();

				password = textBox.Text;
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				return string.Empty;
			}

			if (string.IsNullOrWhiteSpace(passwordFileName))
			{
				return password;
			}

			string encodedPassword = SwishFunctions.EncodePassword(password, ProcessFunctions.KeplerProcess);
			if (FileFunctions.FileExists(passwordFileName))
			{
				FileFunctions.DeleteFile(passwordFileName, null);
			}
			File.WriteAllText(passwordFileName, encodedPassword);

			return password;
		}

		static void buton_Click(object sender, EventArgs e)
		{
			Form form = SwishFunctions.GetForm(sender);
			if (form != null)
			{
				form.Close();
			}
		}

	}
}
