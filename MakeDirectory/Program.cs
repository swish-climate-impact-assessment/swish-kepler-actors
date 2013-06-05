using System.IO;
using System.Windows.Forms;
using System;

namespace MakeDirectory
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				string path = string.Join(" ", arguments);

				path = path.Trim('\"');

				MakeDirectory(path);
			} catch{}
			return 0;
		}

		private static void MakeDirectory(string path)
		{
			if (string.IsNullOrWhiteSpace(path) ||  Directory.Exists(path))
			{
				return;
			}

			path = Path.GetFullPath(path);
			string basePath = Path.GetDirectoryName(path);
			MakeDirectory(basePath);

			Directory.CreateDirectory(path);
		}

	}
}
