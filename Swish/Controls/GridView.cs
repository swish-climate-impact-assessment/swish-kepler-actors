using System;
using System.Drawing;
using System.Windows.Forms;

namespace Swish.Controls
{
	public partial class GridView: UserControl
	{
		public GridView()
		{
			InitializeComponent();
		}

		private Form _parentForm;

		private void GridView_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_parentForm.DialogResult != DialogResult.OK)
			{
				_parentForm.DialogResult = DialogResult.Cancel;
			}

			//if (!)
			//{
			//	e.Cancel = true;
			//}
		}

		private void GridView_KeyUp(object sender, KeyEventArgs e)
		{
			if ((Keys)e.KeyValue == Keys.Escape)
			{
				_parentForm.Close();
				e.Handled = true;
			}
		}

		private void GridView_Load(object sender, EventArgs e)
		{
			_parentForm = DisplayForm.GetForm(sender);
			if (_parentForm != null)
			{
				_parentForm.KeyPreview = true;
				_parentForm.KeyUp += GridView_KeyUp;
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
			if (!_updateRequired)
			{
				return;
			}

			_ignoreUpdate = true;

			try
			{
				Image image = PictureBox.Image;
				PictureBox.Image = null;
				using (image) { }

				if (Layer != null)
				{
					image = GenerateImage(Layer, PictureBox.Width, PictureBox.Height);
					if (Layer.Longitudes.Count > 0)
					{
						XMinimumBox.Text = Layer.Longitudes[0];
						XMaximum.Text = Layer.Longitudes[Layer.Longitudes.Count - 1];
					} else
					{
						XMinimumBox.Text = string.Empty;
						XMaximum.Text = string.Empty;
					}
					if (Layer.Latitudes.Count > 0)
					{
						YMinimumBox.Text = Layer.Latitudes[0];
						YMaximum.Text = Layer.Latitudes[Layer.Latitudes.Count - 1];
					} else
					{
						YMinimumBox.Text = string.Empty;
						YMaximum.Text = string.Empty;
					}
				} else
				{
					image = null;
					XMinimumBox.Text = string.Empty;
					XMaximum.Text = string.Empty;
					YMinimumBox.Text = string.Empty;
					YMaximum.Text = string.Empty;
				}
				PictureBox.Image = image;
			} finally
			{
				_updateRequired = false;
				_ignoreUpdate = false;
			}
		}

		private Image GenerateImage(GridLayer layer, int width, int height)
		{
			Bitmap image = new Bitmap(width, height);
			double minimumValue;
			double maximumValue;
			layer.ValueRange(out minimumValue, out maximumValue);
			double range = maximumValue - minimumValue;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int longitudeIndex = Index(x, width, layer.Longitudes.Count);
					int latitudeIndex = Index(height - 1 - y, height, layer.Latitudes.Count);

					double value = layer.ValueByIndex(longitudeIndex, latitudeIndex);
					int intValue = (int)(0xff * (value - minimumValue) / range);
					Color colour = Color.FromArgb(0xff, intValue, intValue, intValue);
					image.SetPixel(x, y, colour);
				}
			}

			return image;
		}

		private int Index(int index, int count, int newCount)
		{
			return (int)(index / (count - 1.0) * (newCount - 1.0));
		}

		public GridLayer Layer { get; set; }

		private void GridView_Resize(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			_updateRequired = true;
			PopulateForm();
		}
	}
}

