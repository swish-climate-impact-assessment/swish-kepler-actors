using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Swish.Adapters
{
	public class DisplayArgumentsAdapter: IOperation
	{
		public string Name { get { return "test"; } }

		public string Run(OperationArguments splitArguments)
		{
			bool silent = splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "silent");

			List<string> lines = new List<string>();
			lines.Add("Arguments: " + splitArguments.ArgumentString);
			lines.Add("Startup path: " + Application.StartupPath);
			lines.Add("Working directory: " + Environment.CurrentDirectory);

			if (ProcessFunctions.KeplerProcess != null)
			{
				lines.Add("Keper process:");
				lines.Add(ProcessFunctions.WriteProcessInformation(ProcessFunctions.KeplerProcess));
			} else
			{
				lines.Add("Keper process: Not found");
			}

			lines.Add("Current process heritage: ");

			lines.AddRange(ProcessFunctions.WriteProcessHeritage().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
			lines.AddRange(ProcessFunctions.WriteSystemVariables().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
			if (!silent)
			{
				SwishFunctions.MessageTextBox("Test display", lines, false);
			}
			string output = string.Join(Environment.NewLine, lines);
			return output;
		}
	}
}
