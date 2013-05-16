using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Swish.Adapters
{
	public class GraphClientAdapter: IAdapter
	{
		public const string OperationName = "GraphClient";
		public string Name { get { return OperationName; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			List<string> variableNames = splitArguments.VariableNames();
			Display(inputFileName, variableNames);
			Console.WriteLine(inputFileName);
		}

		public static void Display(string inputFileName, List<string> variables)
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
					SaveTableAdapter.Save(inputFileName, useFileName);
				}

				Csv table = CsvFunctions.Read(useFileName);

				using (OhlcvsEditor control = new OhlcvsEditor())
				{
					for (int variableIndex = 0; variableIndex < variables.Count; variableIndex++)
					{
						string variableName = variables[variableIndex];

						int columnIndex = table.ColumnIndex(variableName, true);

						GraphData item = new GraphData();
						item.Values = table.ColunmAsDoubles(columnIndex);
						control.Data.Add(item);
					}

					DisplayForm.Display(control, "Data", true, false);
				}


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
