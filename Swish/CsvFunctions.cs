using System.Collections.Generic;
using System;
using System.IO;

namespace Swish
{
	public static class CsvFunctions
	{
		public static Csv Read(string fileName)
		{
			Csv table = new Csv();
			string[] output = File.ReadAllLines(fileName);
			List<List<string>> _table = new List<List<string>>();
			for (int lineIndex = 0; lineIndex < output.Length; lineIndex++)
			{
				string line = output[lineIndex];
				List<string> items = new List<string>(line.Split(','));

				_table.Add(items);
			}

			if (_table.Count == 0)
			{
				return table;
			}
			table.Header = _table[0];
			_table.RemoveAt(0);
			table.Records = _table;

			return table;
		}

		public static bool Equal(Csv left, Csv right)
		{
			if (left == null)
			{
				if (right == null)
				{
					return true;
				}
				return false;
			}
			if (right == null)
			{
				return false;
			}
			if (left.Header.Count != right.Header.Count || left.Records.Count != right.Records.Count)
			{
				return false;
			}

			for (int headerIndex = 0; headerIndex < left.Header.Count; headerIndex++)
			{
				string leftName = left.Header[headerIndex];
				string rightName = right.Header[headerIndex];
				if (leftName != rightName)
				{
					return false;
				}
			}

			for (int recordIndex = 0; recordIndex < left.Records.Count; recordIndex++)
			{
				List<string> leftRecord = left.Records[recordIndex];
				List<string> rightRecord = right.Records[recordIndex];
				for (int valueIndex = 0; valueIndex < leftRecord.Count; valueIndex++)
				{

					string leftItem = left.Header[valueIndex];
					string rightItem = right.Header[valueIndex];
					if (leftItem != rightItem)
					{
						return false;
					}
				}
			}

			return true;
		}

		public static void Write(string fileName, Csv table)
		{
			List<string> lines = new List<string>();

			string line = string.Empty;
			for (int columnIndex = 0; columnIndex < table.Header.Count; columnIndex++)
			{
				string header = table.Header[columnIndex];
				if (columnIndex + 1 < table.Header.Count)
				{
					line += header + ",";
				} else
				{
					line += header;
				}
			}
			if (!string.IsNullOrWhiteSpace(line))
			{
				lines.Add(line);
			}

			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				line = string.Empty;
				for (int columnIndex = 0; columnIndex < record.Count; columnIndex++)
				{
					string value = record[columnIndex];
					if (columnIndex + 1 < record.Count)
					{
						line += value + ",";
					} else
					{
						line += value;
					}
				}
				lines.Add(line);
			}

			File.WriteAllLines(fileName, lines);
		}
	}
}
