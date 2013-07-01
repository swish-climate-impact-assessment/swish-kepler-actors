using System;
using System.IO;
using Swish.IO;
using System.Collections.Generic;

namespace Swish.Adapters
{
	public class DisplayTableClientAdapter: IOperation
	{
		public const string OperationName = "displayClient";
		public string Name { get { return OperationName; } }

		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			DisplayTable(inputFileName);
			return inputFileName;
		}

		public static void DisplayTable(string inputFileName)
		{
			string extension = Path.GetExtension(inputFileName);
			string useFileName;

			try
			{
				if (extension.ToLower() == SwishFunctions.CsvFileExtension)
				{
					useFileName = inputFileName;
				} else
				{
					useFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
					SwishFunctions.Save(inputFileName, useFileName);
				}

				Table table = CsvFunctions.Read(useFileName);

				int recordCount = Math.Min(table.Records.Count, 1000);
				int[] columnCharacterWidth = new int[table.Headers.Count];
				for (int columnIndex = 0; columnIndex < table.Headers.Count; columnIndex++)
				{
					int recordSize = table.Headers[columnIndex].Length;
					if (recordSize > columnCharacterWidth[columnIndex])
					{
						columnCharacterWidth[columnIndex] = recordSize;
					}
				}

				for (int recordIndex = 0; recordIndex < recordCount; recordIndex++)
				{
					List<string> record = table.Records[recordIndex];

					for (int columnIndex = 0; columnIndex < table.Headers.Count; columnIndex++)
					{
						int recordSize = record[columnIndex].Length;
						if (recordSize > columnCharacterWidth[columnIndex])
						{
							columnCharacterWidth[columnIndex] = recordSize;
						}
					}
				}

				List<string> lines = new List<string>(recordCount);
				{
					string line = string.Empty;
					for (int columnIndex = 0; columnIndex < table.Headers.Count; columnIndex++)
					{
						line += table.Headers[columnIndex].PadRight(columnCharacterWidth[columnIndex]);
						if (columnIndex + 1 < table.Headers.Count)
						{
							line += "\t";
						}
					}
					lines.Add(line);
				}

				for (int recordIndex = 0; recordIndex < recordCount; recordIndex++)
				{
					List<string> record = table.Records[recordIndex];

					string line = string.Empty;
					for (int columnIndex = 0; columnIndex < table.Headers.Count; columnIndex++)
					{
						line += record[columnIndex].PadRight(columnCharacterWidth[columnIndex]);
						if (columnIndex + 1 < table.Headers.Count)
						{
							line += "\t";
						}
					}
					lines.Add(line);
				}

				SwishFunctions.MessageTextBox("File: " + inputFileName + Environment.NewLine, lines, false);
			} catch (Exception error)
			{
				string message = "failed to display " + inputFileName + Environment.NewLine + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose);
				//if (ExceptionFunctions.ForceVerbose)
				//{
				//    message += ProcessFunctions.WriteProcessHeritage();
				//    message += ProcessFunctions.WriteSystemVariables();
				//}

				SwishFunctions.MessageTextBox(message, false);
			}

		}


	}
}
