using System;
using System.IO;
using Swish.IO;

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

				string[] lines = File.ReadAllLines(useFileName);
				for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
				{
					lines[lineIndex] = lines[lineIndex].Replace(',', '\t');
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
