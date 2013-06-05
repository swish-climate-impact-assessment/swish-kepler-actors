using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Swish.Controls;

namespace Swish.Adapters
{
	public class GraphSeriesClientAdapter: IAdapter
	{
		public const string OperationName = "GraphSeriesClient";
		public string Name { get { return OperationName; } }

		public string Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			List<string> variableNames = splitArguments.VariableNames();
			Display(inputFileName, variableNames);
			return inputFileName;
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

				using (TableView control = new TableView())
				{
					for (int variableIndex = 0; variableIndex < variables.Count; variableIndex++)
					{
						string variableName = variables[variableIndex];

						int columnIndex = table.ColumnIndex(variableName, true);

						GraphData item = new GraphData();
						item.Values = table.ColunmAsDoubles(columnIndex);
						item.Name = variableName;
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
