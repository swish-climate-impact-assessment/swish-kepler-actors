using System;
using System.Collections.Generic;
using Swish.Adapters;
using Swish.IO;
using Swish.ScriptGenerators;
using Swish.Stata;

namespace Swish
{
	public static class TableFunctions
	{
		public static void Merge(string inputFileNameA, string inputFileNameB, List<string> mergeVariables, string outputFileName, bool keepMergeColumn)
		{
			Arguments arguments = new Arguments();
			inputFileNameA = SwishFunctions.ConvertInput(inputFileNameA);
			inputFileNameB = SwishFunctions.ConvertInput(inputFileNameB);
			arguments.String(StataScriptFunctions.Input1FileNameToken, inputFileNameA);
			arguments.String(StataScriptFunctions.Input2FileNameToken, inputFileNameB);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);
			string variables = StataScriptFunctions.VariableList(mergeVariables);
			arguments.String(StataScriptFunctions.VariableNamesToken, variables);
			arguments.String(StataScriptFunctions.IntermediateFileNameToken, FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension));

			string operation;
			if (!keepMergeColumn)
			{
				operation = MergeScriptGenerator.NameString;
			} else
			{
				operation = MergeKeepScriptGenerator.NameString;
			}

			OperationFunctions.RunOperation(operation, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void Append(string inputFileNameA, string inputFileNameB, string outputFileName)
		{
			Arguments arguments = new Arguments();
			inputFileNameA = SwishFunctions.ConvertInput(inputFileNameA);
			inputFileNameB = SwishFunctions.ConvertInput(inputFileNameB);
			arguments.String(StataScriptFunctions.Input1FileNameToken, inputFileNameA);
			arguments.String(StataScriptFunctions.Input2FileNameToken, inputFileNameB);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);

			OperationFunctions.RunOperation(AppendScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void Generate(string inputFileName, string outputFileName, string variableName, StataDataType type, string expression)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);
			arguments.String(GenerateScriptGenerator.ResultVariableNameToken, variableName);
			arguments.String(StataScriptFunctions.ExpressionToken, expression);

			if (type != StataDataType.Unknown)
			{
				arguments.String(GenerateScriptGenerator.TypeToken, type.ToString().ToLower());
			}

			OperationFunctions.RunOperation(GenerateScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void Replace(string inputFileName, string outputFileName, string condition, string value)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);
			arguments.String(ReplaceScriptGenerator.ConditionToken, condition);
			arguments.String(ReplaceScriptGenerator.ValueToken, value);

			OperationFunctions.RunOperation(ReplaceScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void SelectRecords(string inputFileName, string outputFileName, string expression)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);

			arguments.String(StataScriptFunctions.ExpressionToken, expression);

			OperationFunctions.RunOperation(SelectRecordsScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void SelectVariables(string inputFileName, string outputFileName, List<string> variables)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);

			arguments.String(StataScriptFunctions.VariableNamesToken, StataScriptFunctions.VariableList(variables));

			OperationFunctions.RunOperation(SelectVariablesScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void Sort(string inputFileName, List<string> variables, string outputFileName)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);

			arguments.String(StataScriptFunctions.VariableNamesToken, StataScriptFunctions.VariableList(variables));

			OperationFunctions.RunOperation(SortScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void Transpose(string inputFileName, string outputFileName)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);

			OperationFunctions.RunOperation(TransposeScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void FillDate(string inputFileName, string outputFileName, string dateVariableName)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);
			arguments.String(StataScriptFunctions.VariableNameToken, dateVariableName);

			OperationFunctions.RunOperation(FillDateScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void DateRange(string outputFileName, string variableName, DateTime startDate, DateTime endDate)
		{
			Arguments arguments = new Arguments();
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);

			arguments.Date(DateRangeScriptGenerator.StartDateToken, startDate);
			arguments.Date(DateRangeScriptGenerator.EndDateToken, endDate);
			arguments.String(StataScriptFunctions.ResultVariableNameToken, variableName);

			OperationFunctions.RunOperation(DateRangeScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void GraphSeries(string inputFileName, List<string> variables)
		{
			Arguments arguments = new Arguments();
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			arguments.StringList(StataScriptFunctions.VariableNamesToken, variables);

			OperationFunctions.RunOperation(GraphSeriesClientAdapter.OperationName, new OperationArguments(arguments));
		}

		public static void EditPostGreSqlPasswordFile()
		{
			OperationFunctions.RunOperation(Swish.Adapters.PostGreSqlPasswordFile.PostGreSqlPasswordFileEditAdapter.OperationName, new OperationArguments(new Arguments()));
		}

		public static void ReshapeLong(string inputFileName, string outputFileName, string variableNamePrefix, string i, string j)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);

			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);

			arguments.String(ReshapeLongScriptGenerator.VariableNamePrefixToken, variableNamePrefix);
			arguments.String(ReshapeLongScriptGenerator.IToken, i);
			arguments.String(ReshapeLongScriptGenerator.JToken, j);

			OperationFunctions.RunOperation(ReshapeLongScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void ReshapeWide(string inputFileName, string outputFileName, string variableNamePrefix, string i, string j)
		{
			Arguments arguments = new Arguments();
			inputFileName = SwishFunctions.ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);

			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameToken, intermediateFileName);

			arguments.String(ReshapeLongScriptGenerator.VariableNamePrefixToken, variableNamePrefix);
			arguments.String(ReshapeLongScriptGenerator.IToken, i);
			arguments.String(ReshapeLongScriptGenerator.JToken, j);

			OperationFunctions.RunOperation(ReshapeWideScriptGenerator.NameString, new OperationArguments(arguments));

			SwishFunctions.ConvertOutput(outputFileName, intermediateFileName);
		}

	}
}
