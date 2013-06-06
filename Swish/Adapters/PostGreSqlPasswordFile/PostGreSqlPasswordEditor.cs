using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Swish.Controls;

namespace Swish.Adapters.PostGreSqlPasswordFile
{
	internal partial class PostGreSqlPasswordEditor: UserControl
	{
		public PostGreSqlPasswordEditor()
		{
			InitializeComponent();
		}

		public PostGreSqlPassword Password{get;set;}

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

				MessageBox.Show("not done yet");
			} finally
			{
				_updateRequired = false;
				_ignoreUpdate = false;
			}
		}

	}
}

