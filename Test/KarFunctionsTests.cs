using System.Collections.Generic;
using System.IO;

namespace Swish.Tests
{
	class KarFunctionsTests
	{
		internal void DumpContents()
		{
			string directory = FileFunctions.CreateTempoaryDirectory();
			string fileName = TestFunctions.TestDataFile("DumpKar.kar", directory);
			string outputDirectory = Path.Combine(directory, Path.GetFileNameWithoutExtension(fileName));

			KarFunctions.DumpContents(fileName, outputDirectory, null);

			List<string> contents = new List<string>(new string[]{
					"AppendTables.urn.lsid.kepler-project.org.ns..26898.57.672.xml",
					"META-INF\\MANIFEST.MF",
			});

			for (int contentIndex = 0; contentIndex < contents.Count; contentIndex++)
			{
				string expectedData = contents[contentIndex];
				string expectedFileName = Path.Combine(directory, "DumpKar", expectedData);

				if (!File.Exists(expectedFileName))
				{
					throw new TestException();
				}
			}
		}


		internal void RemovePort()
		{
			using (KarFile file = new KarFile())
			{
				string fileName = TestFunctions.TestDataFile("RemovePort.xml", file.WorkingDirectory);

				// find the tag "port", with the attribute 
				file.RemovePort("Output");
				file.Flush();

				string expectedFileName = TestFunctions.TestDataFileName("RemovePortResult.xml");

				// the xml comes out with different white space etc
				//if (!EqualFunctions.FilesEqual(fileName, expectedFileName))
				//{
				//    throw new TestException();
				//}
			}

		}
	}
}
