
using System.Collections.Generic;
using System;
namespace Swish
{
	public class Csv
	{
		private List<string> _headder = new List<string>();
		private List<List<string>> _records = new List<List<string>>();

		public List<string> Headers
		{
			get { return _headder; }
			set
			{
				if (value == null)
				{
					_headder = new List<string>();
					return;
				}
				_headder = new List<string>(value);
			}
		}

		public List<List<string>> Records
		{
			get { return _records; }
			set
			{
				if (value == null)
				{
					_records = new List<List<string>>();
					return;
				}
				_records = new List<List<string>>(value);
			}
		}

		public int ColumnIndex(string variableName, bool throwOnMissing)
		{
			int headderIndex = -1;
			for (int columnIndex = 0; columnIndex < Headers.Count; columnIndex++)
			{
				string _name = Headers[columnIndex];
				if (_name == variableName)
				{
					headderIndex = columnIndex;
					break;
				}
			}
			if (throwOnMissing && headderIndex < 0)
			{
				throw new Exception("could not find variable \"" + variableName + "\"");
			}
			return headderIndex;
		}

		public List<int> ColunmAsInts(int columnIndex)
		{
			List<int> values = new List<int>();
			for (int recordIndex = 0; recordIndex < Records.Count; recordIndex++)
			{
				string valueString = Records[recordIndex][columnIndex];
				int value = int.Parse(valueString);
				values.Add(value);
			}
			return values;
		}

		public List<string> ColunmValues(int columnIndex)
		{
			List<string> values = new List<string>();
			for (int recordIndex = 0; recordIndex < Records.Count; recordIndex++)
			{
				string valueString = Records[recordIndex][columnIndex];
				values.Add(valueString);
			}
			return values;
		}

		public List<double> ColunmAsDoubles(int columnIndex)
		{
			List<double> values = new List<double>();
			for (int recordIndex = 0; recordIndex < Records.Count; recordIndex++)
			{
				string valueString = Records[recordIndex][columnIndex];
				double value = double.Parse(valueString);
				values.Add(value);
			}
			return values;
		}

		internal bool ColumnExists(string variableName)
		{
			return ColumnIndex(variableName, false) >= 0;
		}

		internal void Remove(string variableName)
		{
			int columnIndex = ColumnIndex(variableName, false);
			if (columnIndex <= 0)
			{
				return;
			}

			_headder.RemoveAt(columnIndex);
			for (int recordIndex = 0; recordIndex < _records.Count; recordIndex++)
			{
				List<string> record = _records[recordIndex];
				record.RemoveAt(columnIndex);
			}
		}

		internal void Add(string variableName, List<int> values)
		{
			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("");
			}
			_headder.Add(variableName);
			if (values == null || values.Count != _records.Count)
			{
				throw new Exception("");
			}

			for (int recordIndex = 0; recordIndex < _records.Count; recordIndex++)
			{
				List<string> record = _records[recordIndex];
				int value = values[recordIndex];

				record.Add(value.ToString());
			}
		}

		public void Remove(int columnIndex)
		{
			Headers.RemoveAt(columnIndex);
			for (int rowIndex = 0; rowIndex < Records.Count; rowIndex++)
			{
				List<string> record = Records[rowIndex];
				record.RemoveAt(columnIndex);
			}
		}
	}

}
