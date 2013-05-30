namespace Swish.Controls
{
	partial class GridView
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
			this.PictureBox = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.XMaximum = new System.Windows.Forms.Label();
			this.XMinimumBox = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.YMinimumBox = new Swish.Controls.VerticalLabel();
			this.YMaximum = new Swish.Controls.VerticalLabel();
			this.Table = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.Table.SuspendLayout();
			this.SuspendLayout();
			// 
			// PictureBox
			// 
			this.PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PictureBox.Location = new System.Drawing.Point(29, 3);
			this.PictureBox.Name = "PictureBox";
			this.PictureBox.Size = new System.Drawing.Size(180, 142);
			this.PictureBox.TabIndex = 0;
			this.PictureBox.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.XMaximum);
			this.panel1.Controls.Add(this.XMinimumBox);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(29, 151);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(180, 14);
			this.panel1.TabIndex = 1;
			// 
			// XMaximum
			// 
			this.XMaximum.AutoSize = true;
			this.XMaximum.Dock = System.Windows.Forms.DockStyle.Right;
			this.XMaximum.Location = new System.Drawing.Point(145, 0);
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
			// panel2
			// 
			this.panel2.Controls.Add(this.YMinimumBox);
			this.panel2.Controls.Add(this.YMaximum);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(20, 142);
			this.panel2.TabIndex = 1;
			// 
			// YMinimumBox
			// 
			this.YMinimumBox.Alignment = Swish.Controls.VerticalLabel.AlignmentCode.Top;
			this.YMinimumBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.YMinimumBox.AutoSize = true;
			this.YMinimumBox.Location = new System.Drawing.Point(0, 87);
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
			// Table
			// 
			this.Table.ColumnCount = 2;
			this.Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Table.Controls.Add(this.PictureBox, 1, 0);
			this.Table.Controls.Add(this.panel2, 0, 0);
			this.Table.Controls.Add(this.panel1, 1, 1);
			this.Table.Location = new System.Drawing.Point(84, 87);
			this.Table.Name = "Table";
			this.Table.RowCount = 2;
			this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.Table.Size = new System.Drawing.Size(212, 168);
			this.Table.TabIndex = 2;
			// 
			// GridView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.Table);
			this.DoubleBuffered = true;
			this.Name = "GridView";
			this.Size = new System.Drawing.Size(394, 364);
			this.Load += new System.EventHandler(this.GridView_Load);
			this.Resize += new System.EventHandler(this.GridView_Resize);
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.Table.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox PictureBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label XMaximum;
		private System.Windows.Forms.Label XMinimumBox;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TableLayoutPanel Table;
		private VerticalLabel YMaximum;
		private VerticalLabel YMinimumBox;

	}
}

