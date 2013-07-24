using System;
using System.Collections.Generic;
using System.Drawing;

namespace Swish
{
	public class GraphData
	{
		private List<double> _values = new List<double>();
		public List<double> Values
		{
			get { return _values; }
			set
			{
				if (value == null || value.Count == 0)
				{
					_values = new List<double>();
					return;
				}
				_values = new List<double>(value);
			}
		}


		public string _name = string.Empty;
		public string Name
		{
			get { return _name; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_name = string.Empty;
					return;
				}
				_name = value;
			}
		}

		public Tuple<string, Color> Colour { get; set; }
	}

}