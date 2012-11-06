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
			table.Headder = _table[0];
			_table.RemoveAt(0);
			table.Records = _table;

			return table;
		}

	}
}
