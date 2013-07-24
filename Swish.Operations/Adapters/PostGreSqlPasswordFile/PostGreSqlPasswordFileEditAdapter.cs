using System.Collections.Generic;
using System.IO;
using Swish.Controls;

namespace Swish.Adapters.PostGreSqlPasswordFile
{
	public class PostGreSqlPasswordFileEditAdapter: IOperation
	{
		public const string OperationName = "PostGreSqlPasswordFile";
		public string Name { get { return OperationName; } }

		public string Run(OperationArguments splitArguments)
		{
			string fileName = PostGreSqlPasswordFileFunctions.FileName();

			List<PostGreSqlPassword> passwords;
			if (File.Exists(fileName))
			{
				passwords = PostGreSqlPasswordFileFunctions.Read(fileName);
			} else
			{
				passwords = new List<PostGreSqlPassword>();
			}

			using (PostGreSqlPasswordFileEditor control = new PostGreSqlPasswordFileEditor())
			{
				control.Passwords = passwords;

				if (!DisplayForm.Display(control, "Edit Post Gre Sql Password file", true, true))
				{
					return string.Empty;
				}

				passwords = control.Passwords;
			}

			PostGreSqlPasswordFileFunctions.Write(fileName, passwords);

			return string.Empty;
		}
	}
}
