namespace Swish.SimpleInstaller.Controls
{
	partial class FinishedPage
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
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.ExitButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._launchBox = new System.Windows.Forms.CheckBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Top;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(138, 37);
			this.label3.TabIndex = 7;
			this.label3.Text = "Finished";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Top;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(0, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(0, 29);
			this.label2.TabIndex = 8;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.ExitButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 294);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(425, 23);
			this.panel1.TabIndex = 10;
			// 
			// ExitButton
			// 
			this.ExitButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.ExitButton.Location = new System.Drawing.Point(350, 0);
			this.ExitButton.Name = "ExitButton";
			this.ExitButton.Size = new System.Drawing.Size(75, 23);
			this.ExitButton.TabIndex = 1;
			this.ExitButton.Text = "Exit";
			this.ExitButton.UseVisualStyleBackColor = true;
			this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(332, 26);
			this.label1.TabIndex = 11;
			this.label1.Text = "SWISH Kepler actors have been installed.\r\nNote that it may be necessary to restar" +
    "t for all changes to take effect.";
			// 
			// _launchBox
			// 
			this._launchBox.AutoSize = true;
			this._launchBox.Checked = true;
			this._launchBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._launchBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._launchBox.Location = new System.Drawing.Point(0, 277);
			this._launchBox.Name = "_launchBox";
			this._launchBox.Size = new System.Drawing.Size(425, 17);
			this._launchBox.TabIndex = 12;
			this._launchBox.Text = "launch Kepler";
			this._launchBox.UseVisualStyleBackColor = true;
			// 
			// FinishedPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._launchBox);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Name = "FinishedPage";
			this.Size = new System.Drawing.Size(425, 317);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button ExitButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox _launchBox;
	}
}
