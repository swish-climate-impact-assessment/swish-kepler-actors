using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Swish.SimpleInstaller.Controls;
using System.IO;

namespace Swish.SimpleInstaller
{
	static class Program
	{
		[STAThread]
		static int Main(string[] arguments)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
			bool silent = ArgumentFunctions.GetSwitch(ArgumentFunctions.ArgumentCharacter + "silent", splitArguments);
			bool clean = ArgumentFunctions.GetSwitch(ArgumentFunctions.ArgumentCharacter + "clean", splitArguments);

			string logFileName = Path.Combine(Application.StartupPath, "Error.log.txt");
			if (File.Exists(logFileName))
			{
				File.Delete(logFileName);
			}
			/// adding actor steps
			/// Adapter
			///		code
			///		test
			/// 
			/// Actor
			///		ports
			///			names
			///			cmd arguments
			///			connect up
			///		save actor to installer foldrr
			///		
			///	Installer
			///		add actor to project
			///		set copy allways
			/// 
			/// Build events
			///		Exe redirector
			///			pre build delete
			///			post build copy
			///		simple installer
			///			post build copy
			/// 
			/// Development locations 
			/// 

			try
			{
				if (!silent)
				{
					using (InstallerMain control = new InstallerMain())
					{
						control.Clean = clean;
						Application.Run(control);
					}
				} else
				{
					InstallFunctions.Install(clean, null);
					return 0;

				}

			} catch (Exception error)
			{
				string message = ArgumentFunctions.ErrorArgument + " " + ExceptionFunctions.WriteException(error, true);
				Console.Write(message);
				File.WriteAllText(logFileName, message);
				return -1;
			}
			return 0;
		}
	}
}
