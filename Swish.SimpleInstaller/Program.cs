using System;
using System.IO;
using System.Windows.Forms;
using Swish.IO;
using Swish.Kepler;
using Swish.SimpleInstaller.Controls;

namespace Swish.SimpleInstaller
{
	public static class Program
	{
		[STAThread]
		public static int Main(string[] arguments)
		{
			string logFileName = string.Empty;
			try
			{

				logFileName = Path.Combine(Application.StartupPath, "Error.log.txt");
				if (FileFunctions.FileExists(logFileName))
				{
					File.Delete(logFileName);
				}

				Arguments splitArguments = ArgumentParser.Read(arguments);
				ExceptionFunctions.ForceVerbose = splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "verbose");
				ExceptionFunctions.VerboseFileOperations = false;
				bool noGui = splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "silent") || splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "nogui");
				bool clean = splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "clean");
				bool launch = splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "launch") || splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "luanch");

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
				///		save actor to installer folder
				///

				if (!noGui)
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					using (InstallerMain control = new InstallerMain())
					{
						control.Clean = clean;
						control.Launch = launch;
						Application.Run(control);
						launch = control.Launch;
					}
				} else
				{
					InstallFunctions.Install(clean, null);
				}

				if (launch)
				{
					KeplerFunctions.Run();
				}
			} catch (Exception error)
			{
				string message = ArgumentParser.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose);
				if (ExceptionFunctions.ForceVerbose)
				{
					//message += ProcessFunctions.WriteProcessHeritage();
					message += ProcessFunctions.WriteSystemVariables();
				}
				Console.Write(message);
				if (!string.IsNullOrWhiteSpace(logFileName))
				{
					File.WriteAllText(logFileName, message);
				}
				if (ExceptionFunctions.ForceVerbose)
				{
					SwishFunctions.MessageTextBox(message, false);
				}
				return -1;
			}
			return 0;
		}
	}
}
