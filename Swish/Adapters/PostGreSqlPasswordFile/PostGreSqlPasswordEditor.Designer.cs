namespace Swish.Adapters.PostGreSqlPasswordFile
{
	partial class PostGreSqlPasswordEditor
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
			this.label2 = new System.Windows.Forms.Label();
			this._databaseBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._addressBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this._portBox = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this._userNameBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this._passwordBox = new System.Windows.Forms.TextBox();
			this._maskPasswordBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this._portBox)).BeginInit();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Top;
			this.label2.Location = new System.Drawing.Point(0, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Database name";
			// 
			// _databaseBox
			// 
			this._databaseBox.Dock = System.Windows.Forms.DockStyle.Top;
			this._databaseBox.Location = new System.Drawing.Point(0, 46);
			this._databaseBox.Name = "_databaseBox";
			this._databaseBox.Size = new System.Drawing.Size(263, 20);
			this._databaseBox.TabIndex = 3;
			this._databaseBox.TextChanged += new System.EventHandler(this._databaseBox_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Top;
			this.label3.Location = new System.Drawing.Point(0, 66);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Address";
			// 
			// _addressBox
			// 
			this._addressBox.Dock = System.Windows.Forms.DockStyle.Top;
			this._addressBox.Location = new System.Drawing.Point(0, 79);
			this._addressBox.Name = "_addressBox";
			this._addressBox.Size = new System.Drawing.Size(263, 20);
			this._addressBox.TabIndex = 5;
			this._addressBox.TextChanged += new System.EventHandler(this._addressBox_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Top;
			this.label4.Location = new System.Drawing.Point(0, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(26, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Port";
			// 
			// _portBox
			// 
			this._portBox.Dock = System.Windows.Forms.DockStyle.Top;
			this._portBox.Location = new System.Drawing.Point(0, 112);
			this._portBox.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this._portBox.Name = "_portBox";
			this._portBox.Size = new System.Drawing.Size(263, 20);
			this._portBox.TabIndex = 7;
			this._portBox.Value = new decimal(new int[] {
            5432,
            0,
            0,
            0});
			this._portBox.ValueChanged += new System.EventHandler(this._portBox_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "User name";
			// 
			// _userNameBox
			// 
			this._userNameBox.Dock = System.Windows.Forms.DockStyle.Top;
			this._userNameBox.Location = new System.Drawing.Point(0, 13);
			this._userNameBox.Name = "_userNameBox";
			this._userNameBox.Size = new System.Drawing.Size(263, 20);
			this._userNameBox.TabIndex = 1;
			this._userNameBox.TextChanged += new System.EventHandler(this._userNameBox_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Dock = System.Windows.Forms.DockStyle.Top;
			this.label5.Location = new System.Drawing.Point(0, 132);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Password";
			// 
			// _passwordBox
			// 
			this._passwordBox.Dock = System.Windows.Forms.DockStyle.Top;
			this._passwordBox.Location = new System.Drawing.Point(0, 145);
			this._passwordBox.Name = "_passwordBox";
			this._passwordBox.PasswordChar = '#';
			this._passwordBox.Size = new System.Drawing.Size(263, 20);
			this._passwordBox.TabIndex = 9;
			this._passwordBox.TextChanged += new System.EventHandler(this._passwordBox_TextChanged);
			// 
			// _maskPasswordBox
			// 
			this._maskPasswordBox.AutoSize = true;
			this._maskPasswordBox.Checked = true;
			this._maskPasswordBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._maskPasswordBox.Dock = System.Windows.Forms.DockStyle.Top;
			this._maskPasswordBox.Location = new System.Drawing.Point(0, 165);
			this._maskPasswordBox.Name = "_maskPasswordBox";
			this._maskPasswordBox.Size = new System.Drawing.Size(263, 17);
			this._maskPasswordBox.TabIndex = 10;
			this._maskPasswordBox.Text = "Mask";
			this._maskPasswordBox.UseVisualStyleBackColor = true;
			this._maskPasswordBox.CheckedChanged += new System.EventHandler(this._maskPasswordBox_CheckedChanged);
			// 
			// PostGreSqlPasswordEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._maskPasswordBox);
			this.Controls.Add(this._passwordBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this._portBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._addressBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._databaseBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._userNameBox);
			this.Controls.Add(this.label1);
			this.MinimumSize = new System.Drawing.Size(263, 181);
			this.Name = "PostGreSqlPasswordEditor";
			this.Size = new System.Drawing.Size(263, 181);
			this.Load += new System.EventHandler(this.PostGreSqlPasswordEditor_Load);
			this.VisibleChanged += new System.EventHandler(this.PostGreSqlPasswordEditor_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this._portBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _databaseBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _addressBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown _portBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _userNameBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox _passwordBox;
		private System.Windows.Forms.CheckBox _maskPasswordBox;



	}
}

