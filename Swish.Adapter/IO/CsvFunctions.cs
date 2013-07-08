using System.Collections.Generic;
using System;
using System.IO;

namespace Swish.IO
{
	public static class CsvFunctions
	{
		public static Table Read(string fileName)
		{
			Table table = new Table();
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

		public static void Write(string fileName, Table table)
		{
			List<string> lines = new List<string>();

			string line = string.Empty;
			for (int columnIndex = 0; columnIndex < table.Headers.Count; columnIndex++)
			{
				string header = table.Headers[columnIndex];
				if (columnIndex + 1 < table.Headers.Count)
				{
					line += header.ToLower() + ",";
				} else
				{
					line += header.ToLower();
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

		public static List<string> UniqueValues(Table table, int index)
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
