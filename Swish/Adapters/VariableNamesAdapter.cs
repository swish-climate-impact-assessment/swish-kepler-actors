using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class VariableNamesAdapter: IAdapter
	{
		public string Name { get { return "VariableNames"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			SortedList<string, string> variableInformation = VariableInformation(inputFileName);

			Console.WriteLine(string.Join(Environment.NewLine, variableInformation.Keys));
		}

		public static SortedList<string/*name*/, string /*type*/> VariableInformation(string inputFileName)
		{
			if (string.IsNullOrWhiteSpace(inputFileName))
			{
				throw new Exception("inputFileName missing");
			}

			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			lines.Add("describe");

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);

			SortedList<string, string> variableInformation = ParseVariableNames(log);
	
			return  variableInformation;
		}

		private static SortedList<string/*name*/, string /*type*/> ParseVariableNames(string log)
		{
			SortedList<string, string > variableNames = new  SortedList<string,string>();
			List<string> lines = new List<string>(log.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

			/// Other  lines
			/// describe
			/// 
			/// Contains data
			///   obs:             4                          
			///  vars:             4                          
			///  size:            48                          
			/// ------------------------------------------------------------------
			///               storage  display     value
			/// variable name   type   format      label      variable label
			/// ------------------------------------------------------------------
			/// Date            str9   %9s                    
			/// CategoryA       str1   %9s                    
			/// CategoryB       str1   %9s                    
			/// Value           byte   %8.0g                  
			/// ------------------------------------------------------------------
			/// Other lines

			int lineIndex = 0;
			List<string> linesRead;
			ReadUntil(lines, out linesRead, ref lineIndex, ". describe");
			ReadUntil(lines, out linesRead, ref lineIndex, "---------------------------");
			ReadUntil(lines, out linesRead, ref lineIndex, "---------------------------");
			ReadUntil(lines, out linesRead, ref lineIndex, "---------------------------");
			if (linesRead.Count == 0)
			{
				throw new Exception("");
			}
			for (int variableIndex = 0; variableIndex < linesRead.Count; variableIndex++)
			{
				string line = linesRead[variableIndex].Trim();
				string[] fragments = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				string variableName = fragments[0];
				string variableType = fragments[1]; 

				variableNames.Add(variableName, variableType);
			}
			return variableNames;
		}

		private static void ReadUntil(List<string> lines, out List<string> linesRead, ref int lineIndex, string searchLine)
		{
			linesRead = new List<string>();
			while (true)
			{
				if (lineIndex == lines.Count)
				{
					throw new Exception("Could not find line \"" + searchLine + "\"" + Environment.NewLine + string.Join(Environment.NewLine, lines));
				}

				string line = lines[lineIndex++];
				if (line.Trim().StartsWith(searchLine))
				{
					break;
				}
				linesRead.Add(line);
			}
		}
	}
}
