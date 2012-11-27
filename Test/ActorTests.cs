using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Swish.Tests
{
	class ActorTests
	{

		private static string _keplerFileName = null;
		public static string KeplerFileName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_keplerFileName))
				{
					List<string> locations = SwishFunctions.Locations();
					locations.Add(@"C:\Program Files\Kepler-2.3");

					List<string> fileNames = new List<string>();
					fileNames.Add("kepler.bat");

					for (int fileIndex = 0; fileIndex < fileNames.Count; fileIndex++)
					{
						string file = fileNames[fileIndex];
						string fileName = SwishFunctions.ResloveFileName(file, locations, false, true);
						if (!string.IsNullOrWhiteSpace(fileName))
						{
							_keplerFileName = fileName;
							return fileName;
						}
					}

					throw new Exception("Could not find installed version of Kepler");

				}
				return _keplerFileName;
			}
			set { _keplerFileName = value; }
		}

		internal void AppendTables()
		{

			if (DateTime.MaxValue > DateTime.MinValue)
			{
				throw new Exception("this test does not work, but left here for documentation ");

				/// the actors do not run by them self as they do not have a director 
				/// the actors are embeded in the parent work flow, this wouldn't work for
				/// verfication as changes would not be tested
				/// it fails trying to set the port values, maybe it would work with parameters - not tested atm
			}

			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateAppendInputFile(out inputFileName1, out inputFileName2);

			string outputFileName = Path.GetTempFileName() + ".csv";
			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string worflowFileName = @"C:\Users\u5265691\KeplerData\workflows\MyWorkflows\AppendTables.kar";

			List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
			parameters.Add(new Tuple<string, string>("Input1", inputFileName1));
			parameters.Add(new Tuple<string, string>("Input2", inputFileName2));
			parameters.Add(new Tuple<string, string>("Output", outputFileName));

			string arguments = "-runkar -nogui " + worflowFileName;
			for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
			{
				Tuple<string, string> parameter = parameters[parameterIndex];
				arguments += " -" + parameter.Item1 + " " + parameter.Item2;
			}

			string workingDirectory = System.IO.Path.GetDirectoryName(KeplerFileName);
			int exitCode;
			string output;
			SwishFunctions.RunProcess(KeplerFileName, arguments, workingDirectory, false, new TimeSpan(0, 0, 0, 8, 0), out exitCode, out output);

			Csv table = CsvFunctions.Read(outputFileName);

			if (table.Records.Count != 14 - 1 + 15 - 1)
			{
				throw new Exception("append failed");
			}
		}

	}
}
