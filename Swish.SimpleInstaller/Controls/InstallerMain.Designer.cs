namespace Swish.SimpleInstaller.Controls
{
	partial class InstallerMain
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.finishedPage1 = new Swish.SimpleInstaller.Controls.FinishedPage();
			this.progressPage1 = new Swish.SimpleInstaller.Controls.ProgressPage();
			this.welcomePage1 = new Swish.SimpleInstaller.Controls.WelcomePage();
			this.KeplerVerifyPage = new Swish.SimpleInstaller.Controls.KeplerNotInstalledPage();
			this.SuspendLayout();
			// 
			// finishedPage1
			// 
			this.finishedPage1.Launch = true;
			this.finishedPage1.Location = new System.Drawing.Point(148, 118);
			this.finishedPage1.Name = "finishedPage1";
			this.finishedPage1.Size = new System.Drawing.Size(425, 317);
			this.finishedPage1.TabIndex = 2;
			this.finishedPage1.Exit += new System.EventHandler(this.finishedPage1_Exit);
			// 
			// progressPage1
			// 
			this.progressPage1.Clean = false;
			this.progressPage1.Location = new System.Drawing.Point(62, 77);
			this.progressPage1.Name = "progressPage1";
			this.progressPage1.Size = new System.Drawing.Size(474, 379);
			this.progressPage1.TabIndex = 1;
			this.progressPage1.Next += new System.EventHandler(this.progressPage1_Next);
			// 
			// welcomePage1
			// 
			this.welcomePage1.Clean = false;
			this.welcomePage1.Location = new System.Drawing.Point(12, 45);
			this.welcomePage1.Name = "welcomePage1";
			this.welcomePage1.Size = new System.Drawing.Size(514, 248);
			this.welcomePage1.TabIndex = 0;
			this.welcomePage1.Install += new System.EventHandler(this.welcomePage1_Install);
			this.welcomePage1.Cancel += new System.EventHandler(this.welcomePage1_Cancel);
			// 
			// KeplerVerifyPage
			// 
			this.KeplerVerifyPage.Location = new System.Drawing.Point(198, 175);
			this.KeplerVerifyPage.Name = "KeplerVerifyPage";
			this.KeplerVerifyPage.Size = new System.Drawing.Size(474, 379);
			this.KeplerVerifyPage.TabIndex = 3;
			this.KeplerVerifyPage.Failed += new System.EventHandler(this.KeplerVerifyPage_Failed);
			// 
			// InstallerMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(534, 422);
			this.Controls.Add(this.KeplerVerifyPage);
			this.Controls.Add(this.finishedPage1);
			this.Controls.Add(this.progressPage1);
			this.Controls.Add(this.welcomePage1);
			this.Name = "InstallerMain";
			this.Load += new System.EventHandler(this.InstallerMain_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private WelcomePage welcomePage1;
		private ProgressPage progressPage1;
		private FinishedPage finishedPage1;
		private KeplerNotInstalledPage KeplerVerifyPage;
	}
}
