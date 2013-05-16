using System.Collections.Generic;

namespace Swish.Tests
{
	class GridLayer
	{

		private List<string> _latitudes = new List<string>();
		public List<string> Latitudes
		{
			get { return _latitudes; }
			set
			{
				if (value == null || value.Count == 0)
				{
					_latitudes = new List<string>();
					return;
				}
				_latitudes = new List<string>(value);
			}
		}

		private List<string> _longitudes = new List<string>();
		public List<string> Longitudes
		{
			get { return _longitudes; }
			set
			{
				if (value == null || value.Count == 0)
				{
					Longitudes = new List<string>();
					return;
				}
				_longitudes = new List<string>(value);
			}
		}

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

		public int NullValue { get; set; }

		internal void CopyMetadata(GridLayer layer)
		{
			_latitudes = new List<string>(layer.Latitudes);
			_longitudes = new List<string>(layer.Longitudes);
			NullValue = layer.NullValue;
		}
	}
}
