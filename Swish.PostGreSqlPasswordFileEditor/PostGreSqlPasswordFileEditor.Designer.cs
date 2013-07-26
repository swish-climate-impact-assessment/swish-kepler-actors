namespace Swish.PostGreSqlPasswordFileEditor
{
	partial class PostGreSqlPasswordFileEditor
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
			this._passwordBox = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel1 = new System.Windows.Forms.Panel();
			this._removeButton = new System.Windows.Forms.Button();
			this._editButton = new System.Windows.Forms.Button();
			this._addButton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _passwordBox
			// 
			this._passwordBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
			this._passwordBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._passwordBox.FullRowSelect = true;
			this._passwordBox.GridLines = true;
			this._passwordBox.HideSelection = false;
			this._passwordBox.Location = new System.Drawing.Point(0, 0);
			this._passwordBox.MultiSelect = false;
			this._passwordBox.Name = "_passwordBox";
			this._passwordBox.ShowGroups = false;
			this._passwordBox.Size = new System.Drawing.Size(392, 158);
			this._passwordBox.TabIndex = 0;
			this._passwordBox.UseCompatibleStateImageBehavior = false;
			this._passwordBox.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Address";
			this.columnHeader1.Width = 86;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Port";
			this.columnHeader2.Width = 46;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Database name";
			this.columnHeader3.Width = 96;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "User name";
			this.columnHeader4.Width = 83;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Password";
			this.columnHeader5.Width = 72;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._removeButton);
			this.panel1.Controls.Add(this._editButton);
			this.panel1.Controls.Add(this._addButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 158);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(392, 23);
			this.panel1.TabIndex = 1;
			// 
			// _removeButton
			// 
			this._removeButton.Dock = System.Windows.Forms.DockStyle.Left;
			this._removeButton.Location = new System.Drawing.Point(150, 0);
			this._removeButton.Name = "_removeButton";
			this._removeButton.Size = new System.Drawing.Size(75, 23);
			this._removeButton.TabIndex = 2;
			this._removeButton.Text = "Remove";
			this._removeButton.UseVisualStyleBackColor = true;
			this._removeButton.Click += new System.EventHandler(this._removeButton_Click);
			// 
			// _editButton
			// 
			this._editButton.Dock = System.Windows.Forms.DockStyle.Left;
			this._editButton.Location = new System.Drawing.Point(75, 0);
			this._editButton.Name = "_editButton";
			this._editButton.Size = new System.Drawing.Size(75, 23);
			this._editButton.TabIndex = 1;
			this._editButton.Text = "Edit";
			this._editButton.UseVisualStyleBackColor = true;
			this._editButton.Click += new System.EventHandler(this._editButton_Click);
			// 
			// _addButton
			// 
			this._addButton.Dock = System.Windows.Forms.DockStyle.Left;
			this._addButton.Location = new System.Drawing.Point(0, 0);
			this._addButton.Name = "_addButton";
			this._addButton.Size = new System.Drawing.Size(75, 23);
			this._addButton.TabIndex = 0;
			this._addButton.Text = "Add";
			this._addButton.UseVisualStyleBackColor = true;
			this._addButton.Click += new System.EventHandler(this._addButton_Click);
			// 
			// PostGreSqlPasswordFileEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._passwordBox);
			this.Controls.Add(this.panel1);
			this.Name = "PostGreSqlPasswordFileEditor";
			this.Size = new System.Drawing.Size(392, 181);
			this.Load += new System.EventHandler(this.PostGreSqlPasswordFileEditor_Load);
			this.VisibleChanged += new System.EventHandler(this.PostGreSqlPasswordFileEditor_VisibleChanged);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView _passwordBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button _removeButton;
		private System.Windows.Forms.Button _editButton;
		private System.Windows.Forms.Button _addButton;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;

	}
}

