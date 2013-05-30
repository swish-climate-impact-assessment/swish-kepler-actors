using System;
using System.Collections.Generic;

namespace Swish.Tests
{
	class GridFunctionsTests
	{
		internal void ConvertToLayer()
		{
			Csv table = GenerateGridTable();

			int latitudeColumnIndex = 0;
			int longitudeColumnIndex = 1;
			int valueColumnIndex = 2;
			GridLayer layer = GridFunctions.ConvertToLayer(table, longitudeColumnIndex, latitudeColumnIndex, valueColumnIndex);

			for (int cellIndex = 0; cellIndex < layer.Values.Count; cellIndex++)
			{
				double value = layer.Values[cellIndex];
				if (value != cellIndex)
				{
					throw new Exception();
				}
			}
		}

		public static Csv GenerateGridTable()
		{
			List<string> latitudes = new List<string>(new string[] { "1", "2", "3", "4", "5", });
			List<string> longitudes = new List<string>(new string[] { "1", "2", "3", "4", "5", });

			Csv table = new Csv();
			table.Headers.Add("Latitude");
			table.Headers.Add("Longitude");
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

		internal void LayerIO()
		{
			GridLayer layer = GenerateGrid();
			string fileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.AscGridFileExtension);
			GridFunctions.Write(fileName, layer);

			GridLayer layerIn = GridFunctions.Read(fileName);

			if (layerIn.NullValue != layer.NullValue)
			{
				throw new Exception();
			}

			if (layerIn.Latitudes.Count != layer.Latitudes.Count)
			{
				throw new Exception();
			}

			for (int latitudeIndex = 0; latitudeIndex < layer.Latitudes.Count; latitudeIndex++)
			{
				if (layerIn.Latitudes[latitudeIndex] != layer.Latitudes[latitudeIndex])
				{
					throw new Exception();
				}
			}

			if (layerIn.Longitudes.Count != layer.Longitudes.Count)
			{
				throw new Exception();
			}

			for (int longitudeIndex = 0; longitudeIndex < layer.Latitudes.Count; longitudeIndex++)
			{
				if (layerIn.Longitudes[longitudeIndex] != layer.Longitudes[longitudeIndex])
				{
					throw new Exception();
				}
			}

			if (layerIn.Values.Count != layer.Values.Count)
			{
				throw new Exception();
			}

			for (int valueIndex = 0; valueIndex < layer.Latitudes.Count; valueIndex++)
			{
				if (layerIn.Values[valueIndex] != layer.Values[valueIndex])
				{
					throw new Exception();
				}
			}

		}

		internal void ConvertLarge()
		{
			string fileName = @"C:\Users\Ian\Desktop\GetWeatherGrid\MinMaxDayLatLong.csv";

			Csv table = CsvFunctions.Read(fileName);

			int latitudeColumnIndex = table.ColumnIndex("lat", true);
			int longitudeColumnIndex = table.ColumnIndex("long", true);
			int valueColumnIndex = table.ColumnIndex("maxave", true);

			GridLayer layer = GridFunctions.ConvertToLayer(table, longitudeColumnIndex, latitudeColumnIndex, valueColumnIndex);
		}
	}
}
