namespace Swish.SimpleInstaller.Controls
{
	partial class KeplerNotInstalledPage
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
			this.StatusBox = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.NextButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// StatusBox
			// 
			this.StatusBox.AutoSize = true;
			this.StatusBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.StatusBox.Location = new System.Drawing.Point(0, 37);
			this.StatusBox.Name = "StatusBox";
			this.StatusBox.Size = new System.Drawing.Size(37, 13);
			this.StatusBox.TabIndex = 3;
			this.StatusBox.Text = "Result";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.NextButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 356);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(474, 23);
			this.panel1.TabIndex = 5;
			// 
			// NextButton
			// 
			this.NextButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.NextButton.Location = new System.Drawing.Point(399, 0);
			this.NextButton.Name = "NextButton";
			this.NextButton.Size = new System.Drawing.Size(75, 23);
			this.NextButton.TabIndex = 6;
			this.NextButton.Text = "Next";
			this.NextButton.UseVisualStyleBackColor = true;
			this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Top;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(269, 37);
			this.label3.TabIndex = 6;
			this.label3.Text = "Kepler installation";
			// 
			// VerifyKeplerInstalledPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.StatusBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.panel1);
			this.Name = "VerifyKeplerInstalledPage";
			this.Size = new System.Drawing.Size(474, 379);
			this.Load += new System.EventHandler(this.VerifyKeplerInstalledPage_Load);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label StatusBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button NextButton;
		private System.Windows.Forms.Label label3;
	}
}
