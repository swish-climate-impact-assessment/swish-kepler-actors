namespace Swish.Controls
{
	partial class DisplayForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonPanel = new System.Windows.Forms.Panel();
			this.FormOKButton = new System.Windows.Forms.Button();
			this.FormCancelButton = new System.Windows.Forms.Button();
			this.buttonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonPanel
			// 
			this.buttonPanel.Controls.Add(this.FormOKButton);
			this.buttonPanel.Controls.Add(this.FormCancelButton);
			this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonPanel.Location = new System.Drawing.Point(0, 182);
			this.buttonPanel.Name = "buttonPanel";
			this.buttonPanel.Size = new System.Drawing.Size(292, 23);
			this.buttonPanel.TabIndex = 0;
			// 
			// OKButton
			// 
			this.FormOKButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.FormOKButton.Location = new System.Drawing.Point(142, 0);
			this.FormOKButton.Name = "OKButton";
			this.FormOKButton.Size = new System.Drawing.Size(75, 23);
			this.FormOKButton.TabIndex = 0;
			this.FormOKButton.Text = "OK";
			this.FormOKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CancelButton
			// 
			this.FormCancelButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.FormCancelButton.Location = new System.Drawing.Point(217, 0);
			this.FormCancelButton.Name = "CancelButton";
			this.FormCancelButton.Size = new System.Drawing.Size(75, 23);
			this.FormCancelButton.TabIndex = 1;
			this.FormCancelButton.Text = "Cancel";
			this.FormCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// DisplayForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(292, 205);
			this.Controls.Add(this.buttonPanel);
			this.DoubleBuffered = true;
			this.KeyPreview = true;
			this.Name = "DisplayForm";
			this.Text = "DisplayForm";
			this.buttonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		System.Windows.Forms.Button FormOKButton;
		System.Windows.Forms.Button FormCancelButton;
		System.Windows.Forms.Panel buttonPanel;

	}
}

