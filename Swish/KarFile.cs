using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;

namespace Swish
{
	public class KarFile: IDisposable
	{
		public KarFile(string fileName)
		{
			_fileName = fileName;
			_directory = FileFunctions.CreateTempoaryDirectory();
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

					string outputDirectory = Path.Combine(_directory, itemDirectory);
					FileFunctions.CreateDirectory(outputDirectory);

					string outputName = Path.Combine(outputDirectory, itemName);
					StreamFunctions.ExportUntilFail(outputName, zip);
				}

				zip.Close();
			}
		}

		public KarFile()
		{
			_directory = FileFunctions.CreateTempoaryDirectory();
		}

		private string _directory;
		private string _fileName;

		~KarFile()
		{
			Dispose(false);
		}

		public void Close()
		{
			Dispose(true);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				if (!string.IsNullOrWhiteSpace(_fileName))
				{
					throw new Exception("KarFile not closed ");
				}
				return;
			}
			GC.SuppressFinalize(this);

			foreach (string fileName in Directory.GetFiles(_directory))
			{
				File.Delete(fileName);
			}
			Directory.Delete(_directory);

			_fileName = null;
			_directory = null;
		}

		public void Save()
		{
			// update the files on disk, and package them into a kar file, saved at _fileName
			throw new NotImplementedException();
		}

		public string WorkingDirectory { get { return _directory; } }

		public void RemovePort(string name)
		{
			string[] files = Directory.GetFiles(_directory, "*.xml");
			if (files.Length != 1)
			{
				throw new Exception("muli actor kar files not supported");
			}

			string fileName = files[0];
			XmlDocument _document = OpenDocument(fileName);

			for (int itemGroupIndex = 0; itemGroupIndex < _document.DocumentElement.ChildNodes.Count; itemGroupIndex++)
			{
				XmlNode node = _document.DocumentElement.ChildNodes[itemGroupIndex];
				if (node.Name != "port")
				{
					continue;
				}

				bool found = false;
				for (int attributeIndex = 0; attributeIndex < node.Attributes.Count; attributeIndex++)
				{
					XmlAttribute attribute = node.Attributes[attributeIndex];
					if (attribute.Name != "name")
					{
						continue;
					}

					if (attribute.Value != name)
					{
						break;
					}

					_document.DocumentElement.RemoveChild(node);
					found = true;
					break;
				}

				if (found)
				{
					break;
				}
			}

			CloseDocument(fileName, _document);
		}

		private static void CloseDocument(string fileName, XmlDocument _document)
		{
			_document.Save(fileName);

			string contents = File.ReadAllText(fileName);
			contents = contents.Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\"?><!DOCTYPE entity PUBLIC \"-//UC Berkeley//DTD MoML 1//EN\" \"http://ptolemy.eecs.berkeley.edu/xml/dtd/MoML_1.dtd\">");
			File.WriteAllText(fileName, contents);
		}

		private static XmlDocument OpenDocument(string fileName)
		{
			string contents = File.ReadAllText(fileName);
			contents= contents.Replace("<!DOCTYPE entity PUBLIC \"-//UC Berkeley//DTD MoML 1//EN\"", string.Empty);
			contents= contents.Replace("\"http://ptolemy.eecs.berkeley.edu/xml/dtd/MoML_1.dtd\">", string.Empty);
			File.WriteAllText(fileName, contents);

			XmlDocument _document = new XmlDocument();
			_document.Load(fileName);

			if (_document.DocumentElement.Name != "entity")
			{
				throw new Exception();
			}
			return _document;
		}

		public void Flush()
		{
			// write any contents in memory back to disk, 
			// I expect that parts will be loaded and kept in memory for speed
			// this forces the files to be synchronised
		}
	}
}
