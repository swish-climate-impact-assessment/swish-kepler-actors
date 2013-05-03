using System;
using System.IO;
using System.Windows.Forms;
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

				Arguments splitArguments = new Arguments(arguments);
				ExceptionFunctions.ForceVerbose = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "verbose");
				ExceptionFunctions.VerboseFileOperations = true;
				bool noGui = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "silent") || splitArguments.Exists(Arguments.DefaultArgumentPrefix + "nogui");
				bool clean = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "clean");
				bool luanch = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "luanch");

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
						Application.Run(control);
					}
				} else
				{
					InstallFunctions.Install(clean, null);
				}

				if (luanch)
				{
					KeplerFunctions.Run();
				}
			} catch (Exception error)
			{
				string message = Arguments.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose);
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
