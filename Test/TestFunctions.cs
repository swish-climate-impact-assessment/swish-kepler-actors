using System.IO;
using System.Windows.Forms;

namespace Swish.Tests
{
	public static class TestFunctions
	{
		public static string TestDataFileName(string file)
		{
			string testDataDirectory = Path.Combine(Application.StartupPath, "Data");
			string fileName = Path.Combine(testDataDirectory, file);
			return fileName;
		}

		/// <summary>
		/// returns the filename of a test data file copied to the given (test) directory
		/// </summary>
		internal static string TestDataFile(string file, string directory)
		{
			string fileName = TestDataFileName( file);
			string destinationFileName = Path.Combine(directory, file);
			if (File.Exists(destinationFileName))
			{
				File.Delete(destinationFileName);
			}
			File.Copy(fileName, destinationFileName);
			return destinationFileName;
		}
	}
}

