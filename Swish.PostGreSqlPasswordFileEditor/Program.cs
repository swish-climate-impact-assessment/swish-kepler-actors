using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System;

namespace Swish.PostGreSqlPasswordFileEditor
{
	public static class Program
	{
		static int Main(string[] arguments)
		{
			Run();
			return 0;
		}

		public static void Run()
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
					return;
				}

				passwords = control.Passwords;
			}

			PostGreSqlPasswordFileFunctions.Write(fileName, passwords);
		}

	}
}
