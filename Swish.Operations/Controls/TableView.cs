using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace Swish.Controls
{
	internal partial class TableView: UserControl
	{
		public TableView()
		{
			InitializeComponent();
		}

		private void Parent_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void TableView_VisibleChanged(object sender, EventArgs e)
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

		private void TableView_Load(object sender, EventArgs e)
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
				RangeStartBar.Value = 0;
				if (_data.Count > 0)
				{
					RangeEndBar.Maximum = _data[0].Values.Count;
					RangeEndBar.Value = _data[0].Values.Count;
				} else
				{
					RangeEndBar.Value = 0;
				}

				PopulateGraph();
			} finally
			{
				_updateRequired = false;
				_ignoreUpdate = false;
			}
		}

		private void PopulateGraph()
		{
			if (GraphBox.Width == 0 || GraphBox.Height == 0)
			{
				return;
			}

			Image _image = GraphBox.Image;
			GraphBox.Image = null;
			if (_image != null)
			{
				using (_image) { }
			}

			Bitmap image = new Bitmap(GraphBox.Width, GraphBox.Height);
			using (Graphics graphics = Graphics.FromImage(image))
			{
				graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, GraphBox.Width, GraphBox.Height);
			}

			Random random = new Random();
			string title = string.Empty;
			for (int dataIndex = 0; dataIndex < _data.Count; dataIndex++)
			{
				GraphData dataItem = _data[dataIndex];

				if (dataItem.Colour == null)
				{
					dataItem.Colour = PickColour(random);
				}

				Color colour = dataItem.Colour.Item2;

				List<double> values = dataItem.Values;
				GraphFunctions.DrawLines(values, RangeStartBar.Value, RangeEndBar.Value - RangeStartBar.Value, colour, image);

				title += dataItem.Colour.Item1 + ": ";
				if (!string.IsNullOrWhiteSpace(dataItem.Name))
				{
					title += dataItem.Name;
				} else
				{
					title += "Unknown";
				}
				if (dataIndex + 1 < _data.Count)
				{
					title += ", ";
				}
			}

			_nameBox.Text = title;

			GraphBox.Image = image;

			RangeStartBar.Minimum = 0;
			int count;
			if (_data.Count > 0)
			{
				count = _data[0].Values.Count - 1;
			} else
			{
				count = 0;
			}
			RangeStartBar.Maximum = count;
			RangeStartBar.Value = Math.Min(RangeStartBar.Value, count);

			RangeEndBar.Minimum = 0;
			if (_data.Count > 0)
			{
				count = _data[0].Values.Count - 1;
			} else
			{
				count = 0;
			}
			RangeEndBar.Maximum = count;
			RangeEndBar.Value = Math.Min(RangeEndBar.Value, count);

			if (_data.Count == 1)
			{
				List<double> averageValues = Average20Days(_data[0].Values);
			}

			if (_data.Count > 0)
			{

				double yMinimum;
				double yMaximum;
				GraphFunctions.ValueRange(out yMinimum, out yMaximum, _data[0].Values, RangeStartBar.Value, RangeEndBar.Value - RangeStartBar.Value);

				XMinimumBox.Text = (RangeStartBar.Value + 1).ToString();
				XMaximum.Text = (RangeEndBar.Value + 1).ToString();
				YMinimumBox.Text = yMinimum.ToString("F2").Trim('0').Trim('.');
				YMaximum.Text = yMaximum.ToString("F2").Trim('0').Trim('.');
			} else
			{
				XMinimumBox.Text = string.Empty;
				XMaximum.Text = string.Empty;
				YMinimumBox.Text = string.Empty;
				YMaximum.Text = string.Empty;
			}

		}

		private List<Tuple<string, Color>> _colours;

		private Tuple<string, Color> PickColour(Random random)
		{
			if (_colours == null)
			{
				_colours = new List<Tuple<string, Color>>();


				PropertyInfo[] properties = typeof(Color).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
				for (int colourIndex = 0; colourIndex < properties.Length; colourIndex++)
				{
					PropertyInfo property = properties[colourIndex];
					if (property.CanWrite || property.PropertyType != typeof(Color))
					{
						continue;
					}

					string name = property.Name;

					Color colour = (Color)property.GetValue(null, null);
					_colours.Add(new Tuple<string, Color>(name, colour));
				}
			}

			int index = random.Next(_colours.Count);

			Tuple<string, Color> colourChoice = _colours[index];
			return colourChoice;
		}

		private List<double> Average20Days(List<double> values)
		{
			List<double> averages = new List<double>();
			for (int valueIndex = 0; valueIndex < values.Count; valueIndex++)
			{
				int startIndex = Math.Max(0, valueIndex - 20 + 1);
				int count = valueIndex - startIndex + 1;
				double sum = 0;
				while (startIndex <= valueIndex)
				{
					sum += values[startIndex];
					startIndex++;
				}
				double average = sum / count;
				averages.Add(average);
			}

			return averages;
		}

		private void TableView_Resize(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			PopulateGraph();
		}

		private void RangeStartBar_Scroll(object sender, EventArgs e)
		{
			if (_ignoreUpdate || _data.Count == 0)
			{
				return;
			}

			_ignoreUpdate = true;
			try
			{
				if (RangeEndBar.Value < RangeStartBar.Value)
				{
					RangeEndBar.Value = RangeStartBar.Value;
				}

				PopulateGraph();
			} finally
			{
				_ignoreUpdate = false;
			}
		}

		private void RangeEndBar_Scroll(object sender, EventArgs e)
		{
			if (_ignoreUpdate || _data.Count == 0)
			{
				return;
			}

			_ignoreUpdate = true;
			try
			{
				if (RangeStartBar.Value > RangeEndBar.Value)
				{
					RangeStartBar.Value = RangeEndBar.Value;
				}

				PopulateGraph();
			} finally
			{
				_ignoreUpdate = false;
			}
		}

		private List<GraphData> _data = new List<GraphData>();
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public List<GraphData> Data
		{
			get { return _data; }
			set
			{
				if (value == null || value.Count == 0)
				{
					_data = new List<GraphData>();
					return;
				}
				_data = new List<GraphData>(value);
			}
		}


	}
}

