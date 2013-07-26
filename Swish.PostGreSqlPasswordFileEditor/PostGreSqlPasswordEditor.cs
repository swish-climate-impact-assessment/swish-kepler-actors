using System;
using System.Windows.Forms;

namespace Swish.PostGreSqlPasswordFileEditor
{
	internal partial class PostGreSqlPasswordEditor: UserControl
	{
		public PostGreSqlPasswordEditor()
		{
			InitializeComponent();
		}

		public PostGreSqlPassword Password { get; set; }

		private void Parent_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void PostGreSqlPasswordEditor_VisibleChanged(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			if (Visible && _updateRequired)
			{
				PopulateForm();
			}
		}

		private void PostGreSqlPasswordEditor_Load(object sender, EventArgs e)
		{
			Form form = DisplayForm.GetForm(this);
			if (form != null)
			{
				form.FormClosed += new FormClosedEventHandler(Parent_FormClosed);
			}

			ResumePopulateForm();
		}

		/// <summary>
		/// prevents modifications to user controls. useful while updating multiple data items in quick succession.
		/// suspending populate form will prevent the control from refreshing until the resume populate form function is called
		/// </summary>
		internal void SuspendPopulateForm()
		{
			_ignoreUpdate = true;
		}

		/// <summary>
		/// reinstate default control refresh behaviour to update as data is modified, and refreshes the control.
		/// control refreshes can be disabled using the Suspend populate form function
		/// </summary>
		internal void ResumePopulateForm()
		{
			if (!_ignoreUpdate)
			{
				return;
			}
			_ignoreUpdate = false;
			PopulateForm();
		}

		private bool _updateRequired = true;
		private bool _ignoreUpdate = true;
		/// <summary>
		/// sets properties Of current form and children forms with current data that has been supplied 
		/// can be disabled by calling SuspendPopulateForm
		/// can be enabled by calling ResumePopulateForm
		/// typically called when data has changed as a result of being set
		/// if lots of data is being provided to this form, suggest suspending updates until all data has been assigned
		/// </summary>
		internal void PopulateForm()
		{
			if (_ignoreUpdate)
			{
				return;
			}
			if (!this.Visible)
			{
				_updateRequired = true;
				return;
			}

			_ignoreUpdate = true;
			try
			{
				if (Password != null)
				{
					_addressBox.Text = Password.Address;
					_databaseBox.Text = Password.DatabaseName;
					_passwordBox.Text = Password.Password;
					_portBox.Value = Password.Port;
					_userNameBox.Text = Password.UserName;
				}
			} finally
			{
				_updateRequired = false;
				_ignoreUpdate = false;
			}
		}

		private void _maskPasswordBox_CheckedChanged(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			if (_maskPasswordBox.Checked)
			{
				_passwordBox.PasswordChar = '#';
			} else
			{
				_passwordBox.PasswordChar = '\0';
			}
		}

		private void _userNameBox_TextChanged(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			if (Password != null)
			{
				Password.UserName = _userNameBox.Text;
			}
		}

		private void _databaseBox_TextChanged(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			if (Password != null)
			{
				Password.DatabaseName = _databaseBox.Text;
			}
		}

		private void _addressBox_TextChanged(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			if (Password != null)
			{
				Password.Address = _addressBox.Text;
			}
		}

		private void _portBox_ValueChanged(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			if (Password != null)
			{
				Password.Port = (int)_portBox.Value;
			}
		}

		private void _passwordBox_TextChanged(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			if (Password != null)
			{
				Password.Password = _passwordBox.Text;
			}
		}


	}
}

