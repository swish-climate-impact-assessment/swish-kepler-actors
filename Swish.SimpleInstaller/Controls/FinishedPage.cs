using System;
using System.Windows.Forms;

namespace Swish.SimpleInstaller.Controls
{
	public partial class FinishedPage: UserControl
	{
		public FinishedPage()
		{
			InitializeComponent();
		}

		public event EventHandler Exit;
		private void ExitButton_Click(object sender, EventArgs e)
		{
			if (Exit != null)
			{
				Exit(this, EventArgs.Empty);
			}
		}



		public bool Launch
		{
			get { return _launchBox.Checked; }
			set { _launchBox.Checked = value; }
		}
	}
}
