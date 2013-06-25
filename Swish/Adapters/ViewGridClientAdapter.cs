using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Swish.Controls;

namespace Swish.Adapters
{
	public class ViewGridClientAdapter: IAdapter
	{
		public const string OperationName = "ViewGridClient";
		public string Name { get { return OperationName; } }

		public string Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			Display(inputFileName);
			return inputFileName;
		}

		public static void Display(string inputFileName)
		{
			string extension = Path.GetExtension(inputFileName);
			try
			{
				using (GridView control = new GridView())
				{
					GridLayer layer = GridFunctions.Read(inputFileName);

					control.Layer = layer;
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
