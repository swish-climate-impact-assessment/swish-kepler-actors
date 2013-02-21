using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class DisplayTableClientAdapter
	{
		public const string OperationName = "displayClient";
		public string Name { get { return OperationName; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
			DisplayTable(inputFileName);
		}

		public static void DisplayTable(string inputFileName)
		{
			string extension = Path.GetExtension(inputFileName);
			string useFileName;

			try
			{
				if (extension.ToLower() == ".csv")
				{
					useFileName = inputFileName;
				} else
				{
					useFileName = FileFunctions.TempoaryOutputFileName(".csv");
				AdapterFunctions.	SaveFile(inputFileName, useFileName);
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