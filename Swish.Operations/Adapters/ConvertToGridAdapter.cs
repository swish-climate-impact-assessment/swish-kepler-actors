using System.IO;
using Swish.IO;

namespace Swish.Adapters
{
	public class ConvertToGrid: IOperation
	{
		public string Name { get { return "ConvertToGrid"; } }

		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string variableName = splitArguments.VariableName();
			string longitudeName = splitArguments.String("LongitudeVariableName", true);
			string latitudeName = splitArguments.String("LatitudeVariableName", true);

			string output = Convert(inputFileName, variableName, longitudeName, latitudeName);

			return output;
		}

		/// <summary>
		/// force means ignore any cache and require the user to enter a password
		/// </summary>
		public static string Convert(string inputFileName, string variableName, string longitudeName, string latitudeName)
		{
			string extension = Path.GetExtension(inputFileName);
			string useFileName;
			if (extension.ToLower() == SwishFunctions.CsvFileExtension)
			{
				useFileName = inputFileName;
			} else
			{
				useFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
				SwishFunctions.Save(inputFileName, useFileName);
			}

			Table table = CsvFunctions.Read(useFileName, true);

			int valueColumnIndex = table.ColumnIndex(variableName, true);
			int longitudeColumnIndex = table.ColumnIndex(longitudeName, true);
			int latitudeColumnIndex = table.ColumnIndex(latitudeName, true);

			GridLayer layer = GridFunctions.ConvertToLayer(table, longitudeColumnIndex, latitudeColumnIndex, valueColumnIndex);

			string fileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.AscGridFileExtension);
			GridFunctions.Write(fileName, layer);

			return fileName;
		}

	}
}
