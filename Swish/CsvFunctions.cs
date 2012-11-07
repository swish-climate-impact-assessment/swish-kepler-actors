using System.Collections.Generic;
using System;
using System.IO;

namespace Swish
{
	public static class CsvFunctions
	{
		public static Csv Read(string outputFileName)
		{
			Csv table = new Csv();
			List<string> output = new List<string>(File.ReadAllLines(outputFileName));
			List<List<string>> _table = new List<List<string>>();
			for (int lineIndex = 0; lineIndex < output.Count; lineIndex++)
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
			if (left  == null)
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

			for (int headerIndex = 0;  headerIndex < left.Header.Count;  headerIndex++)
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
	}
}
