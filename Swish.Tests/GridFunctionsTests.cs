using System;
using System.Collections.Generic;
using Swish.IO;

namespace Swish.Tests
{
	class GridFunctionsTests
	{
		internal void ConvertToLayer()
		{
			Table table = GenerateTestData.GenerateGridTable();

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

		internal void LayerIO()
		{
			GridLayer layer = GenerateTestData.GenerateGrid();
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



	}
}
