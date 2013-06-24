using System;
using System.Windows.Forms;
using System.IO;

namespace Swish.SimpleInstaller.Controls
{
	public partial class WelcomePage: UserControl
	{
		public WelcomePage()
		{
			InitializeComponent();
			string installerVersionTextFileName = Path.Combine(Application.StartupPath, "Version.txt");
			if (File.Exists(installerVersionTextFileName ))
			{
				string version = File.ReadAllText(installerVersionTextFileName);
				this._versionBox.Text = version;
			}
		}

		public event EventHandler Install;
		public event EventHandler Cancel;

		private void InstallButton_Click(object sender, EventArgs e)
		{
			if (Install != null)
			{
				Install(this, EventArgs.Empty);
			}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			if (Cancel != null)
			{
				Cancel(this, EventArgs.Empty);
			}
		}

		private void WelcomePage_Load(object sender, EventArgs e)
		{
			if (Clean)
			{
				InstallButton.Text = "Uninstall";
			}
		}


		public bool Clean { get; set; }
	}
}
