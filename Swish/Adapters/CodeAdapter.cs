using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Swish.Controls;
using System.Reflection;

namespace Swish.Adapters
{
	public class CodeAdapter: IAdapter
	{
		public const string OperationName = "Code";
		public string Name { get { return OperationName; } }

		public string Run(AdapterArguments splitArguments)
		{
			string declaringType = splitArguments.String("declaringType", true);
			string returnType = splitArguments.String("returnType", false);
			string functionName = splitArguments.String("functionName", true);

			List<string> argumentTypes = new List<string>();
			List<string> argumentNames= new List<string>();
			List<string> argumentInputs= new List<string>();

			int argumentIndex = 1;
			string argument1Type = splitArguments.String("argumentType"+argumentIndex.ToString(), false);
			string argument1Name = splitArguments.String("argument1Name" + argumentIndex.ToString(), false);
			string argument1Value = splitArguments.String("argument1Value" + argumentIndex.ToString(), false);


			if (string.IsNullOrWhiteSpace(returnType))
			{
				returnType = typeof(void).FullName;
			}


			string returnName = StataScriptFunctions.TemporaryVariableName();

			List<string> codeLines = new List<string>();
			codeLines.Add("//returnType: " + returnType);
			codeLines.Add("//returnName: " + returnName);
			codeLines.Add("int " + returnName + " = 0;");
			codeLines.Add("{");

			codeLines.Add("System.Windows.Forms. MessageBox.Show(\"This is a test\");");

			codeLines.Add("}");
			string fileName = FileFunctions.TempoaryOutputFileName(".cs");
			File.WriteAllLines(fileName, codeLines);

			return fileName;
		}

		public static string GetFunctionDetails()
		{
			Type type = typeof(System.Windows.Forms.MessageBox);

			MethodInfo[] functions = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

			MethodInfo function = functions[14];
			string thing = function.ReturnType.ToString() + " " + function.DeclaringType.ToString() + "." + function.Name + "(";
			ParameterInfo[] functionArguments = function.GetParameters();
			for (int argumentIndex = 0; argumentIndex < functionArguments.Length; argumentIndex++)
			{
				ParameterInfo argument = functionArguments[argumentIndex];

				thing += argument.ParameterType.ToString() + " " + argument.Name;
				if (argumentIndex + 1 < functionArguments.Length)
				{
					thing += ", ";
				}
			}
			thing += ")";
			return thing;
		}


	}
}
