using System;
using System.Diagnostics;
using LibraryTypes.BootStrap;

namespace Swish.Tests
{
	public class ProcessorFunctionsTests
	{

		internal void ReceiveOutput()
		{
			string expectedOutput = "this is the output";
			string toolFileName = CSharpCompiler.MakeExecutable("class Program{static void Main(string[] arguments){System.Console.WriteLine(\"" + expectedOutput + "\"); }}");

			ProcessResult result = ProcessFunctions.Run(toolFileName, string.Empty, Environment.CurrentDirectory, false, new TimeSpan(0, 0, 24), false, true, true);

			if (result.Output.Trim() != expectedOutput)
			{
				throw new Exception();
			}

			Process process = new Process();
			process.StartInfo.FileName = toolFileName;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;

			process.Start();

			while (!process.HasExited)
			{
				System.Threading.Thread.Sleep(100);
			}

			string altOutput = process.StandardOutput.ReadToEnd();

			if (altOutput.Trim() != expectedOutput)
			{
				throw new Exception();
			}

		}
	}
}
