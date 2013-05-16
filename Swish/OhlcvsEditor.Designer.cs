namespace Swish
{
	partial class OhlcvsEditor
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.GraphBox = new System.Windows.Forms.PictureBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.BalancesBox = new System.Windows.Forms.ListView();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.AverageBox = new System.Windows.Forms.PictureBox();
			this.MinimumDateBar = new System.Windows.Forms.TrackBar();
			this.MaximumDateBar = new System.Windows.Forms.TrackBar();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.GraphBox)).BeginInit();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.AverageBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MinimumDateBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MaximumDateBar)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(574, 394);
			this.tabControl1.TabIndex = 12;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.GraphBox);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(566, 368);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Graph";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// GraphBox
			// 
			this.GraphBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GraphBox.Location = new System.Drawing.Point(3, 3);
			this.GraphBox.Name = "GraphBox";
			this.GraphBox.Size = new System.Drawing.Size(560, 362);
			this.GraphBox.TabIndex = 9;
			this.GraphBox.TabStop = false;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.BalancesBox);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(566, 368);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Data";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// BalancesBox
			// 
			this.BalancesBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BalancesBox.FullRowSelect = true;
			this.BalancesBox.GridLines = true;
			this.BalancesBox.HideSelection = false;
			this.BalancesBox.Location = new System.Drawing.Point(3, 3);
			this.BalancesBox.MultiSelect = false;
			this.BalancesBox.Name = "BalancesBox";
			this.BalancesBox.ShowGroups = false;
			this.BalancesBox.Size = new System.Drawing.Size(560, 339);
			this.BalancesBox.TabIndex = 11;
			this.BalancesBox.UseCompatibleStateImageBehavior = false;
			this.BalancesBox.View = System.Windows.Forms.View.Details;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.AverageBox);
			this.tabPage3.Controls.Add(this.MinimumDateBar);
			this.tabPage3.Controls.Add(this.MaximumDateBar);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(566, 368);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "20 Day Average";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// AverageBox
			// 
			this.AverageBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AverageBox.Location = new System.Drawing.Point(3, 3);
			this.AverageBox.Name = "AverageBox";
			this.AverageBox.Size = new System.Drawing.Size(560, 272);
			this.AverageBox.TabIndex = 10;
			this.AverageBox.TabStop = false;
			// 
			// MinimumDateBar
			// 
			this.MinimumDateBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MinimumDateBar.Location = new System.Drawing.Point(3, 275);
			this.MinimumDateBar.Maximum = 999;
			this.MinimumDateBar.Name = "MinimumDateBar";
			this.MinimumDateBar.Size = new System.Drawing.Size(560, 45);
			this.MinimumDateBar.TabIndex = 12;
			this.MinimumDateBar.TickFrequency = 10;
			this.MinimumDateBar.Scroll += new System.EventHandler(this.MinimumBar_Scroll);
			// 
			// MaximumDateBar
			// 
			this.MaximumDateBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MaximumDateBar.Location = new System.Drawing.Point(3, 320);
			this.MaximumDateBar.Maximum = 999;
			this.MaximumDateBar.Name = "MaximumDateBar";
			this.MaximumDateBar.Size = new System.Drawing.Size(560, 45);
			this.MaximumDateBar.TabIndex = 11;
			this.MaximumDateBar.TickFrequency = 10;
			this.MaximumDateBar.Value = 999;
			this.MaximumDateBar.Scroll += new System.EventHandler(this.MaximumDateBar_Scroll);
			// 
			// OhlcvsEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl1);
			this.Name = "OhlcvsEditor";
			this.Size = new System.Drawing.Size(574, 394);
			this.Load += new System.EventHandler(this.OhlcvsEditor_Load);
			this.VisibleChanged += new System.EventHandler(this.OhlcvsEditor_VisibleChanged);
			this.Resize += new System.EventHandler(this.OhlcvsEditor_Resize);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.GraphBox)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.AverageBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MinimumDateBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MaximumDateBar)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.PictureBox GraphBox;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ListView BalancesBox;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.PictureBox AverageBox;
		private System.Windows.Forms.TrackBar MinimumDateBar;
		private System.Windows.Forms.TrackBar MaximumDateBar;

	}
}

