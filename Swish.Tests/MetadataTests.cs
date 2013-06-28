using System;
using System.Collections.Generic;
using System.IO;
using Swish.IO;
using Swish.Stata;

namespace Swish.Tests
{
	class MetadataTests
	{
		internal void SequanceHasMetadata()
		{
			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			string metadataFileName = MetadataFunctions.FileName(outputFileName);

			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);

			TableFunctions.Sort(inputFileName, new List<string>(new string[] { "head4" }), intermediateFileName);
			TableFunctions.Generate(intermediateFileName, outputFileName, "testVariable1", StataDataType.Unknown, "-head4");
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new TestException();
			}

			if (!FileFunctions.FileExists(metadataFileName))
			{
				throw new TestException();
			}

			string text = File.ReadAllText(metadataFileName);
			if (false
				|| !text.Contains(inputFileName)
				|| !text.Contains("Sort")
				|| !text.Contains("head4")
				|| !text.Contains(intermediateFileName)
				|| !text.Contains("Generate")
				|| !text.Contains("testVariable")
				|| !text.Contains("-head4")
				|| !text.Contains(outputFileName)
				)
			{
				throw new Exception();
			}
		}

		internal void MetadataExists()
		{
			/// this verifies metadata can be detected for a data file 
			// similar to File.Exists

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			string metadataFileName = MetadataFunctions.FileName(outputFileName);

			if (MetadataFunctions.Exists(outputFileName))
			{
				throw new Exception();
			}

			File.WriteAllText(metadataFileName, "some text");

			if (!MetadataFunctions.Exists(outputFileName))
			{
				throw new Exception();
			}
		}

		internal void MetadataFileName()
		{
			// given a file name get the metadata file name

			string fileName = "dataFile.csv";
			string expectedMetadataFileName = "dataFile.csv_SwishMetadata.xml";

			string metadataFileName = MetadataFunctions.FileName(fileName);
			if (metadataFileName != expectedMetadataFileName)
			{
				throw new Exception();
			}
		}

		internal void ValuesWritten()
		{
			// also the operation

			/// verifies that the user name, and pc are included in the metadata
			/// this includes the user name,
			/// pc name,
			/// anything else that looks interesting
			/// 

			MetadataRecord record = new MetadataRecord();
			record.User = "TestUser";
			record.ComputerName = "PCName";
			DateTime time = DateTime.Now;
			string timeString = time.ToShortDateString() + " " + time.ToLongTimeString();
			record.Time = time;
			record.Arguments.Add(new Tuple<string, string>("input1", "value1"));
			record.Operation = "TestOperation";

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			metadata.Add(record);

			string fileName = FileFunctions.TempoaryOutputFileName(".txt");
			MetadataFunctions.Save(fileName, metadata);

			string metadataFileName = MetadataFunctions.FileName(fileName);
			if (!FileFunctions.FileExists(metadataFileName))
			{
				throw new Exception();
			}
			string text = File.ReadAllText(metadataFileName);

			if (!text.Contains("TestUser")
				|| !text.Contains("PCName")
				|| !text.Contains(timeString)
				|| !text.Contains("input1")
				|| !text.Contains("value1")
				|| !text.Contains("TestOperation")
				)
			{
				throw new Exception();
			}
		}

		internal void LoadMetadata()
		{
			List<MetadataRecord> metadata = new List<MetadataRecord>();

			MetadataRecord record1 = new MetadataRecord();
			record1.User = "TestUser";
			record1.ComputerName = "PCName";
			DateTime time = DateTime.Now;
			time = time.Date.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second);
			record1.Time = time;
			record1.Arguments.Add(new Tuple<string, string>("input1", "value1"));
			record1.Operation = "TestOperation";
			metadata.Add(record1);

			MetadataRecord record2 = new MetadataRecord();
			record2.User = "TestUser2";
			record2.ComputerName = "PCName2";
			record2.Time = time;
			record2.Arguments.Add(new Tuple<string, string>("input12", "value12"));
			record2.Arguments.Add(new Tuple<string, string>("input22", "value22"));
			record2.Operation = "TestOperation2";
			metadata.Add(record2);

			string fileName = FileFunctions.TempoaryOutputFileName(".txt");
			MetadataFunctions.Save(fileName, metadata);

			List<MetadataRecord> loadedMetadata = MetadataFunctions.Load(fileName);

			if (loadedMetadata.Count != 2)
			{
				throw new Exception();
			}

			MetadataRecord loadedRecord1 = loadedMetadata[0];
			MetadataRecord loadedRecord2 = loadedMetadata[1];

			if (!Equal(record1, loadedRecord1))
			{
				throw new Exception();
			}

			if (!Equal(record2, loadedRecord2))
			{
				throw new Exception();
			}


		}

		private static bool Equal(MetadataRecord left, MetadataRecord right)
		{
			if (false
				|| right.User != left.User
				|| right.ComputerName != left.ComputerName
				|| right.Time != left.Time
				|| right.Operation != left.Operation
				|| right.Arguments.Count != left.Arguments.Count
				)
			{
				throw new Exception();
			}

			for (int argumentIndex = 0; argumentIndex < right.Arguments.Count; argumentIndex++)
			{
				Tuple<string, string> record1Argument = left.Arguments[argumentIndex];
				Tuple<string, string> loadedRecord1Argument = right.Arguments[argumentIndex];
				if (loadedRecord1Argument.Item1 != record1Argument.Item1 || loadedRecord1Argument.Item2 != record1Argument.Item2)
				{
					throw new Exception();
				}
			}

			return true;
		}

		internal void MergeMetadata()
		{
			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateMergeInputFiles(out  inputFileName1, out  inputFileName2, false);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);

			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			TableFunctions.Merge(inputFileName1, inputFileName2, variables, outputFileName, false);

			string metadataFileName = MetadataFunctions.FileName(outputFileName);
			string text = File.ReadAllText(metadataFileName);

			if (!text.Contains(StataFunctionsTests.MergeVariable))
			{
				throw new Exception();
			}
		}
	}
}
