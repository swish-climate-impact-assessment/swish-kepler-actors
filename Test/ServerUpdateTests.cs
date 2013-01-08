using System;
using System.IO;
using System.Windows.Forms;
using Swish.Server.ServerCommands;

namespace Swish.Tests
{
	class ServerUpdateTests
	{
		internal void ExchangeFiles()
		{
			string fileName = Path.Combine(Application.StartupPath, "ExchangeFiles.File.txt");
			string text = string.Empty;
			int dataLength = 100;
			for (int textIndex = 0; textIndex < dataLength; textIndex++)
			{
				text += (char)('A' + textIndex % 26);
			}
			File.WriteAllText(fileName, text);

			using (MemoryStream stream = new MemoryStream())
			{
				UpdateServerCommand.SendFile(stream, fileName);
				long position1 = stream.Position;

				long expected = "100".Length + "\r\n".Length + dataLength + "\r\n".Length + "0".Length + "\r\n".Length + "\r\n".Length;

				UpdateServerCommand.SendFile(stream, fileName);
				long position2 = stream.Position;

				stream.Position = 0;

				string baseDirectory = Path.Combine(Path.GetTempPath(), "ExchangeFiles");
				FileFunctions.CreateDirectory(baseDirectory,null);
				UpdateServerCommand.ReceiveFile(stream, baseDirectory);
				if (stream.Position != position1)
				{
					throw new Exception();
				}

				fileName = Path.Combine(baseDirectory, "ExchangeFiles.File.txt");
				if (!File.Exists(fileName))
				{
					throw new Exception();
				}
				byte[] bytes = File.ReadAllBytes(fileName);

				if (bytes.Length != 100)
				{
					throw new Exception();
				}

				for (int textIndex = 0; textIndex < dataLength; textIndex++)
				{
					char character = (char)('A' + textIndex % 26);
					if (bytes[textIndex] != character)
					{
						throw new Exception();
					}
				}

				UpdateServerCommand.ReceiveFile(stream, baseDirectory);
				if (stream.Position != position2)
				{
					throw new Exception();
				}
			}

		}
	}
}
