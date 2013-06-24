using System;
using System.Windows.Forms;

namespace Swish.SimpleInstaller.Controls
{
	public partial class KeplerNotInstalledPage: UserControl
	{
		public KeplerNotInstalledPage()
		{
			InitializeComponent();

		}
		public event EventHandler Failed;

		private void NextButton_Click(object sender, EventArgs e)
		{
			if (Failed != null)
			{
				Failed(this, EventArgs.Empty);
			}
		}

		private void VerifyKeplerInstalledPage_Load(object sender, EventArgs e)
		{
			StatusBox.Text = "";

			if (KeplerFunctions.Installed)
			{
				StatusBox.Text += "Kepler location: " + KeplerFunctions.BinDirectory + Environment.NewLine;
			} else
			{
				StatusBox.Text += "Kepler not found, Kepler must be installed to use SWISH Kepler Actors" + Environment.NewLine;
			}

			if (StataFunctions.Installed)
			{
				StatusBox.Text += "Stata location: " + StataFunctions.BinDirectory + Environment.NewLine;
			} else
			{
				StatusBox.Text += "Stata not found, Stata is required to use most SWISH Kepler Actors" + Environment.NewLine;
			}

			if (RFunctions.Installed)
			{
				StatusBox.Text += "R location: " + RFunctions.BinDirectory + Environment.NewLine;
			} else
			{
				StatusBox.Text += "R not found, R is used by some SWISH Kepler Actors" + Environment.NewLine;
			}

			if (JavaFunctions.Installed)
			{
				StatusBox.Text += "Java location: " + JavaFunctions.BinDirectory + Environment.NewLine;
			} else
			{
				StatusBox.Text += "Java not found, Java is required to run Kepler" + Environment.NewLine;
			}

			if (KeplerFunctions.Installed && RFunctions.Installed && JavaFunctions.Installed && StataFunctions.Installed)
			{
				NextButton.Text = "Next";
			} else
			{
				NextButton.Text = "Ignore";
			}
		}
	}
}



