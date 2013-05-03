using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Swish.ScriptGenerators;

namespace Swish.Tests
{
	public class TestsMain
	{
#if !MONO
		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

		private static bool Is64Bit(Process process)
		{
			if ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
			{
				IntPtr processHandle;
				bool retVal;

				try
				{
					processHandle = Process.GetProcessById(process.Id).Handle;
				} catch
				{
					return false; // access is denied to the process
				}

				return IsWow64Process(processHandle, out retVal) && retVal;
			}

			return false; // not on 64-bit Windows
		}
#endif






		static void Main(string[] arguments)
		{
			if (arguments.Length > 0 && arguments[0].ToLower() == "generate")
			{
				try
				{
					GenerateScripts(@"C:\Swish\StataScripts");
					GenerateScripts(@"..\..\..\SimpleInstaller\Scripts");
					GenerateScripts(@"..\..\..\SimpleInstaller\bin\Debug\Scripts");
				} catch (Exception error)
				{
					string message = ExceptionFunctions.Write(error, false);
					message += ProcessFunctions.WriteProcessHeritage();
					message += ProcessFunctions.WriteSystemVariables();

					Console.WriteLine(message);
					SwishFunctions.MessageTextBox(message, false);
				}
				return;
			}

			try
			{

				Swish.SimpleInstaller.Program.Main(new string[] { ">silent" });

				//string leftFileName = @"C:\Users\u5265691\Desktop\FinalWorking\merge4.csv";
				//string rightFileName = @"C:\Users\u5265691\Desktop\FinalWorking\Merge4.do.csv";

				//if (!TablesSomewhatEqulivilent(leftFileName, rightFileName))
				//{
				//    throw new Exception();
				//}

				// missing argument
				// Arguments splitArguments = new Arguments(@">operation merge >input1 D:\HEALTH FORECASTING\FINAL\../TAPM/Brisbane/dataset1/x >input2  >variables date zone group >keepMerge true");
				// string operation = splitArguments.String(Arguments.OperationArgument, true);
				// AdapterFunctions.RunThisOperation(operation, splitArguments);

				/// Input / output 
				/// 
				/// The idea is that the data source is dynamicly resloved and can be:
				///		local data file
				///		files on network server
				///		post gis database
				///		web service of some kind
				/// 
				/// 

				new GetVariableNamesTests().Names();
				new CountOfPreviousDaysTests().Count();
				new FillDatesTests().Fill();
				new GenerateDateRangeOperationTests().GenerateDateRange();
				new ProcessorFunctionsTests().ReceiveOutput();
				new MetadataTests().LoadMetadata();
				new MetadataTests().ValuesWritten();
				new MetadataTests().MetadataFileName();
				new MetadataTests().MetadataExists();
				new SwishFunctionsTests().EncodeDecodePasswordBytes();
				new SwishFunctionsTests().EncodeDecodePassword();
				new SwishFunctionsTests().Password();
				new ArgumentFunctionsTests().GetFlags();
				new KarFunctionsTests().RemovePort();
				new KarFunctionsTests().DumpContents();
				new ArgumentFunctionsTests().GetSwitch();
				new SwishFunctionsTests().TestRunBatchMode();
				new StringIOTests().TryReadStringEscapedCharacters();
				new ArgumentFunctionsTests().ArgumentInQuotes();

				if (!StataFunctions.StataInstalled)
				{
					Console.WriteLine("Stata not found!");
					Console.WriteLine("Ignoring stata tests");
					return;
				}

				new MetadataTests().MergeMetadata();
				new MetadataTests().SequanceHasMetadata();
				new AdapterTests().Generate();
				new AdapterTests().MergeZero();
				new AdapterTests().Replace_1();
				new AdapterTests().Replace_2();
				new AdapterTests().CommandScript();
				new AdapterTests().SelectExpression();
				new AdapterTests().SelectColumns();
				new AdapterTests().TransposeTable();
				new AdapterTests().MergeSorted();
				new AdapterTests().Append();
				new AdapterTests().Mean();
				new AdapterTests().Sort();

				new StataFunctionsTests().Stata12FileName();
				new StataFunctionsTests().ReturnLog();
				new StataScriptFunctionsTests().SaveDynamicFileFormat();
				new StataScriptFunctionsTests().LoadDynamicFileFormat();
				new StataScriptFunctionsTests().StataFileFormat();
				new AdapterTests().RemoveMergeColoumn();
				new StataFunctionsTests().StataBatchMode();

				new TimeSeriesFillTests().Fill();

			} catch (Exception error)
			{
				string message = ExceptionFunctions.Write(error, false);
				message += ProcessFunctions.WriteProcessHeritage();
				message += ProcessFunctions.WriteSystemVariables();
				Console.WriteLine(message);
			}
		}

		private static void GenerateScripts(string directory)
		{
			directory = Path.Combine(Application.StartupPath, directory);
			directory = Path.GetFullPath(directory);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			List<IScriptGenerator> generators = TypeFunctions.Interfaces<IScriptGenerator>();
			for (int generatorIndex = 0; generatorIndex < generators.Count; generatorIndex++)
			{
				IScriptGenerator generator = generators[generatorIndex];

				List<string> lines = new List<string>();
				StataScriptFunctions.WriteHeadder(lines);

				string fileName = Path.Combine(directory, generator.Name + SwishFunctions.DoFileExtension);
				SwishFunctions.SetTemporaryVariableId(Math.Abs(fileName.GetHashCode()));

				generator.GenerateScript(lines);
				StataScriptFunctions.WriteFooter(lines);

				if (FileFunctions.FileExists(fileName))
				{
					File.Delete(fileName);
				}
				File.WriteAllLines(fileName, lines);
			}
		}

		private static bool TablesSomewhatEqulivilent(string leftFileName, string rightFileName)
		{
			Csv left = CsvFunctions.Read(leftFileName);

			Csv right = CsvFunctions.Read(rightFileName);

			if (false
				|| left.Header.Count != right.Header.Count
				|| left.Records.Count != right.Records.Count
				)
			{
				throw new Exception();
			}

			for (int leftIndex = 0; leftIndex < left.Header.Count; leftIndex++)
			{
				string leftVariable = left.Header[leftIndex];

				int rightIndex = right.ColumnIndex(leftVariable, true);
			}

			for (int leftIndex = 0; leftIndex < left.Header.Count; leftIndex++)
			{
				string leftVariable = left.Header[leftIndex];
				int rightIndex = right.ColumnIndex(leftVariable, true);

				List<string> leftValues = left.ColunmValues(leftIndex);
				List<string> rightValues = right.ColunmValues(rightIndex);

				leftValues.Sort();
				rightValues.Sort();
				if (!EqualFunctions.Equal(leftValues, rightValues))
				{
					if (leftVariable == "_merge")
					{
						continue;
					}
					throw new Exception();
				}

			}

			return true;
		}

	}
}






