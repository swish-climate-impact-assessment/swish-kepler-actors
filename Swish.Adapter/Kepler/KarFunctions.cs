using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Swish.IO;

namespace Swish.Kepler
{
	public static class KarFunctions
	{
		public static void DumpContents(string fileName, string directory, ReportProgressFunction ReportProgress)
		{
			using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (ZipInputStream zip = new ZipInputStream(file))
			{
				while (true)
				{
					ZipEntry item = zip.GetNextEntry();
					if (item == null)
					{
						break;
					}

					string itemDirectory = Path.GetDirectoryName(item.Name);
					string itemName = Path.GetFileName(item.Name);

					string outputDirectory = Path.Combine(directory, itemDirectory);
					FileFunctions.CreateDirectory(outputDirectory, ReportProgress);

					string outputName = Path.Combine(outputDirectory, itemName);
					StreamFunctions.ExportUntilFail(outputName, zip, ReportProgress);
				}

				zip.Close();
			}
		}

	}
}
