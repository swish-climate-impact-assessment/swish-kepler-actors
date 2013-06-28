using System;
using System.Collections.Generic;

namespace Swish.IO
{
	public class GridLayer
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

		public double NullValue { get; set; }

		internal void CopyMetadata(GridLayer layer)
		{
			_latitudes = new List<string>(layer.Latitudes);
			_longitudes = new List<string>(layer.Longitudes);
			NullValue = layer.NullValue;
		}

		public double ValueByIndex(int longitudeIndex, int latitudeIndex)
		{
			if (longitudeIndex < 0 || latitudeIndex < 0 || longitudeIndex >= _longitudes.Count || latitudeIndex >= _latitudes.Count)
			{
				throw new ArgumentOutOfRangeException("longitudeIndex, latitudeIndex");
			}

			double value = _values[latitudeIndex * _longitudes.Count + longitudeIndex];
			return value;
		}

		public void ValueRange(out double minimumValue, out double maximumValue)
		{

			minimumValue = double.MaxValue;
			maximumValue = double.MinValue;

			for (int valueIndex = 0; valueIndex < _values.Count; valueIndex++)
			{
				double value = _values[valueIndex];
				if (value < minimumValue)
				{
					minimumValue = value;
				}
				if (value > maximumValue)
				{
					maximumValue = value;
				}
			}

			if (minimumValue == double.MaxValue)
			{
				minimumValue = NullValue;
			}
			if (maximumValue == double.MinValue)
			{
				maximumValue = NullValue;
			}
		}
	}
}
