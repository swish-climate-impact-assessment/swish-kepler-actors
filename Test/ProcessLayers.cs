using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Swish.Tests
{
	static class ProcessLayers
	{
		static string directory = @"C:\Users\Ian\Desktop\Work junk\swishKAGit\Test\GetWeatherGrid";

		internal static void Run()
		{
			string fileName = @"C:\Users\Ian\Desktop\Work junk\swishKAGit\Test\GetWeatherGrid\landmask.csv";
			Csv maskTable = CsvFunctions.Read(fileName);

			int longitudeColumnIndex = maskTable.ColumnIndex("\"long\"", true);
			int latitudeColumnIndex = maskTable.ColumnIndex("\"lat\"", true);
			int landmaskIndex = maskTable.ColumnIndex("\"landmask\"", true);
			GridLayer mask = ConvertToLayer(maskTable, longitudeColumnIndex, latitudeColumnIndex, landmaskIndex);

			string maskName = Path.Combine(directory, "mask");
			WriteFile(maskName + ".bin", mask);
			DoValues(mask, maskName);

			string inputFileName = Path.Combine(directory, "MinMaxDayLatLong.csv");
			Csv allLayers = CsvFunctions.Read(inputFileName);

			string dateVariableName = "\"date\"";
			int dateColumnIndex = allLayers.ColumnIndex(dateVariableName, true);
			SortedList<string, Csv> dateTables = SplitByDate(allLayers, dateColumnIndex);
			List<string> longitudes = null;
			List<string> latitudes = null;

			if (dateTables.Keys.Count == 0)
			{
				throw new Exception();
			}

			string firstDate = dateTables.Keys[0];
			Csv firstTable = dateTables[firstDate];
			longitudeColumnIndex = firstTable.ColumnIndex("\"long\"", true);
			latitudeColumnIndex = firstTable.ColumnIndex("\"lat\"", true);
			GridLayer firstLayer = ConvertToLayer(firstTable, longitudeColumnIndex, latitudeColumnIndex, 0);

			CommonArea(out longitudes, out latitudes, mask, firstLayer);
			mask = SelectArea(mask, longitudes, latitudes);


			List<GridLayer> minimumGrids = new List<GridLayer>();
			List<GridLayer> maximumGrids = new List<GridLayer>();
			using (ThreadedFileProcessor processor = new ThreadedFileProcessor())
			{
				processor.Function = ConvertFunction;
				processor.MultiThreaded = true;

				for (int dateIndex = 0; dateIndex < dateTables.Keys.Count; dateIndex++)
				{
					string date = dateTables.Keys[dateIndex];
					Csv dateTable = dateTables[date];

					processor.Add(new Tuple<string, Csv, List<GridLayer>, List<GridLayer>, GridLayer>(date, dateTable, minimumGrids, maximumGrids, mask));
				}
				processor.Run();
				processor.WaitUntilFinished();
			}

			double threshold = 30;
			GridLayer count30 = new GridLayer();
			for (int lattudeIndex = 0; lattudeIndex < latitudes.Count; lattudeIndex++)
			{
				for (int longitudeIndex = 0; longitudeIndex < longitudes.Count; longitudeIndex++)
				{
					int count = 0;
					for (int dateIndex = 0; dateIndex < maximumGrids.Count; dateIndex++)
					{
						GridLayer layer = maximumGrids[dateIndex];
						double value = layer.Values[lattudeIndex * layer.Longitudes.Count + longitudeIndex];
						if (value > threshold)
						{
							count++;
						}
					}
					count30.Values.Add(count);
				}
			}

			string resultName = Path.Combine(directory, "Count above 30");
			WriteFile(resultName + ".bin", count30);
			DoValues(count30, resultName);

			List<double> uniqueValues = UniqueValues(count30);
			while (true)
			{
				using (ThreadedFileProcessor processor = new ThreadedFileProcessor())
				{
					processor.Function = IsoFunction;
					processor.MultiThreaded = true;
					for (int valueIndex = 1; valueIndex < uniqueValues.Count; valueIndex++)
					{
						double value = uniqueValues[valueIndex];
						string name = Path.Combine(directory, "iso " + value.ToString());
						processor.Add(new Tuple<string, double, GridLayer>(name, value, count30));
					}
					processor.Run();
					processor.WaitUntilFinished();
				}
			}

		}

		private static void ConvertFunction(object data)
		{
			Tuple<string, Csv, List<GridLayer>, List<GridLayer>, GridLayer> _data = (Tuple<string, Csv, List<GridLayer>, List<GridLayer>, GridLayer>)data;

			string date = _data.Item1;
			Csv dateTable = _data.Item2;
			List<GridLayer> minimumGrids = _data.Item3;
			List<GridLayer> maximumGrids = _data.Item4;
			GridLayer mask = _data.Item5;

			int longitudeColumnIndex = dateTable.ColumnIndex("\"long\"", true);
			int latitudeColumnIndex = dateTable.ColumnIndex("\"lat\"", true);

			int minimumColumnIndex = dateTable.ColumnIndex("\"minave\"", true);
			GridLayer minimumLayer = ConvertToLayer(dateTable, longitudeColumnIndex, latitudeColumnIndex, minimumColumnIndex);
			minimumLayer = Multiply(mask, minimumLayer);

			string minimumName = Path.Combine(directory, "minave " + date);
			WriteFile(minimumName + ".bin", minimumLayer);
			DoValues(minimumLayer, minimumName);
			minimumGrids.Add(minimumLayer);

			int maximumColumnIndex = dateTable.ColumnIndex("\"maxave\"", true);
			GridLayer maximumLayer = ConvertToLayer(dateTable, longitudeColumnIndex, latitudeColumnIndex, maximumColumnIndex);
			maximumLayer = Multiply(mask, maximumLayer);

			string maximumName = Path.Combine(directory, "maxave " + date);
			WriteFile(maximumName + ".bin", maximumLayer);
			DoValues(maximumLayer, maximumName);
			maximumGrids.Add(maximumLayer);


			//string dateString = Path.GetFileNameWithoutExtension(maximumName + ".bin");
			//dateString = dateString.Substring(dateString.Length - "2000-00-00".Length);
		}

		private static void IsoFunction(object data)
		{
			Tuple<string, double, GridLayer> _data = (Tuple<string, double, GridLayer>)data;

			string name = _data.Item1;
			double threshold = _data.Item2;
			GridLayer count30 = _data.Item3;

			GridLayer result;
			if (!File.Exists(name + ".bin"))
			{
				result = new GridLayer();
				result.CopyMetadata(count30);

				for (int valueIndex = 0; valueIndex < count30.Values.Count; valueIndex++)
				{
					double value = count30.Values[valueIndex];
					if (value < threshold)
					{
						value = 0;
					} else
					{
						value = 1;
					}
					result.Values.Add(value);
				}

				WriteFile(name + ".bin", result);
				DoValues(result, name);
			} else
			{
				result = ReadFile(name + ".bin");
			}

			List<Polygon> polygons;
			using (Bitmap image = GenerateImage(result.Values, result.Longitudes.Count, result.Latitudes.Count))
			{
				polygons = PolygonFontConverter.ImageToPolygon(image);
			}

			List<List<PointF>> boundaries = new List<List<PointF>>();
			for (int polygonIndex = 0; polygonIndex < polygons.Count; polygonIndex++)
			{
				Polygon polygon = polygons[polygonIndex];
				boundaries.Add(polygon.Verticies);

				for (int cutoutIndex = 0; cutoutIndex < polygon.Cutouts.Count; cutoutIndex++)
				{
					Polygon cutout = polygon.Cutouts[cutoutIndex];
					boundaries.Add(cutout.Verticies);
				}
			}

			List<List<PointF>> newBoundaries = new List<List<PointF>>();
			for (int lineIndex = 0; lineIndex < boundaries.Count; lineIndex++)
			{
				List<PointF> boundary = boundaries[lineIndex];

				List<PointF> newBoundary = new List<PointF>();
				for (int pointIndex = 0; pointIndex < boundary.Count; pointIndex++)
				{
					PointF point = boundary[pointIndex];
					PointF newPoint = new PointF(300.0f * point.X / result.Longitudes.Count, 300.0f * point.Y / result.Latitudes.Count);
					newBoundary.Add(newPoint);
				}
				newBoundaries.Add(newBoundary);
			}
			boundaries = newBoundaries;

			double offset = 1.0 / 2;

			StringBuilder scad = new StringBuilder();
			scad.AppendLine("linear_extrude(height = " + threshold * 5 + ")");
			scad.AppendLine("{");

			for (int lineIndex = 0; lineIndex < boundaries.Count; lineIndex++)
			{
				List<PointF> boundary = boundaries[lineIndex];

				List<string> lines = new List<string>();
				for (int pointIndex = 0; pointIndex < boundary.Count; pointIndex++)
				{
					PointF point = boundary[pointIndex];
					lines.Add("[" + point.X.ToString("f2") + ", " + point.Y.ToString("f2") + "], ");
				}
				string linesFileName = name + " lines " + lineIndex.ToString("d3") + ".txt";
				File.WriteAllLines(linesFileName, lines);


				bool convex = false;
				for (int pointIndex = 0; pointIndex < boundary.Count; pointIndex++)
				{
					PointF _point0 = boundary[(pointIndex + boundary.Count - 1) % boundary.Count];
					PointF _point1 = boundary[pointIndex];
					PointF _point2 = boundary[(pointIndex + boundary.Count + 1) % boundary.Count];

					Vector3 point0 = new Vector3(_point0.X, _point0.Y, 0);
					Vector3 point1 = new Vector3(_point1.X, _point1.Y, 0);
					Vector3 point2 = new Vector3(_point2.X, _point2.Y, 0);

					Vector3 direction01 = point1 - point0;
					Vector3 direction02 = point2 - point0;

					Vector3 cross = FloatMath.CrossProduct(direction01, direction02);

					Vector3 up = new Vector3(0, 0, 1);
					double direction = FloatMath.Dot(cross, up);
					if (direction < 0)
					{
						convex = true;
						break;
					}
				}

				if (!convex)
				{
					List<Vector3> outerBoundary = OffsetBoundary(boundary, offset);
					List<Vector3> innerBoundary = OffsetBoundary(boundary, -offset);
					for (int innerPointIndex = 0; innerPointIndex < innerBoundary.Count; innerPointIndex++)
					{
						Vector3 innerPoint = innerBoundary[innerPointIndex];

						for (int outerPointIndex = 0; outerPointIndex < outerBoundary.Count; outerPointIndex++)
						{
							Vector3 outerPoint = outerBoundary[outerPointIndex];

							if (Projects.StlLibrary.EqualFunctions.Equal(innerPoint, outerPoint, 0.01))
							{
								throw new Exception();
							}
						}
					}

					lines = new List<string>();
					for (int pointIndex = 0; pointIndex < innerBoundary.Count; pointIndex++)
					{
						Vector3 point = innerBoundary[pointIndex];
						lines.Add("[" + point.X.ToString("f2") + ", " + point.Y.ToString("f2") + "], ");
					}
					linesFileName = name + " lines " + lineIndex.ToString("d3") + " inner.txt";
					File.WriteAllLines(linesFileName, lines);

					lines = new List<string>();
					for (int pointIndex = 0; pointIndex < outerBoundary.Count; pointIndex++)
					{
						Vector3 point = outerBoundary[pointIndex];
						lines.Add("[" + point.X.ToString("f2") + ", " + point.Y.ToString("f2") + "], ");
					}
					linesFileName = name + " lines " + lineIndex.ToString("d3") + " outer.txt";
					File.WriteAllLines(linesFileName, lines);


					scad.AppendLine("difference()");
					scad.AppendLine("{");
					scad.Append("#");
					ScadFunctions.WritePolygon(scad, outerBoundary);
					ScadFunctions.WritePolygon(scad, innerBoundary);
					scad.AppendLine("}");
					continue;
				} else
				{
					for (int pointIndex = 0; pointIndex < boundary.Count; pointIndex++)
					{
						PointF _point0 = boundary[pointIndex];
						PointF _point1 = boundary[(pointIndex + boundary.Count + 1) % boundary.Count];
						PointF _point2 = boundary[(pointIndex + boundary.Count + 2) % boundary.Count];
						PointF _point3 = boundary[(pointIndex + boundary.Count + 3) % boundary.Count];

						Vector3 point0 = new Vector3(_point0.X, _point0.Y, 0);
						Vector3 point1 = new Vector3(_point1.X, _point1.Y, 0);
						Vector3 point2 = new Vector3(_point2.X, _point2.Y, 0);
						Vector3 point3 = new Vector3(_point3.X, _point3.Y, 0);


						Vector3 direction01 = point1 - point0;
						Vector3 direction12 = point2 - point1;
						Vector3 direction23 = point3 - point2;

						Vector3 normal01 = FloatMath.Normalise(new Vector3(direction01.Y, -direction01.X, 0));
						Vector3 normal12 = FloatMath.Normalise(new Vector3(direction12.Y, -direction12.X, 0));
						Vector3 normal23 = FloatMath.Normalise(new Vector3(direction23.Y, -direction23.X, 0));

						Vector3 outerOffsetPoint01 = point1 + offset * normal01;
						Vector3 outerOffsetPoint12 = point2 + offset * normal12;
						Vector3 outerOffsetPoint23 = point3 + offset * normal23;

						Vector3 innerOffsetPoint01 = point0 - offset * normal01;
						Vector3 innerOffsetPoint12 = point1 - offset * normal12;
						Vector3 innerOffsetPoint23 = point2 - offset * normal23;

						Vector3 pointA = FloatMath.IntersectionLineLine(outerOffsetPoint01, direction01, outerOffsetPoint12, direction12);
						Vector3 pointB = FloatMath.IntersectionLineLine(innerOffsetPoint01, direction01, innerOffsetPoint12, direction12);
						Vector3 pointC = FloatMath.IntersectionLineLine(innerOffsetPoint12, direction12, innerOffsetPoint23, direction23);
						Vector3 pointD = FloatMath.IntersectionLineLine(outerOffsetPoint12, direction12, outerOffsetPoint23, direction23);

						scad.Append("polygon([");
						scad.Append("[" + (pointA.X).ToString("f2") + ", " + (pointA.Y).ToString("f2") + "]");
						scad.Append(", ");
						scad.Append("[" + (pointB.X).ToString("f2") + ", " + (pointB.Y).ToString("f2") + "]");
						scad.Append(", ");
						scad.Append("[" + (pointC.X).ToString("f2") + ", " + (pointC.Y).ToString("f2") + "]");
						scad.Append(", ");
						scad.Append("[" + (pointD.X).ToString("f2") + ", " + (pointD.Y).ToString("f2") + "]");
						scad.AppendLine("]);");
					}

				}
			}

			scad.AppendLine("}");
			string scadFileName = name + " iso lines" + ".scad";
			File.WriteAllText(scadFileName, scad.ToString());

			string stlFileName = scadFileName + " iso lines" + ".stl";
			MakePattern.CompileScad(stlFileName, scadFileName);
		}

		private static void Range(GridLayer layer, out double minimumValue, out double maximumValue)
		{
			minimumValue = double.MaxValue;
			maximumValue = double.MinValue;

			for (int index = 0; index < layer.Values.Count; index++)
			{
				double value = layer.Values[index];
				if (value == layer.NullValue)
				{
					continue;
				}
				if (value < minimumValue)
				{
					minimumValue = value;
				}
				if (value > maximumValue)
				{
					maximumValue = value;
				}
			}

		}

		private static GridLayer ReadBilTile(string fileName)
		{
			// ie file extension "*.bil"

			string headderFileName = Path.ChangeExtension(fileName, ".hdr");
			if (!File.Exists(headderFileName))
			{
				throw new Exception();
			}

			int lattudeCount = -1;
			int longitudeCount = -1;
			GridLayer layer = new GridLayer();

			using (StreamReader file = new StreamReader(headderFileName))
			{
				while (true)
				{
					string line = file.ReadLine();
					if (line == null)
					{
						break;
					}
					string buffer;
					StringIO.SkipWhiteSpace(out buffer, ref line);
					if (StringIO.TryRead("NROWS", ref line))
					{
						StringIO.SkipWhiteSpace(out buffer, ref line);
						line = line.Trim();
						lattudeCount = int.Parse(line);
					}

					if (StringIO.TryRead("NCOLS", ref line))
					{
						StringIO.SkipWhiteSpace(out buffer, ref line);
						line = line.Trim();
						longitudeCount = int.Parse(line);
					}

					if (StringIO.TryRead("NODATA", ref line))
					{
						StringIO.SkipWhiteSpace(out buffer, ref line);
						line = line.Trim();
						layer.NullValue = int.Parse(line);
					}
				}
			}

			if (lattudeCount == -1 || longitudeCount == -1)
			{
				throw new Exception();
			}

			using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				if (file.Length < lattudeCount * longitudeCount * sizeof(short))
				{
					throw new Exception();
				}

				for (int lattudeIndex = 0; lattudeIndex < lattudeCount; lattudeIndex++)
				{
					for (int longitudeIndex = 0; longitudeIndex < longitudeCount; longitudeIndex++)
					{
						short value = RawStreamIO.ReadShort(file);
						short newValue = (short)(((value & 0xff) << 8) | ((value >> 8) & 0xff));
						//values[lattudeIndex * longitudeCount + longitudeIndex] = value;
						layer.Values.Add(newValue);
					}
				}
			}

			return layer;
		}

		private static GridLayer Multiply(GridLayer left, GridLayer right)
		{
			GridLayer newLayer = new GridLayer();
			for (int indexLong = 0; indexLong < left.Longitudes.Count; indexLong++)
			{
				string longitude = left.Longitudes[indexLong];
				newLayer.Longitudes.Add(longitude);
			}

			for (int indexLat = 0; indexLat < left.Latitudes.Count; indexLat++)
			{
				string latitude = left.Latitudes[indexLat];
				newLayer.Latitudes.Add(latitude);
			}

			int valueCount = left.Longitudes.Count * left.Latitudes.Count;
			for (int valueIndex = 0; valueIndex < valueCount; valueIndex++)
			{
				double leftValue = left.Values[valueIndex];
				double rightValue = right.Values[valueIndex];

				double value = leftValue * rightValue;
				newLayer.Values.Add(value);
			}

			return newLayer;
		}

		private static GridLayer SelectArea(GridLayer layer, List<string> longitudes, List<string> latitudes)
		{
			GridLayer newLayer = new GridLayer();
			for (int indexLong = 0; indexLong < longitudes.Count; indexLong++)
			{
				string longitude = longitudes[indexLong];
				newLayer.Longitudes.Add(longitude);
			}

			for (int indexLat = 0; indexLat < latitudes.Count; indexLat++)
			{
				string latitude = latitudes[indexLat];
				int latitudeIndex = layer.Latitudes.IndexOf(latitude);

				for (int indexLong = 0; indexLong < longitudes.Count; indexLong++)
				{
					string longitude = longitudes[indexLong];
					int longitudeIndex = layer.Longitudes.IndexOf(longitude);

					double value = layer.Values[latitudeIndex * layer.Longitudes.Count + longitudeIndex];
					newLayer.Values.Add(value);
				}

				newLayer.Latitudes.Add(latitude);
			}

			return newLayer;
		}

		private static void CommonArea(out List<string> longitudes, out List<string> latitudes, GridLayer left, GridLayer right)
		{
			longitudes = new List<string>();
			for (int longitudeIndex = 0; longitudeIndex < left.Longitudes.Count; longitudeIndex++)
			{
				string longitude = left.Longitudes[longitudeIndex];

				if (right.Longitudes.Contains(longitude))
				{
					longitudes.Add(longitude);
				}
			}

			latitudes = new List<string>();
			for (int latitudeIndex = 0; latitudeIndex < left.Latitudes.Count; latitudeIndex++)
			{
				string latitude = left.Latitudes[latitudeIndex];

				if (right.Latitudes.Contains(latitude))
				{
					latitudes.Add(latitude);
				}
			}
		}

		private static GridLayer ConvertToLayer(Csv table, int longitudeIndex, int latitudeIndex, int valueIndex)
		{
			GridSorter sorter = new GridSorter();
			sorter.LongitudeIndex = longitudeIndex;
			sorter.LatitudeIndex = latitudeIndex;

			GridLayer layer = new GridLayer();
			layer.Longitudes = UniqueValues(table, longitudeIndex);
			layer.Latitudes = UniqueValues(table, latitudeIndex);

			if (table.Records.Count != layer.Longitudes.Count * layer.Latitudes.Count)
			{
				throw new Exception();
			}

			Csv newTable = new Csv();
			newTable.Records = ListFunctions.Sort(table.Records, sorter.Compare);
			newTable.Header = table.Header;

			layer.Values = newTable.ColunmAsDoubles(valueIndex);

			return layer;
		}

		private class GridSorter
		{
			public int LongitudeIndex;
			public int LatitudeIndex;

			public int Compare(List<string> left, List<string> right)
			{
				double leftLatitude = double.Parse(left[LatitudeIndex]);
				double rightLatitude = double.Parse(right[LatitudeIndex]);

				if (leftLatitude > rightLatitude)
				{
					return -1;
				}
				if (leftLatitude < rightLatitude)
				{
					return 1;
				}

				double leftLongitude = double.Parse(left[LongitudeIndex]);
				double rightLongitude = double.Parse(right[LongitudeIndex]);

				if (leftLongitude < rightLongitude)
				{
					return -1;
				}
				if (leftLongitude > rightLongitude)
				{
					return 1;
				}

				return 0;
			}
		}

		private static void DoValues(GridLayer layer, string name)
		{
			return;
			//using (Bitmap image = GenerateImage(layer.Values, layer.Longitudes.Count, layer.Latitudes.Count))
			//{
			//	string imageFileName = name + ".png";
			//	image.Save(imageFileName);
			//}

			//List<Triangle> triangles = GenerateMesh(layer.Values, layer.Longitudes.Count, layer.Latitudes.Count);
			//string stlFileName = name + ".stl";
			//StlIO.WriteBinaryStl(stlFileName, triangles);
		}

		private static List<Triangle> GenerateMesh(List<double> values, int longitudeCount, int lattudeCount)
		{
			double minimumValue;
			double maximumValue;
			Range(values, out minimumValue, out maximumValue);
			List<Triangle> triangles = new List<Triangle>();
			for (int lattudeIndex = 0; lattudeIndex + 1 < lattudeCount; lattudeIndex++)
			{
				for (int longitudeIndex = 0; longitudeIndex + 1 < longitudeCount; longitudeIndex++)
				{
					double value00 = values[(lattudeIndex + 0) * longitudeCount + longitudeIndex + 0];
					double value01 = values[(lattudeIndex + 0) * longitudeCount + longitudeIndex + 1];
					double value10 = values[(lattudeIndex + 1) * longitudeCount + longitudeIndex + 0];
					double value11 = values[(lattudeIndex + 1) * longitudeCount + longitudeIndex + 1];

					Vector3 point00 = new Vector3(
						300 * (longitudeIndex + 0.0 - 1) / (longitudeCount - 1),
						300 - 300 * (lattudeIndex + 0.0 - 1) / (lattudeCount - 1),
						value00
					);

					Vector3 point01 = new Vector3(
						300 * (longitudeIndex + 1.0 - 1) / (longitudeCount - 1),
						300 - 300 * (lattudeIndex + 0.0 - 1) / (lattudeCount - 1),
						value01
					);

					Vector3 point10 = new Vector3(
						300 * (longitudeIndex + 0.0 - 1) / (longitudeCount - 1),
						300 - 300 * (lattudeIndex + 1.0 - 1) / (lattudeCount - 1),
						value10
					);

					Vector3 point11 = new Vector3(
						300 * (longitudeIndex + 1.0 - 1) / (longitudeCount - 1),
						300 - 300 * (lattudeIndex + 1.0 - 1) / (lattudeCount - 1),
						value11
					);

					Triangle triangle = new Triangle();
					triangle.Point0 = point00;
					triangle.Point1 = point10;
					triangle.Point2 = point01;
					triangles.Add(triangle);

					triangle = new Triangle();
					triangle.Point0 = point01;
					triangle.Point1 = point10;
					triangle.Point2 = point11;
					triangles.Add(triangle);
				}
			}
			return triangles;
		}

		private static Bitmap GenerateImage(List<double> values, int longitudeCount, int lattudeCount)
		{
			double minimumValue;
			double maximumValue;
			Range(values, out minimumValue, out maximumValue);

			if (minimumValue == maximumValue)
			{
				maximumValue += 1;
			}

			Bitmap image = new Bitmap(longitudeCount, lattudeCount);
			for (int lattudeIndex = 0; lattudeIndex < lattudeCount; lattudeIndex++)
			{
				for (int longitudeIndex = 0; longitudeIndex < longitudeCount; longitudeIndex++)
				{
					double value = values[lattudeIndex * longitudeCount + longitudeIndex];

					value = (value - minimumValue) / (maximumValue - minimumValue);

					int intValue = (int)(0xff * value);

					Color colour = Color.FromArgb(0xff, intValue, intValue, intValue);
					image.SetPixel(longitudeIndex, lattudeIndex, colour);
				}
			}
			return image;
		}

		private static void Range(List<double> values, out double minimumValue, out double maximumValue)
		{
			minimumValue = double.MaxValue;
			maximumValue = double.MinValue;
			for (int index = 0; index < values.Count; index++)
			{
				double value = values[index];
				if (value < minimumValue)
				{
					minimumValue = value;
				}
				if (value > maximumValue)
				{
					maximumValue = value;
				}
			}
		}

		private static GridLayer ReadFile(string fileName)
		{
			GridLayer layer = new GridLayer();
			using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				layer.Longitudes = RawStreamIO.ReadList(file, RawStreamIO.ReadString);
				layer.Latitudes = RawStreamIO.ReadList(file, RawStreamIO.ReadString);
				layer.Values = RawStreamIO.ReadList(file, RawStreamIO.ReadDouble);
			}
			return layer;
		}

		private static List<double> ReadFile(string binaryFileName, out int longitudeCount, out int lattudeCount)
		{
			List<double> values = new List<double>();
			using (FileStream file = new FileStream(binaryFileName, FileMode.Open, FileAccess.Read))
			{
				longitudeCount = RawStreamIO.ReadInt(file);
				lattudeCount = RawStreamIO.ReadInt(file);

				for (int lattudeIndex = 0; lattudeIndex < lattudeCount; lattudeIndex++)
				{
					for (int longitudeIndex = 0; longitudeIndex < longitudeCount; longitudeIndex++)
					{
						double value = RawStreamIO.ReadDouble(file);
						//values[lattudeIndex * longitudeCount + longitudeIndex] = value;
						values.Add(value);
					}
				}
			}
			return values;
		}

		private static void WriteFile(string fileName, GridLayer layer)
		{
			using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				RawStreamIO.WriteList(file, layer.Longitudes, RawStreamIO.Write);
				RawStreamIO.WriteList(file, layer.Latitudes, RawStreamIO.Write);
				RawStreamIO.WriteList(file, layer.Values, RawStreamIO.Write);
			}
		}

		private static void WriteFile(string fileName, List<double> values, int longitudeCount, int lattudeCount)
		{
			using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				RawStreamIO.Write(file, longitudeCount);
				RawStreamIO.Write(file, lattudeCount);

				for (int rowIndex = 0; rowIndex < values.Count; rowIndex++)
				{
					double value = values[rowIndex];
					RawStreamIO.Write(file, value);
				}
			}
		}

		private static List<string> UniqueValues(Csv table, int index)
		{
			List<string> longitudes = new List<string>();
			for (int rowIndex = 0; rowIndex < table.Records.Count; rowIndex++)
			{
				List<string> record = table.Records[rowIndex];
				string longitude = record[index];
				if (!longitudes.Contains(longitude))
				{
					longitudes.Add(longitude);
				}
			}

			return longitudes;
		}

		private static SortedList<string, Csv> SplitByDate(Csv table, int columnIndex)
		{
			SortedList<string, Csv> dateTables = new SortedList<string, Csv>();
			for (int rowIndex = 0; rowIndex < table.Records.Count; rowIndex++)
			{
				List<string> record = table.Records[rowIndex];
				string date = record[columnIndex];

				Csv dateTable;
				if (dateTables.ContainsKey(date))
				{
					dateTable = dateTables[date];
				} else
				{
					dateTable = new Csv();
					for (int headderIndex = 0; headderIndex < table.Header.Count; headderIndex++)
					{
						string headder = table.Header[headderIndex];
						dateTable.Header.Add(headder);
					}

					dateTables.Add(date, dateTable);
				}

				dateTable.Records.Add(record);
			}
			return dateTables;
		}

		private static List<double> UniqueValues(GridLayer layer)
		{
			List<double> values = new List<double>();
			for (int valueIndex = 0; valueIndex < layer.Values.Count; valueIndex++)
			{
				double value = layer.Values[valueIndex];
				if (!ListFunctions.Contains(value, values, CompareFunctions.Compare))
				{
					ListFunctions.Add(value, values, CompareFunctions.Compare);
				}
			}
			return values;
		}

		private static List<Vector3> OffsetBoundary(List<PointF> boundary, double offset)
		{
			List<Vector3> newBoundary = new List<Vector3>();
			for (int pointIndex = 0; pointIndex < boundary.Count; pointIndex++)
			{
				PointF previousPoint = boundary[(pointIndex + boundary.Count - 1) % boundary.Count];
				PointF point = boundary[pointIndex];
				PointF nextPoint = boundary[(pointIndex + boundary.Count + 1) % boundary.Count];

				Vector3 previousLineDirection = new Vector3(point.X, point.Y, 0) - new Vector3(previousPoint.X, previousPoint.Y, 0);
				Vector3 previousLineNormal = FloatMath.Normalise(new Vector3(previousLineDirection.Y, -previousLineDirection.X, 0));
				Vector3 previousOffsetPoint = new Vector3(previousPoint.X, previousPoint.Y, 0) + offset * previousLineNormal;

				Vector3 nextLineDirection = new Vector3(nextPoint.X, nextPoint.Y, 0) - new Vector3(point.X, point.Y, 0);
				Vector3 nextLineNormal = FloatMath.Normalise(new Vector3(nextLineDirection.Y, -nextLineDirection.X, 0));
				Vector3 nextOffsetPoint = new Vector3(nextPoint.X, nextPoint.Y, 0) + offset * nextLineNormal;

				Vector3 offsetPoint = FloatMath.IntersectionLineLine(previousOffsetPoint, previousLineDirection, nextOffsetPoint, nextLineDirection);

				double length = FloatMath.Length(offsetPoint - new Vector3(point.X, point.Y, 0));
				if (length > Math.Abs(offset) * 2)
				{
					throw new Exception();
				}

				newBoundary.Add(offsetPoint);
			}
			return newBoundary;
		}
	}
}

