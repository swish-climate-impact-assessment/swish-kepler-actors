using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;

namespace Swish.Tests
{
	public class TestsMain
	{
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

		static void Main(string[] arguments)
		{
			try
			{


				/// Input / output 
				/// 
				/// The idea is that the data source is dynamicly resloved and can be:
				///		local data file
				///		files on network server
				///		post gis database
				///		web service of some kind
				/// 
				/// 
				/// Generate metadata 
				/// start with a log file that accopanyies the output file with the operation name and input details
				/// 

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
					return;
				}

				new AdapterTests().MergeKeep();
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

			} catch (Exception error)
			{
				Console.WriteLine(ExceptionFunctions.Write(error, false));
			}


		}
	}
}




