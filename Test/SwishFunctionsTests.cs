using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using Swish.Server;

namespace Swish.Tests
{
	public class SwishFunctionsTests
	{
		public void TestRunBatchMode()
		{
			/// this tests that the executable run is Stata, 
			/// and that the arguments are correct
			/// 

			/// test works by creating a fake Stata and testing the arguments
			/// make a fake Stata

			// "C:\Program Files\Stata12\StataMP" /e do c:\data\bigjob.do
			string argumentsFileName = "ArgumentsOut.txt";
			string executableFileName = CSharpCompiler.MakeExecutable("public class Program{static void Main(string[] arguments){System.IO.File.WriteAllText(\"" + argumentsFileName + "\", string.Join(\" \", arguments));}}", false);

			string directory = Path.GetTempPath();
			List<string> lines = new List<string>();

			argumentsFileName = Path.Combine(directory, argumentsFileName);
			string expectedArguments = StataFunctions.BatchArgument;
			try
			{
				StataFunctions.ExecutablePath = executableFileName;

				string log = StataFunctions.RunScript(lines, false);

				string arguments = File.ReadAllText(argumentsFileName);
				if (!arguments.StartsWith(expectedArguments))
				{
					throw new TestException();
				}
			} finally
			{
				StataFunctions.ExecutablePath = null;
			}
		}

		internal void Password()
		{
			if (DateTime.MinValue < DateTime.MaxValue)
			{
				// this is a manual test
				return;
			}

			string prompt = "Enter a password";
			string prompt2 = "Enter another password";

			string password = AdapterFunctions.Password(prompt, false);

			string password2 = AdapterFunctions.Password(prompt, false);

			string password3 = AdapterFunctions.Password(prompt2, false);

			string password4 = AdapterFunctions.Password(prompt, false);

			string password5 = AdapterFunctions.Password(prompt2, false);
		}

		internal void EncodeDecodePassword()
		{
			Process process = Process.GetCurrentProcess();
			string password = "this is the password text";

			string encodedPassword = SwishFunctions.EncodePassword(password, process);

			if (encodedPassword == password)
			{
				throw new TestException();
			}

			string decodedPassword = SwishFunctions.DecodePassword(encodedPassword, process);

			if (decodedPassword != password)
			{
				throw new TestException();
			}
		}

		internal void EncodeDecodePasswordBytes()
		{
			Process process = Process.GetCurrentProcess();
			string password = "this is the password text";
			byte[] bytes = ASCIIEncoding.ASCII.GetBytes(password);

			byte[] encodedBytes = SwishFunctions.MangleBytes(bytes, process);

			if (EqualFunctions.Equal(bytes, encodedBytes))
			{
				throw new TestException();
			}

			byte[] decodedBytes = SwishFunctions.MangleBytes(encodedBytes, process);

			if (!EqualFunctions.Equal(bytes, decodedBytes))
			{
				throw new TestException();
			}
		}
	}
}
