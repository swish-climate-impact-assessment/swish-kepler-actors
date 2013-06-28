using System;
using System.Drawing;
using System.Windows.Forms;

namespace Swish.Controls
{
	public partial class DisplayForm: Form
	{
		public static bool Display(Control control, string title, bool okButton, bool cancelButton)
		{
			DialogResult result;
			using (DisplayForm form = new DisplayForm())
			{
				form.SetUp(control, title, okButton, cancelButton);
				result = form.ShowDialog();
			}
			return result == DialogResult.OK;
		}

		public DisplayForm()
		{
			InitializeComponent();
		}

		internal void SetUp(Control control, string title, bool okButton, bool cancelButton)
		{
			control.Dock = DockStyle.Fill;

			Application.DoEvents();
			int controlHeight = control.Size.Height;
			if (okButton || cancelButton)
			{
				FormOKButton.Visible = okButton;
				FormCancelButton.Visible = cancelButton;
				buttonPanel.Visible = true;
				controlHeight += buttonPanel.Size.Height;
				ControlBox = false;
			} else
			{
				buttonPanel.Visible = false;
				ControlBox = true;
			}

			Text = title;
			ClientSize = new Size(control.Size.Width, controlHeight);
			Controls.Add(control);
			control.BringToFront();
		}

		private void DisplayForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form form = GetForm(sender);
			if (form.DialogResult != DialogResult.OK)
			{
				form.DialogResult = DialogResult.Cancel;
			}

			//if (!)
			//{
			// e.Cancel = true;
			//}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			Form form = GetForm(sender);
			form.DialogResult = DialogResult.Cancel;
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			Form form = GetForm(sender);
			form.DialogResult = DialogResult.OK;
		}

		private void DisplayForm_KeyUp(object sender, KeyEventArgs e)
		{
			if ((Keys)e.KeyValue == Keys.Escape)
			{
				Close();
				e.Handled = true;
			}
		}

		public static Form GetForm(object sender)
		{
			Control item = (Control)sender;
			while (item != null && !(item is Form))
			{
				item = item.Parent;
			}
			if (item == null)
			{
				// I don't know
				return null;
			}

			Form form = (Form)item;
			return form;
		}

	}
}

