using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Delete
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			string deletePath = string.Join(" ", arguments).Trim('\"');

			List<string> files = new List<string>();
			if (deletePath.Contains("*"))
			{
				string directory = Path.GetDirectoryName(deletePath);
				string searchString = Path.GetFileName(deletePath);
				string[] filePaths = Directory.GetFiles(directory, searchString);
				files.AddRange(filePaths);
			} else
			{
				files.Add(deletePath);
			}
			for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
			{
				string fileName = files[fileIndex];
				try
				{
					File.Delete(fileName);
				} catch { }
			}

			return 0;
		}

	}
}
