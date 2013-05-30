namespace Swish.Controls
{
	partial class TableView
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
			this.Table = new System.Windows.Forms.TableLayoutPanel();
			this.GraphBox = new System.Windows.Forms.PictureBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.YMinimumBox = new Swish.Controls.VerticalLabel();
			this.YMaximum = new Swish.Controls.VerticalLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.RangeEndBar = new System.Windows.Forms.TrackBar();
			this.RangeStartBar = new System.Windows.Forms.TrackBar();
			this.panel3 = new System.Windows.Forms.Panel();
			this.XMaximum = new System.Windows.Forms.Label();
			this.XMinimumBox = new System.Windows.Forms.Label();
			this.Table.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.GraphBox)).BeginInit();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.RangeEndBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.RangeStartBar)).BeginInit();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// Table
			// 
			this.Table.ColumnCount = 2;
			this.Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Table.Controls.Add(this.GraphBox, 1, 0);
			this.Table.Controls.Add(this.panel2, 0, 0);
			this.Table.Controls.Add(this.panel1, 1, 1);
			this.Table.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Table.Location = new System.Drawing.Point(0, 0);
			this.Table.Name = "Table";
			this.Table.RowCount = 2;
			this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
			this.Table.Size = new System.Drawing.Size(347, 303);
			this.Table.TabIndex = 13;
			// 
			// GraphBox
			// 
			this.GraphBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GraphBox.Location = new System.Drawing.Point(29, 3);
			this.GraphBox.Name = "GraphBox";
			this.GraphBox.Size = new System.Drawing.Size(315, 177);
			this.GraphBox.TabIndex = 16;
			this.GraphBox.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.YMinimumBox);
			this.panel2.Controls.Add(this.YMaximum);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(20, 177);
			this.panel2.TabIndex = 1;
			// 
			// YMinimumBox
			// 
			this.YMinimumBox.Alignment = Swish.Controls.VerticalLabel.AlignmentCode.Top;
			this.YMinimumBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.YMinimumBox.AutoSize = true;
			this.YMinimumBox.Location = new System.Drawing.Point(0, 122);
			this.YMinimumBox.Name = "YMinimumBox";
			this.YMinimumBox.Orientation = Swish.Controls.VerticalLabel.OrientationCode.Right;
			this.YMinimumBox.Size = new System.Drawing.Size(52, 52);
			this.YMinimumBox.TabIndex = 1;
			this.YMinimumBox.Text = "Text here";
			// 
			// YMaximum
			// 
			this.YMaximum.Alignment = Swish.Controls.VerticalLabel.AlignmentCode.Top;
			this.YMaximum.AutoSize = true;
			this.YMaximum.Dock = System.Windows.Forms.DockStyle.Top;
			this.YMaximum.Location = new System.Drawing.Point(0, 0);
			this.YMaximum.Name = "YMaximum";
			this.YMaximum.Orientation = Swish.Controls.VerticalLabel.OrientationCode.Right;
			this.YMaximum.Size = new System.Drawing.Size(52, 52);
			this.YMaximum.TabIndex = 0;
			this.YMaximum.Text = "Text here";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.RangeEndBar);
			this.panel1.Controls.Add(this.RangeStartBar);
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(29, 186);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(315, 114);
			this.panel1.TabIndex = 1;
			// 
			// RangeEndBar
			// 
			this.RangeEndBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.RangeEndBar.Location = new System.Drawing.Point(0, 58);
			this.RangeEndBar.Maximum = 999;
			this.RangeEndBar.Name = "RangeEndBar";
			this.RangeEndBar.Size = new System.Drawing.Size(315, 45);
			this.RangeEndBar.TabIndex = 14;
			this.RangeEndBar.TickFrequency = 10;
			this.RangeEndBar.Value = 999;
			this.RangeEndBar.ValueChanged += new System.EventHandler(this.RangeEndBar_Scroll);
			// 
			// RangeStartBar
			// 
			this.RangeStartBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.RangeStartBar.Location = new System.Drawing.Point(0, 13);
			this.RangeStartBar.Maximum = 999;
			this.RangeStartBar.Name = "RangeStartBar";
			this.RangeStartBar.Size = new System.Drawing.Size(315, 45);
			this.RangeStartBar.TabIndex = 15;
			this.RangeStartBar.TickFrequency = 10;
			this.RangeStartBar.ValueChanged += new System.EventHandler(this.RangeStartBar_Scroll);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.XMaximum);
			this.panel3.Controls.Add(this.XMinimumBox);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(315, 13);
			this.panel3.TabIndex = 14;
			// 
			// XMaximum
			// 
			this.XMaximum.AutoSize = true;
			this.XMaximum.Dock = System.Windows.Forms.DockStyle.Right;
			this.XMaximum.Location = new System.Drawing.Point(280, 0);
			this.XMaximum.Name = "XMaximum";
			this.XMaximum.Size = new System.Drawing.Size(35, 13);
			this.XMaximum.TabIndex = 1;
			this.XMaximum.Text = "label1";
			// 
			// XMinimumBox
			// 
			this.XMinimumBox.AutoSize = true;
			this.XMinimumBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.XMinimumBox.Location = new System.Drawing.Point(0, 0);
			this.XMinimumBox.Name = "XMinimumBox";
			this.XMinimumBox.Size = new System.Drawing.Size(35, 13);
			this.XMinimumBox.TabIndex = 0;
			this.XMinimumBox.Text = "label1";
			// 
			// TableView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.Table);
			this.Name = "TableView";
			this.Size = new System.Drawing.Size(347, 303);
			this.Load += new System.EventHandler(this.TableView_Load);
			this.VisibleChanged += new System.EventHandler(this.TableView_VisibleChanged);
			this.Resize += new System.EventHandler(this.TableView_Resize);
			this.Table.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.GraphBox)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.RangeEndBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.RangeStartBar)).EndInit();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox GraphBox;
		private System.Windows.Forms.TrackBar RangeStartBar;
		private System.Windows.Forms.TrackBar RangeEndBar;
		private System.Windows.Forms.TableLayoutPanel Table;
		private System.Windows.Forms.Panel panel2;
		private VerticalLabel YMinimumBox;
		private VerticalLabel YMaximum;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label XMaximum;
		private System.Windows.Forms.Label XMinimumBox;

	}
}

