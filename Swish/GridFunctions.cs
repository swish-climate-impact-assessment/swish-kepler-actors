using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Swish
{
	public static class GridFunctions
	{
		public static GridLayer ConvertToLayer(Csv table, int longitudeColumnIndex, int latitudeColumnIndex, int valueColumnIndex)
		{
			int nullValue = -9999;

			Tuple<double, double, int>[] sortedRecords = new Tuple<double, double, int>[table.Records.Count];

			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				double latitude = double.Parse(record[latitudeColumnIndex]);
				double longitude = double.Parse(record[longitudeColumnIndex]);

				Tuple<double, double, int> sortRecord = new Tuple<double, double, int>(latitude, longitude, recordIndex);
				sortedRecords[recordIndex] = sortRecord;
			}

			ListFunctions.QuickSort(sortedRecords, Compare);

			List<string> _longitudes = CsvFunctions.UniqueValues(table, longitudeColumnIndex);
			List<double> longitudes = ConvertToDoubles(_longitudes);
			List<string> _latitudes = CsvFunctions.UniqueValues(table, latitudeColumnIndex);
			List<double> latitudes = ConvertToDoubles(_latitudes);

			List<double> values = new List<double>();
			for (int latitudeIndex = 0; latitudeIndex < latitudes.Count; latitudeIndex++)
			{
				double latitude = latitudes[latitudeIndex];

				for (int longitudeIndex = 0; longitudeIndex < longitudes.Count; longitudeIndex++)
				{
					double longitude = longitudes[longitudeIndex];

					double value;
					Tuple<double, double, int> searchRecord = new Tuple<double, double, int>(latitude, longitude, -1);
					int listIndex = ListFunctions.IndexOf(searchRecord, sortedRecords, Compare);
					if (listIndex >= 0)
					{
						listIndex = sortedRecords[listIndex].Item3;
						List<string> record = table.Records[listIndex];
						value = double.Parse(record[valueColumnIndex]);
					} else
					{
						value = nullValue;
					}
					values.Add(value);
				}
			}

			GridLayer layer = new GridLayer();
			layer.Longitudes = _longitudes;
			layer.Latitudes = _latitudes;
			layer.NullValue = nullValue;
			layer.Values = values;

			return layer;
		}

		private static List<double> ConvertToDoubles(List<string> _longitudes)
		{
			List<double> longitudes = new List<double>();
			for (int longitudeIndex = 0; longitudeIndex < _longitudes.Count; longitudeIndex++)
			{
				string longitude = _longitudes[longitudeIndex];
				double value = double.Parse(longitude);
				longitudes.Add(value);
			}
			return longitudes;
		}

		private static int Compare(Tuple<double, double, int> left, Tuple<double, double, int> right)
		{
			double leftLatitude = left.Item1;
			double rightLatitude = right.Item1;

			if (leftLatitude > rightLatitude)
			{
				return -1;
			}
			if (leftLatitude < rightLatitude)
			{
				return 1;
			}

			double leftLongitude = left.Item2;
			double rightLongitude = right.Item2;

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

		public static void Write(string fileName, GridLayer layer)
		{
			double x0;
			double y0;
			if (layer.Longitudes.Count > 0)
			{
				x0 = double.Parse(layer.Longitudes[0]);
			} else
			{
				x0 = layer.NullValue;
			}
			if (layer.Latitudes.Count > 0)
			{
				y0 = double.Parse(layer.Latitudes[0]);
			} else
			{
				y0 = layer.NullValue;
			}

			double cellSize;
			if (layer.Longitudes.Count > 1)
			{
				cellSize = double.Parse(layer.Longitudes[1]) - double.Parse(layer.Longitudes[0]);
			} else if (layer.Latitudes.Count > 1)
			{
				cellSize = double.Parse(layer.Latitudes[1]) - double.Parse(layer.Latitudes[0]);
			} else
			{
				cellSize = 1;
			}


			using (StreamWriter file = new StreamWriter(fileName))
			{
				file.WriteLine("ncols         " + layer.Longitudes.Count);
				file.WriteLine("nrows         " + layer.Latitudes.Count);
				file.WriteLine("xllcorner     " + x0);
				file.WriteLine("yllcorner     " + y0);
				file.WriteLine("cellsize      " + cellSize);
				file.WriteLine("NODATA_value  " + layer.NullValue.ToString());

				for (int latitudeIndex = 0; latitudeIndex < layer.Latitudes.Count; latitudeIndex++)
				{
					StringBuilder line = new StringBuilder();
					for (int longitudeIndex = 0; longitudeIndex < layer.Longitudes.Count; longitudeIndex++)
					{
						line.Append(layer.Values[latitudeIndex * layer.Longitudes.Count + longitudeIndex] + " ");
					}
					file.WriteLine(line);
				}
			}

		}

		public static GridLayer Read(string fileName)
		{
			int columns = -1;
			int rows = -1;
			double xllcorner = -9999;
			double yllcorner = -9999;
			double cellSize = -1;
			double nullValue = -9999;

			List<double> values = new List<double>();
			using (StreamReader file = new StreamReader(fileName))
			{
				bool data = false;
				while (true)
				{
					string line = file.ReadLine();
					if (line == null)
					{
						break;
					}
					if (!data)
					{
						string buffer;
						StringIO.SkipWhiteSpace(out buffer, ref line);
						if (StringIO.TryRead("ncols", ref line))
						{
							StringIO.SkipWhiteSpace(out buffer, ref line);
							StringIO.TryRead(out columns, ref line);
						} else if (StringIO.TryRead("nrows", ref line))
						{
							StringIO.SkipWhiteSpace(out buffer, ref line);
							StringIO.TryRead(out rows, ref line);
						} else if (StringIO.TryRead("xllcorner", ref line))
						{
							StringIO.SkipWhiteSpace(out buffer, ref line);
							StringIO.TryRead(out xllcorner, ref line);
						} else if (StringIO.TryRead("yllcorner", ref line))
						{
							StringIO.SkipWhiteSpace(out buffer, ref line);
							StringIO.TryRead(out yllcorner, ref line);
						} else if (StringIO.TryRead("cellsize", ref line))
						{
							StringIO.SkipWhiteSpace(out buffer, ref line);
							StringIO.TryRead(out cellSize, ref line);
						} else if (StringIO.TryRead("NODATA_value", ref line))
						{
							StringIO.SkipWhiteSpace(out buffer, ref line);
							StringIO.TryRead(out nullValue, ref line);
						} else if (line.Length > 0)
						{
							data = true;
						}
					}
					if (data)
					{
						while (true)
						{
							string buffer;
							StringIO.SkipWhiteSpace(out buffer, ref line);
							if (string.IsNullOrWhiteSpace(line))
							{
								break;
							}
							double value;
							if (!StringIO.TryRead(out value, ref line))
							{
								throw new Exception("could not read line in file \"" + fileName + "\"");
							}
							values.Add(value);
						}
					}
				}
			}

			if (columns * rows != values.Count)
			{
				throw new Exception("Unknown or missing values in file \"" + fileName + "\"");
			}

			GridLayer layer = new GridLayer();
			for (int rowIndex = 0; rowIndex < rows; rowIndex++)
			{
				layer.Latitudes.Add((yllcorner + cellSize * rowIndex).ToString());
			}
			for (int columnIndex = 0; columnIndex < columns; columnIndex++)
			{
				layer.Longitudes.Add((xllcorner + cellSize * columnIndex).ToString());
			}

			layer.NullValue = nullValue;
			layer.Values = values;

			return layer;
		}
	}
}

