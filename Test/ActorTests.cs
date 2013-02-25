using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Tests
{
	class ActorTests
	{

		internal void AppendTables()
		{

			if (DateTime.MaxValue > DateTime.MinValue)
			{
				throw new TestException("this test does not work, but left here for documentation ");

				/// the actors do not run by them self as they do not have a director 
				/// the actors are embeded in the parent work flow, this wouldn't work for
				/// verfication as changes would not be tested
				/// it fails trying to set the port values, maybe it would work with parameters - not tested atm
			}

			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateAppendInputFile(out inputFileName1, out inputFileName2);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string worflowFileName = @"C:\Users\u5265691\KeplerData\workflows\MyWorkflows\AppendTables.kar";

			List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
			parameters.Add(new Tuple<string, string>("Input1", inputFileName1));
			parameters.Add(new Tuple<string, string>("Input2", inputFileName2));
			parameters.Add(new Tuple<string, string>("Output", outputFileName));

			KeplerFunctions.RunWorkflow(worflowFileName, parameters);

			Csv table = CsvFunctions.Read(outputFileName);

			if (table.Records.Count != 14 - 1 + 15 - 1)
			{
				throw new TestException("append failed");
			}
		}

	}
}
