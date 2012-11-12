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

			if (clean)
			{
				string logFileName = Path.Combine(Application.StartupPath, "Error.log.txt");
				throw new Exception();


				/// adding actor process
				/// 
				/// 
				/// Actor
				///		save actor to installer foldrr
				///		add actor to project
				/// 
				/// Build events
				///		Exe redirector
				///		simple installerredirector
				/// 
				/// Development locations
				/// 

				try
				{
					if (File.Exists(logFileName))
					{
						File.Delete(logFileName);
					}
					InstallFunctions.Install(null);
				} catch (Exception error)
				{
					string message = ArgumentFunctions.ErrorArgument + " " + ExceptionFunctions.WriteException(error, true);
					Console.Write(message);
					File.WriteAllText(logFileName, message);
					return -1;
				}
				return 0;
			}

			if (silent)
			{
				string logFileName = Path.Combine(Application.StartupPath, "Error.log.txt");
				try
				{
					if (File.Exists(logFileName))
					{
						File.Delete(logFileName);
					}
					InstallFunctions.Install(null);
				} catch (Exception error)
				{
					string message = ArgumentFunctions.ErrorArgument + " " + ExceptionFunctions.WriteException(error, true);
					Console.Write(message);
					File.WriteAllText(logFileName, message);
					return -1;
				}
				return 0;
			}

			Application.Run(new InstallerMain());
			return 0;
		}
	}
}
