using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Swish.Controls;

namespace Swish.Adapters.PostGreSqlPasswordFile
{
	internal partial class PostGreSqlPasswordFileEditor: UserControl
	{
		public PostGreSqlPasswordFileEditor()
		{
			InitializeComponent();
		}

		private List<PostGreSqlPassword> _passwords = new List<PostGreSqlPassword>();
		public List<PostGreSqlPassword> Passwords
		{
			get { return _passwords; }
			set
			{
				if (value == null || value.Count == 0)
				{
					_passwords = new List<PostGreSqlPassword>();
					return;
				}
				_passwords = new List<PostGreSqlPassword>(value);
			}
		}

		private void Parent_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void PostGreSqlPasswordFileEditor_VisibleChanged(object sender, EventArgs e)
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

		private void PostGreSqlPasswordFileEditor_Load(object sender, EventArgs e)
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

				List<ListViewItem> items = new List<ListViewItem>();
				for (int passwordIndex = 0; passwordIndex < _passwords.Count; passwordIndex++)
				{
					PostGreSqlPassword password = _passwords[passwordIndex];

					ListViewItem item = new ListViewItem();

					SetItem(item, password);

					items.Add(item);
				}

				_passwordBox.Enabled = false;
				_passwordBox.Items.Clear();
				_passwordBox.Items.AddRange(items.ToArray());

				_passwordBox.Enabled = true;
			} finally
			{
				_updateRequired = false;
				_ignoreUpdate = false;
			}
		}

		private void SetItem(ListViewItem item, PostGreSqlPassword password)
		{
			List<ListViewItem.ListViewSubItem> values = new List<ListViewItem.ListViewSubItem>();

			values.Add(new ListViewItem.ListViewSubItem(item, password.Port.ToString()));
			values.Add(new ListViewItem.ListViewSubItem(item, password.DatabaseName));
			values.Add(new ListViewItem.ListViewSubItem(item, password.UserName));
			values.Add(new ListViewItem.ListViewSubItem(item, "########"));

			item.SubItems.Clear();
			item.SubItems.AddRange(values.ToArray());
			item.Text = password.Address;
		}

		private void _addButton_Click(object sender, EventArgs e)
		{
			PostGreSqlPassword password = new PostGreSqlPassword();
			if (!EditPassword(ref password))
			{
				return;
			}

			_passwords.Add(password);

			ListViewItem item = new ListViewItem();
			SetItem(item, password);
			_passwordBox.Items.Add(item);
		}

		private static bool EditPassword(ref PostGreSqlPassword password)
		{
			using (PostGreSqlPasswordEditor control = new PostGreSqlPasswordEditor())
			{
				PostGreSqlPassword copy = Copy(password);
				control.Password = password;
				if (!DisplayForm.Display(control, "Edit password", true, true))
				{
					return false;
				}
				password = control.Password;
			}
			return true;
		}

		private static PostGreSqlPassword Copy(PostGreSqlPassword password)
		{
			PostGreSqlPassword copy = new PostGreSqlPassword();
			copy.Address = password.Address;
			copy.DatabaseName = password.DatabaseName;
			copy.Password = password.Password;
			copy.Port = password.Port;
			copy.UserName = password.UserName;
			return copy;
		}

		private void _editButton_Click(object sender, EventArgs e)
		{
			if (_passwordBox.SelectedIndices.Count != 1)
			{
				return;
			}

			int passwordIndex = _passwordBox.SelectedIndices[0];
			PostGreSqlPassword password = _passwords[passwordIndex];
			if (!EditPassword(ref password))
			{
				return;
			}
			_passwords.RemoveAt(passwordIndex);
			_passwords.Insert(passwordIndex, password);

			ListViewItem item = _passwordBox.Items[passwordIndex];
			SetItem(item, password);
		}

		private void _removeButton_Click(object sender, EventArgs e)
		{
			if (_passwordBox.SelectedIndices.Count != 1)
			{
				return;
			}

			int passwordIndex = _passwordBox.SelectedIndices[0];
			PostGreSqlPassword password = _passwords[passwordIndex];

			if (MessageBox.Show("Remove password " + password.UserName + " " + password.DatabaseName + " " + password.Address + "?", "Confirm", MessageBoxButtons.OKCancel) != DialogResult.OK)
			{
				return;
			}

			_passwords.RemoveAt(passwordIndex);
			_passwordBox.Items.RemoveAt(passwordIndex);
		}

	}
}

