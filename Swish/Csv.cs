
using System.Collections.Generic;
namespace Swish
{
	public class Csv
	{
		private List<string> _headder = new List<string>();
		private List<List<string>> _records = new List<List<string>>();

		public List<string> Header
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


		public int ColumnIndex(string variableName)
		{
			int headderIndex = -1;
			for (int columnIndex = 0; columnIndex < Header.Count; columnIndex++)
			{
				string _name = Header[columnIndex];
				if (_name == variableName)
				{
					headderIndex = columnIndex;
					break;
				}
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
	}

}
