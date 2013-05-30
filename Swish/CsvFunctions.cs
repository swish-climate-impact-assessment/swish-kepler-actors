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
			string[] _lines = File.ReadAllLines(fileName);

			List<string> lines = new List<string>(_lines);
			if (lines.Count == 0)
			{
				throw new Exception();
			}

			string line = lines[0];
			lines.RemoveAt(0);

			List<string> _headers = new List<string>(line.Split(','));
			List<string> headers = new List<string>();
			for (int headerIndex = 0; headerIndex < _headers.Count; headerIndex++)
			{
				string header = _headers[headerIndex];
				header = header.Trim('\"');
				headers.Add(header);
			}
			table.Headers = headers;

			List<List<string>> records = new List<List<string>>();
			for (int recordIndex = 0; recordIndex < lines.Count; recordIndex++)
			{
				line = lines[recordIndex];
				List<string> record = new List<string>(line.Split(','));
				records.Add(record);
			}
			table.Records = records;

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
			if (left.Headers.Count != right.Headers.Count || left.Records.Count != right.Records.Count)
			{
				return false;
			}

			for (int headerIndex = 0; headerIndex < left.Headers.Count; headerIndex++)
			{
				string leftName = left.Headers[headerIndex];
				string rightName = right.Headers[headerIndex];
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

					string leftItem = left.Headers[valueIndex];
					string rightItem = right.Headers[valueIndex];
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
			for (int columnIndex = 0; columnIndex < table.Headers.Count; columnIndex++)
			{
				string header = table.Headers[columnIndex];
				if (columnIndex + 1 < table.Headers.Count)
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

		public static List<string> UniqueValues(Csv table, int index)
		{
			List<string> longitudes = new List<string>();
			for (int rowIndex = 0; rowIndex < table.Records.Count; rowIndex++)
			{
				List<string> record = table.Records[rowIndex];
				string longitude = record[index];
				if (!longitudes.Contains(longitude))
				{
					longitudes.Add(longitude);
				}
			}

			return longitudes;
		}


	}
}
