using System;
using System.Collections.Generic;
using System.IO;
using Swish.IO;

namespace Swish.Adapters
{
	public class RunCodeAdapter: IOperation
	{
		public const string OperationName = "RunCode";
		public string Name { get { return OperationName; } }

		public string Run(OperationArguments splitArguments)
		{
			string input = splitArguments.String("code", true);

			List<string> lines = new List<string>(File.ReadAllLines(input));

			SwishFunctions.MessageTextBox(" lines: ", lines, false);

			if (lines.Count < 1)
			{
				throw new Exception();
			}

			string returnType;
			string returnName;
			ReadHeadder(out returnType, out returnName, lines);

			SwishFunctions.MessageTextBox(" returnType: " + returnType + Environment.NewLine +
			"returnName: " + returnName, false);
			List<string> codeLines = new List<string>();

			codeLines.Add("public class Program");
			codeLines.Add("{");
			codeLines.Add("static int Main(string[] arguments)");
			codeLines.Add("{");

			codeLines.AddRange(lines);

			if (!string.IsNullOrWhiteSpace(returnType) && returnType != typeof(void).FullName)
			{
				if (string.IsNullOrWhiteSpace(returnName))
				{
					throw new Exception("return variable name missing from \"" + input + "\"");
				}
				codeLines.Add("return " + returnName + ";");
			} else
			{
				codeLines.Add("return 0;");
			}
			codeLines.Add("}");
			codeLines.Add("}");

			string code = string.Join(Environment.NewLine, codeLines);
			string executableFileName = CSharpCompiler.MakeExecutable(code);

			ProcessFunctions.Run(executableFileName, string.Empty, string.Empty, false, TimeSpan.Zero, false, false, true);

			string directory = Path.GetDirectoryName(executableFileName);
			FileFunctions.DeleteDirectoryAndContents(directory, null);
			return string.Empty;
		}

		private static void ReadHeadder(out string returnType, out string returnName, List<string> lines)
		{
			returnType = string.Empty;
			returnName = string.Empty;

			while (lines.Count > 0)
			{
				string line = lines[0];

				if (line.Contains("returnType"))
				{
					lines.RemoveAt(0);
					int colonIndex = line.IndexOf(':');
					returnType = line.Substring(colonIndex + 1).Trim();
				} else if (line.Contains("returnName"))
				{
					lines.RemoveAt(0);
					int colonIndex = line.IndexOf(':');
					returnName = line.Substring(colonIndex + 1).Trim();
				} else if (!string.IsNullOrWhiteSpace(line))
				{
					break;
				}
			}
		}
	}
}
