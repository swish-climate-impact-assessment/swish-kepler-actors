using System.IO;
using System.Windows.Forms;

namespace Swish.Tests
{
	static class TestFunctions
	{

		public static string TestDataFileName(string file)
		{
			string testDataDirectory = Path.Combine(Application.StartupPath, "Data");
			string fileName = Path.Combine(testDataDirectory, file);
			return fileName;
		}

	}
}
