using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Swish
{
	internal partial class OhlcvsEditor: UserControl
	{
		public OhlcvsEditor()
		{
			InitializeComponent();
		}

		private void Parent_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void OhlcvsEditor_VisibleChanged(object sender, EventArgs e)
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

		private void OhlcvsEditor_Load(object sender, EventArgs e)
		{
			Form form = DisplayForm.GetForm(this);
			if (form != null)
			{
				form.FormClosed += new FormClosedEventHandler(Parent_FormClosed);
			}
			//check point
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
				PopulateValues();
				PopulateGraph();
				Populate20DayAverageGraph();
			} finally
			{
				BalancesBox.Visible = true;
				_updateRequired = false;
				_ignoreUpdate = false;
			}
		}

		private void PopulateValues()
		{
			BalancesBox.Visible = false;
			BalancesBox.Items.Clear();
			BalancesBox.Columns.Clear();
			BalancesBox.Groups.Clear();

			int valueCount;
			if (_data.Count > 0)
			{
				valueCount = _data[0].Values.Count;
			} else
			{
				valueCount = 0;
			}
			for (int dataIndex = 0; dataIndex < _data.Count; dataIndex++)
			{
				GraphData item = _data[dataIndex];
				BalancesBox.Columns.Add(item.Name, (BalancesBox.Width - 1) / _data.Count);
			}

			BalancesBox.Groups.Add("", "");

			List<ListViewItem> items = new List<ListViewItem>();
			for (int valueIndex = 0; valueIndex < valueCount; valueIndex++)
			{


				List<string> valueStrings = new List<string>();
				for (int dataIndex = 0; dataIndex < _data.Count; dataIndex++)
				{
					GraphData dataItem = _data[dataIndex];
					double value = dataItem.Values[valueIndex];
					valueStrings.Add(value.ToString());
				}

				ListViewItem listItem = new ListViewItem(valueStrings.ToArray(), BalancesBox.Groups[""]);
				items.Add(listItem);
			}

			BalancesBox.ListViewItemSorter = new Sorter(0, false);
			BalancesBox.Sorting = SortOrder.Ascending;
			BalancesBox.Items.AddRange(items.ToArray());

			for (int index = 0; index < BalancesBox.Columns.Count; index++)
			{
				BalancesBox.AutoResizeColumn(index, ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}

		private sealed class Sorter: IComparer
		{
			private bool _decending;
			internal bool Decending { get { return _decending; } }

			private int _column;
			internal int Column { get { return _column; } }

			internal Sorter(int coloumn, bool decending)
			{
				_decending = decending;
				_column = coloumn;
			}

			int IComparer.Compare(object x, object y)
			{
				ListViewItem left;
				ListViewItem right;

				if (!_decending)
				{
					left = (ListViewItem)x;
					right = (ListViewItem)y;
				} else
				{
					left = (ListViewItem)y;
					right = (ListViewItem)x;
				}

				return string.Compare(left.Text, right.Text);
			}
		}

		private void BalancesBox_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}

			if (BalancesBox.ListViewItemSorter == null)
			{
				BalancesBox.ListViewItemSorter = new Sorter(0, false);
				return;
			}

			Sorter current = BalancesBox.ListViewItemSorter as Sorter;
			if (current.Column == e.Column)
			{
				BalancesBox.ListViewItemSorter = new Sorter(e.Column, !current.Decending);
				return;
			}

			BalancesBox.ListViewItemSorter = new Sorter(e.Column, false);
		}

		private void PopulateGraph()
		{
			if (GraphBox.Width == 0 || GraphBox.Height == 0)
			{
				return;
			}

			Bitmap image = new Bitmap(GraphBox.Width, GraphBox.Height);
			for (int dataIndex = 0; dataIndex < _data.Count; dataIndex++)
			{
				GraphData dataItem = _data[ dataIndex];

				List<double> values = dataItem.Values;
				GraphFunctions.DrawLines(values, image);
			}

			GraphBox.Image = image;
		}

		private void OhlcvsEditor_Resize(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			PopulateGraph();
			Populate20DayAverageGraph();
		}

		private int _minimumDate;
		private int _maximumDate;

		private void Populate20DayAverageGraph()
		{
			if (AverageBox.Width == 0 || AverageBox.Height == 0 || _data.Count <= 1)
			{
				return;
			}

			/*
			int valueCount = _data[0].Values.Count;
			int minimumDate = 0;
			int maximumDate = valueCount - 1;

			if (_minimumDate < minimumDate || _minimumDate > maximumDate)
			{
				_minimumDate = minimumDate;
				MinimumDateBar.Value = _minimumDate;
			}
			if (_maximumDate < _minimumDate)
			{
				_maximumDate = minimumDate;
				MaximumDateBar.Value = _maximumDate;
			} else if (_maximumDate < minimumDate || _maximumDate > maximumDate)
			{
				_maximumDate = maximumDate;
				MaximumDateBar.Value = _maximumDate;
			}

			Bitmap image = new Bitmap(AverageBox.Width, AverageBox.Height);
			if (data.Count > 0)
			{

				for (int dataIndex = 0; dataIndex < _data.Count; dataIndex++)
				{
					GraphData dataItem = _data[ dataIndex];


				}

				DoubleSeries openingPrices = TradeFunctions.GetPrices(data, false);
				DoubleSeries _averages = MoveAboveAverage20DaySignal.Average(openingPrices, 20);
				List<double> zones = ProfitFunctions.CalculateZones(openingPrices, _averages);

				List<double> values = ToDoubleList(openingPrices);
				List<double> averages = ToDoubleList(_averages);


				double yMinimum;
				double yMaximum;
				GraphFunctions.ValueRange(out yMinimum, out yMaximum, values);

				//GraphFunctions.DrawZones(zones, image);
				GraphFunctions.DrawLines(values, image, yMinimum, yMaximum);
				GraphFunctions.DrawLines(averages, image, yMinimum, yMaximum);
			}

			AverageBox.Image = image;
			 */
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_ignoreUpdate)
			{
				return;
			}
			PopulateGraph();
			Populate20DayAverageGraph();
		}

		private void MinimumBar_Scroll(object sender, EventArgs e)
		{
			if (_ignoreUpdate || _data.Count == 0)
			{
				return;
			}


			int valueCount = _data[0].Values.Count;

			//int minimumDate = 0;//_prices[0].Date;
			int maximumDate = valueCount - 1;//_prices[_prices.Count - 1].Date;

			_minimumDate = (int)(MinimumDateBar.Value);

			if (_maximumDate < _minimumDate)
			{
				_maximumDate = _minimumDate;
				MaximumDateBar.Value = _maximumDate;
			}

			Populate20DayAverageGraph();
		}

		private void MaximumDateBar_Scroll(object sender, EventArgs e)
		{
			if (_ignoreUpdate || _data.Count == 0)
			{
				return;
			}

			int valueCount = _data[0].Values.Count;

			//int minimumDate = 0;//_prices[0].Date;
			int maximumDate = valueCount - 1;//_prices[_prices.Count - 1].Date;

			_maximumDate = MaximumDateBar.Value;

			if (_minimumDate > _maximumDate)
			{
				_minimumDate = _maximumDate;
				MinimumDateBar.Value = _minimumDate;
			}

			Populate20DayAverageGraph();
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

