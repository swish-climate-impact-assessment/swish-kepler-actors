using System;
using Swish.Controls;
using Swish.IO;
using System.IO;
using System.Collections.Generic;
using Swish.Stata;

namespace Swish.Tests
{
	public static class GenerateTestData
	{
		public static string GenerateMeanInputFile()
		{
			string fileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(fileName))
			{
				File.Delete(fileName);
			}

			List<string> lines = new List<string>();
			lines.Add("head2,head4,head6");
			lines.Add(".0606988,25,\"o\"");
			lines.Add(".401169,8,\"q\"	");
			lines.Add(".0395464,11,\"j\"");
			lines.Add(".4409516,18,\"u\"");
			lines.Add(".8912628,9,\"t\"");
			lines.Add(".5076998,15,\"x\"");
			lines.Add(".3962736,22,\"c\"");
			lines.Add(".5650445,7,\"w\"");
			lines.Add(".0135083,24,\"l\"");
			lines.Add(".8413007,19,\"g\"");
			lines.Add(".1266622,21,\"v\"");
			lines.Add(".3177137,26,\"b\"");
			lines.Add(".0492435,16,\"h\"");
			lines.Add(".9201018,14,\"y\"");
			lines.Add(".5963242,10,\"s\"");
			lines.Add(".80274,2,\"e\"	");
			lines.Add(".8763074,3,\"k\"");
			lines.Add(".1328795,1,\"r\"");
			lines.Add(".3056452,20,\"i\"");
			lines.Add(".2734099,23,\"a\"");
			lines.Add(".1731265,4,\"p\"");
			lines.Add(".7124847,5,\"m\"");
			lines.Add(".2824091,12,\"a\"");
			lines.Add(".0523007,0,\"z\"");
			lines.Add(".5270553,17,\"d\"");
			lines.Add(".3202022,6,\"f\"");
			lines.Add(".9965218,13,\"n\"");

			File.WriteAllLines(fileName, lines.ToArray());

			return fileName;
		}

		public static void GenerateMergeInputFiles(out string inputFileName1, out string inputFileName2, bool missing)
		{
			inputFileName1 = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(inputFileName1))
			{
				File.Delete(inputFileName1);
			}

			List<string> lines = new List<string>();
			lines.Add("head2,head4");
			// left
			lines.Add("0.060698805,25");
			lines.Add("0.03954637,11");
			lines.Add("0.565044458,7");
			lines.Add("0.841300741,19");
			lines.Add("0.132879521,1");
			lines.Add("0.273409937,23");
			lines.Add("0.712484717,5");
			lines.Add("0.527055285,17");
			lines.Add("0.996521752,13");

			// joint
			lines.Add("0.891262839,9");
			lines.Add("0.507699792,15");
			lines.Add("0.126662171,21");
			lines.Add("0.876307404,3");

			lines.Add("0.440951591,18");
			lines.Add("0.013508331,24");
			lines.Add("0.282409141,12");
			lines.Add("0.052300672,0");
			lines.Add("0.320202245,6");

			if (!missing)
			{
				// missing
				lines.Add("0.40116903,8");
				lines.Add("0.396273631,22");
				lines.Add("0.31771373,26");
				lines.Add("0.049243499,16");
				lines.Add("0.596324211,10");
				lines.Add("0.802740035,2");
				lines.Add("0.30564522,20");
				lines.Add("0.17312655,4");

			}
			File.WriteAllLines(inputFileName1, lines.ToArray());

			inputFileName2 = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(inputFileName2))
			{
				File.Delete(inputFileName2);
			}

			lines = new List<string>();
			lines.Add("head6,head4");
			// right
			lines.Add("c,2");
			lines.Add("t,20");
			lines.Add("v,22");
			lines.Add("e,4");
			lines.Add("k,10");
			lines.Add("i,8");
			lines.Add("p,16");
			lines.Add("z,26");

			// joint
			lines.Add("o,15");
			lines.Add("j,9");
			lines.Add("u,21");
			lines.Add("d,3");

			lines.Add("x,24");
			lines.Add("g,6");
			lines.Add("r,18");
			lines.Add("a,0");
			lines.Add("m,12");

			if (!missing)
			{
				// missing
				lines.Add("q,17");
				lines.Add("w,23");
				lines.Add("l,11");
				lines.Add("b,1");
				lines.Add("h,7");
				lines.Add("y,25");
				lines.Add("s,19");
				lines.Add("f,5");
				lines.Add("n,13");
			}

			File.WriteAllLines(inputFileName2, lines.ToArray());
		}

		public const string MergeVariable = "head4";

		public static string CarsDataFileName = TestFunctions.TestDataFileName("carsdata.dta");
		public static Table CarData()
		{
			Table expectedTable = new Table();
			//List<List<string>> expectedTable = new List<List<string>>();

			expectedTable.Headers.Add("cars");
			expectedTable.Headers.Add("hhsize");
			List<string> list = new List<string>();
			list.Add("1");
			list.Add("1");
			expectedTable.Records.Add(list);
			list = new List<string>();
			list.Add("2");
			list.Add("2");
			expectedTable.Records.Add(list);
			list = new List<string>();
			list.Add("2");
			list.Add("3");
			expectedTable.Records.Add(list);
			list = new List<string>();
			list.Add("2");
			list.Add("4");
			expectedTable.Records.Add(list);
			list = new List<string>();
			list.Add("3");
			list.Add("5");
			expectedTable.Records.Add(list);
			return expectedTable;
		}

		internal static void GenerateAppendInputFile(out string inputFileName1, out string inputFileName2)
		{
			inputFileName1 = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(inputFileName1))
			{
				File.Delete(inputFileName1);
			}

			List<string> lines = new List<string>();

			lines.Add("head2\thead4\thead6");
			lines.Add("0.0606988\t25\to");
			lines.Add("0.401169\t8\tq");
			lines.Add("0.0395464\t11\tj");
			lines.Add("0.4409516\t18\tu");
			lines.Add("0.8912628\t9\tt");
			lines.Add("0.5076998\t15\tx");
			lines.Add("0.3962736\t22\tc");
			lines.Add("0.5650445\t7\tw");
			lines.Add("0.0135083\t24\tl");
			lines.Add("0.8413007\t19\tg");
			lines.Add("0.1266622\t21\tv");
			lines.Add("0.3177137\t26\tb");
			lines.Add("0.0492435\t16\th");

			File.WriteAllLines(inputFileName1, lines.ToArray());

			inputFileName2 = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(inputFileName2))
			{
				File.Delete(inputFileName2);
			}

			lines = new List<string>();

			lines.Add("head2\thead4\thead6");
			lines.Add("0.9201018\t14\ty");
			lines.Add("0.5963242\t10\ts");
			lines.Add("0.80274\t2\te");
			lines.Add("0.8763074\t3\tk");
			lines.Add("0.1328795\t1\tr");
			lines.Add("0.3056452\t20\ti");
			lines.Add("0.2734099\t23\ta");
			lines.Add("0.1731265\t4\tp");
			lines.Add("0.7124847\t5\tm");
			lines.Add("0.2824091\t12\ta");
			lines.Add("0.0523007\t0\tz");
			lines.Add("0.5270553\t17\td");
			lines.Add("0.3202022\t6\tf");
			lines.Add("0.9965218\t13\tn");


			File.WriteAllLines(inputFileName2, lines.ToArray());

		}

		public static string GenerateReplaceInputFile()
		{
			string fileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(fileName))
			{
				File.Delete(fileName);
			}

			List<string> lines = new List<string>();
			lines.Add("head2, head4, head6");
			lines.Add("1, 2, 6");
			lines.Add("1, 2, 6");
			lines.Add("1, 5, 6");
			lines.Add("1, 5, 6");
			lines.Add("4, 5, 6");
			lines.Add("4, 5, 6");
			lines.Add("4, 5, 3");
			lines.Add("4, 5, 3");
			lines.Add("4, 2, 3");
			lines.Add("4, 2, 3");

			File.WriteAllLines(fileName, lines.ToArray());

			return fileName;
		}

		public static string GenerateRandomDateIndexIntegerDoubleData()
		{
			string fileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(fileName))
			{
				File.Delete(fileName);
			}

			List<string> lines = new List<string>();

			int count = 1000;
			lines.Add("dateValue,indexValue,integerValue,doubleValue");
			Random random = new Random();
			for (int index = 0; index < count; index++)
			{
				lines.Add(""
					+ new DateTime(2000, 1, 1).AddDays(index).ToShortDateString() + ","
					+ index.ToString() + ","
					+ random.Next().ToString() + ","
					+ random.NextDouble().ToString()
					);
			}

			File.WriteAllLines(fileName, lines.ToArray());

			return fileName;
		}

		internal static void GenerateMergeDoFile(string doFileName)
		{
			List<string> variableNames = new List<string>();
			variableNames.Add("head4");
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, @"C:\Swish\SampleData\MergeTable1.csv");

			StataScriptFunctions.SortCommand(lines, StataScriptFunctions.VariableList(variableNames));
			StataScriptFunctions.SaveFileCommand(lines, @"C:\Swish\SampleData\MergeTableTemp.dta");

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, @"C:\Swish\SampleData\MergeTable2.csv");

			StataScriptFunctions.SortCommand(lines, StataScriptFunctions.VariableList(variableNames));
			lines.Add("merge 1:1 " + StataScriptFunctions.VariableList(variableNames) + " using \"" + @"C:\Swish\SampleData\MergeTableTemp.dta" + "\"");
			lines.Add("drop " + StataScriptFunctions.MergeColumnName);
			StataScriptFunctions.SaveFileCommand(lines, @"C:\Swish\SampleData\MergeTableOut.csv");

			StataScriptFunctions.WriteFooter(lines);

			File.WriteAllLines(doFileName, lines.ToArray());



		}

		public static GridLayer GenerateGrid()
		{
			List<string> latitudes = new List<string>(new string[] { "1", "2", "3", "4", "5", });
			List<string> longitudes = new List<string>(new string[] { "1", "2", "3", "4", "5", });

			GridLayer layer = new GridLayer();
			layer.Latitudes = latitudes;
			layer.Longitudes = longitudes;

			for (int longitudeIndex = 0; longitudeIndex < longitudes.Count; longitudeIndex++)
			{
				string longitude = longitudes[longitudeIndex];
				for (int latitudeIndex = 0; latitudeIndex < latitudes.Count; latitudeIndex++)
				{
					string latitude = latitudes[latitudeIndex];

					double valueA = longitudeIndex / (longitudes.Count - 1.0);
					double valueB = latitudeIndex / (latitudes.Count - 1.0);
					double value = 1 - Math.Sqrt(valueA * valueA + valueB * valueB);

					layer.Values.Add(value);
				}
			}
			return layer;
		}

		public const string LatitudeVariableName  = "latitude";
		public const string LongitudeVariableName = "longitude";
		public static Table GenerateGridTable()
		{
			List<string> latitudes = new List<string>(new string[] { "1", "2", "3", "4", "5", });
			List<string> longitudes = new List<string>(new string[] { "1", "2", "3", "4", "5", });

			Table table = new Table();
			table.Headers.Add(LatitudeVariableName);
			table.Headers.Add(LongitudeVariableName);
			table.Headers.Add("Value");

			for (int longitudeIndex = 0; longitudeIndex < longitudes.Count; longitudeIndex++)
			{
				string longitude = longitudes[longitudeIndex];
				for (int latitudeIndex = 0; latitudeIndex < latitudes.Count; latitudeIndex++)
				{
					string latitude = latitudes[latitudeIndex];

					//double valueA = longitudeIndex / (longitudes.Count - 1.0);
					//double valueB = latitudeIndex / (latitudes.Count - 1.0);
					//double value = 1 - Math.Sqrt(valueA * valueA + valueB * valueB);

					double value = latitudeIndex * longitudes.Count + longitudeIndex;
					List<string> record = new List<string>(new string[] { latitude, longitude, value.ToString() });

					table.Records.Add(record);
				}
			}
			return table;
		}

		internal static string GenerateFillDateInputData(DateTime startDate, DateTime endDate)
		{
			string inputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			Table table = new Table();
			table.Headers.Add("date");
			table.Headers.Add("other");

			double aValue;
			List<string> record;

			DateTime date = startDate.AddDays(1);
			Random random = new Random();
			while (date.AddDays(1) <= endDate)
			{
				aValue = random.NextDouble();
				record = new List<string>();
				record.Add(date.ToShortDateString());
				record.Add(aValue.ToString());
				table.Records.Add(record);
				date = date.AddDays(1);
			}

			int daysInYear = (int)((endDate - startDate).TotalDays) + 1;
			while (table.Records.Count > daysInYear / 2)
			{
				int number = random.Next(table.Records.Count);
				table.Records.RemoveAt(number);
			}

			aValue = random.NextDouble();
			record = new List<string>();
			record.Add(startDate.ToShortDateString());
			record.Add(aValue.ToString());
			table.Records.Add(record);

			aValue = random.NextDouble();
			record = new List<string>();
			record.Add(endDate.ToShortDateString());
			record.Add(aValue.ToString());
			table.Records.Add(record);

			CsvFunctions.Write(inputFileName, table);
			return inputFileName;
		}

	}
}
