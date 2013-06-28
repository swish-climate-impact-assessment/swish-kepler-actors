using System;
using System.Collections.Generic;
using System.IO;
using Swish.IO;

namespace Swish.Adapters.PostGreSqlPasswordFile
{
	static class PostGreSqlPasswordFileFunctions
	{
		internal static string FileName()
		{
			string applicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string postGreSqlDirectory = Path.Combine(applicationDataDirectory, "postgresql");
			FileFunctions.CreateDirectory(postGreSqlDirectory, null);
			string postGreSqlPasswordFileName = Path.Combine(postGreSqlDirectory, "pgpass.conf");

			return postGreSqlPasswordFileName;
		}

		internal static List<PostGreSqlPassword> Read(string fileName)
		{
			string[] lines = File.ReadAllLines(fileName);

			List<PostGreSqlPassword> passwords = new List<PostGreSqlPassword>();
			for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
			{
				string line = lines[lineIndex];

				string[] fragments = line.Split(':');
				if (fragments.Length != 5)
				{
					continue;
					//throw new Exception("Bad password file, could not read line " + (lineIndex + 1).ToString() + " \"" + line + "\"");
				}

				PostGreSqlPassword password = new PostGreSqlPassword();
				password.Address = fragments[0];
				int port;
				if (!int.TryParse(fragments[1], out port))
				{
					continue;
				}
				password.Port = port;
				password.DatabaseName = fragments[2];
				password.UserName = fragments[3];
				password.Password = fragments[4];

				passwords.Add(password);
			}

			return passwords;
		}

		internal static void Write(string fileName, List<PostGreSqlPassword> passwords)
		{
			List<string> lines = new List<string>();
			for (int passwordIndex = 0; passwordIndex < passwords.Count; passwordIndex++)
			{
				// tern5.qern.qcif.edu.au:5432:ewedb:ian_szarka:Phurah30
				PostGreSqlPassword password = passwords[passwordIndex];

				string line = password.Address
					+ ":" + password.Port.ToString()
					+ ":" + password.DatabaseName
					+ ":" + password.UserName
					+ ":" + password.Password;

				lines.Add(line);
			}

			using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
				{
					string line = lines[lineIndex];
					byte[] buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(line);
					file.Write(buffer, 0, buffer.Length);
					if (lineIndex + 1 < lines.Count)
					{
						file.WriteByte((byte)'\n');
					}
				}
			}
		}
	}
}
