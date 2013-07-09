using System;
using System.Collections.Generic;
using Swish.IO;

namespace Swish.Adapters
{
	public class SplitByVariableAdapter: IOperation
	{
		public string Name { get { return "SplitByVariable"; } }

		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string variableName = splitArguments.VariableName();

			List<string> fileNames = Split(inputFileName, variableName);
			string output = string.Join(Environment.NewLine, fileNames);
			return output;
		}

		public static List<string> Split(string inputFileName, string variableName)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("Variable missing");
			}

			string useInput = SwishFunctions.ConvertToCsvFile(inputFileName);
			Table table = CsvFunctions.Read(useInput, true);

			int columnIndex = table.ColumnIndex(variableName, true);

			SortedList<string, List<List<string>>> splitValues = new SortedList<string, List<List<string>>>();
			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				string value = record[columnIndex];

				List<List<string>> records;
				if (splitValues.ContainsKey(value))
				{
					records = splitValues[value];
				} else
				{
					records = new List<List<string>>();
					splitValues.Add(value, records);
				}
				records.Add(record);
			}

			List<string> fileNames = new List<string>();
			for (int splitIndex = 0; splitIndex < splitValues.Count; splitIndex++)
			{
				List<List<string>> records = splitValues.Values[splitIndex];

				Table splitTable = new Table();
				splitTable.Headers = table.Headers;
				splitTable.Records = records;

				string fileName = FileFunctions.TempoaryOutputFileName(".csv");
				CsvFunctions.Write(fileName, splitTable);
				fileNames.Add(fileName);
			}

			return fileNames;
		}

	}
}
