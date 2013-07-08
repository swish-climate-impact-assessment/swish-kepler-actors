using System.Collections.Generic;
using Swish.Controls;
using Swish.IO;

namespace Swish.Tests
{
	public class GraphingAdapterTest
	{
		internal void ManualTSGraph()
		{
			string inputFileName = GenerateTestData.GenerateRandomDateIndexIntegerDoubleData();
			List<string> variables = new List<string>();
			variables.Add("integerValue");
			variables.Add("doubleValue");

			TableFunctions.GraphSeries(inputFileName, variables);
		}

		internal void GraphGrid()
		{
			GridLayer layer = GenerateTestData.GenerateGrid();

			using (GridView control = new GridView())
			{
				control.Layer = layer;

				DisplayForm.Display(control, "", true, false);
			}

		}

		internal void PlotLargeTS()
		{
			string fileName = @"C:\Users\Ian\Desktop\GetWeatherGrid\MinMaxDayLatLong.csv";
			TableFunctions.GraphSeries(fileName, new List<string>(new string[] { "maxave" }));
		}
	}
}
