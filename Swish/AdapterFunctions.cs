using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Swish
{
	public static class AdapterFunctions
	{
		public const string AppendOperation = "append";
		public const string CommandOperation = "stataCommand";
		public const string CompressOperation = "compress";
		public const string DisplayClientOperation = "displayClient";
		public const string DisplayOperation = "display";
		public const string DoScriptOperation = "doScript";
		public const string FormatOperation = "format";
		public const string GenerateOperation = "generate";
		public const string MergeOperation = "merge";
		public const string PasswordOperation = "password";
		public const string RemoveColumnsOperation = "removeColumns";
		public const string ReplaceOperation = "replace";
		public const string SaveOperation = "save";
		public const string SelectCloumnsOperation = "selectColumns";
		public const string SelectRecordsOperation = "selectRecords";
		public const string SortOperation = "sort";
		public const string TemporaryFileNameOperation = "temporaryFileName";
		public const string TestOperation = "test";
		public const string TransposeOperation = "transpose";

		public static void RunOperation(string operation, List<Tuple<string, string>> splitArguments)
		{
			switch (operation)
			{
			case FormatOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string format = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "format", splitArguments, true);
					AdapterFunctions.Format(inputFileName, outputFileName, variableNames, format);
					Console.Write(outputFileName);
				}
				break;

			case CompressOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.Compress(inputFileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case GenerateOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					string variableName = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "variable", splitArguments, true);
					string expression = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "expression", splitArguments, true);
					string type = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "type", splitArguments, false);
					AdapterFunctions.Generate(inputFileName, outputFileName, variableName, type, expression);
					Console.Write(outputFileName);
				}
				break;

			case PasswordOperation:
				{
					string password = AdapterFunctions.Password();
					Console.Write(password);
				}
				break;

			case TransposeOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.Transpose(inputFileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case SortOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "", splitArguments, true));
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					outputFileName = AdapterFunctions.Sort(inputFileName, variableNames, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case SelectCloumnsOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.SelectColumns(inputFileName, outputFileName, variableNames);
					Console.Write(outputFileName);
				}
				break;

			case SelectRecordsOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string expression = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "expression", splitArguments, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.Select(inputFileName, outputFileName, expression);
					Console.Write(outputFileName);
				}
				break;

			case SaveOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.SaveFile(inputFileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case MergeOperation:
				{
					string input1FileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "1", splitArguments, true));
					string input2FileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "2", splitArguments, true));
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					List<MergeRecordResult> keep = ArgumentFunctions.GetArgumentFlags<MergeRecordResult>(ArgumentFunctions.ArgumentCharacter + "keep", splitArguments, false, false);

					string keepMergeString = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "keepMerge", splitArguments, false));
					bool keepMerge;
					if (!string.IsNullOrWhiteSpace(keepMergeString))
					{
						keepMerge = bool.Parse(keepMergeString.ToLower());
					} else
					{
						keepMerge = false;
					}
					AdapterFunctions.Merge(input1FileName, input2FileName, variableNames, outputFileName, keepMerge, keep);
					Console.Write(outputFileName);
				}
				break;

			case DoScriptOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "filename", splitArguments, true));
					string log = StataFunctions.RunScript(inputFileName, false);
					Console.Write(log);
				}
				break;

			case CommandOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "", splitArguments, true));
					string command = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "command", splitArguments, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.StataCommand(inputFileName, outputFileName, command);
					Console.Write(outputFileName);
				}
				break;

			case AppendOperation:
				{
					string input1FileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "1", splitArguments, true));
					string input2FileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "2", splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					outputFileName = AdapterFunctions.Append(input1FileName, input2FileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case RemoveColumnsOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					RemoveColumns(inputFileName, outputFileName, variableNames);
					Console.Write(outputFileName);
				}
				break;

			case DisplayOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					Display(inputFileName);
				}
				break;

			case DisplayClientOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					DisplayClient(inputFileName);
				}
				break;

			case TestOperation:
				{
					string argumentText = string.Empty;
					for (int argumentIndex = 0; argumentIndex < splitArguments.Count; argumentIndex++)
					{
						Tuple<string, string> argument = splitArguments[argumentIndex];
						argumentText += argument.Item1 + " " + argument.Item2 + " ";
					}
					SwishFunctions.MessageTextBox(argumentText, false);
				}
				break;

			case TemporaryFileNameOperation:
				{
					string fileName = FileFunctions.TempoaryOutputFileName(string.Empty);
					if (FileFunctions.FileExists(fileName))
					{
						File.Delete(fileName);
					}
					Console.Write(fileName);
				}
				break;

			case ReplaceOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					string condition = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "condition", splitArguments, true);
					string value = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "value", splitArguments, true);
					Replace(inputFileName, outputFileName, condition, value);
					Console.Write(outputFileName);
				}
				break;

			default:
				throw new Exception("Unknown operation \"" + operation + "\"");
			}
		}

		private static void Format(string inputFileName, string outputFileName, List<string> variableNames, string format)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			if (string.IsNullOrWhiteSpace(format))
			{
				throw new Exception("Format missing");
			}


			string extension = Path.GetExtension(outputFileName);
			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(extension);

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			lines.Add("format " + StataScriptFunctions.VariableList(variableNames) + " " + format);

			string line = StataScriptFunctions.SaveFileCommand(intermaidateOutput);
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(intermaidateOutput))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			File.Move(intermaidateOutput, outputFileName);
		}

		private static void Compress(string inputFileName, string outputFileName)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			string extension = Path.GetExtension(outputFileName);
			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(extension);

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			lines.Add("compress");

			string line = StataScriptFunctions.SaveFileCommand(intermaidateOutput);
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(intermaidateOutput))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			File.Move(intermaidateOutput, outputFileName);
		}

		public static void Generate(string inputFileName, string outputFileName, string variableName, string type, string expression)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("Variable missing");
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new Exception("Expression missing");
			}

			string extension = Path.GetExtension(outputFileName);
			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(extension);

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			if (string.IsNullOrWhiteSpace(type))
			{
				lines.Add(" generate " + variableName + " =" + expression);
			} else
			{
				lines.Add(" generate " + type + " " + variableName + " =" + expression);
			}

			string line = StataScriptFunctions.SaveFileCommand(intermaidateOutput);
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(intermaidateOutput))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			File.Move(intermaidateOutput, outputFileName);
		}

		private static string Password()
		{

			using (MaskedTextBox textBox = new MaskedTextBox())
			using (Panel panel = new Panel())
			using (Button buton = new Button())
			using (Form form = new Form())
			{
				textBox.SuspendLayout();
				buton.SuspendLayout();
				panel.SuspendLayout();
				form.SuspendLayout();

				textBox.UseSystemPasswordChar = true;
				textBox.Multiline = true;
				textBox.SelectionStart = 0;
				textBox.SelectionLength = 0;
				textBox.Size = new Size(150, textBox.Height);
				textBox.Dock = DockStyle.Top;
				textBox.Font = new Font(textBox.Font, FontStyle.Bold);
				textBox.TabIndex = 0;

				buton.Click += new EventHandler(buton_Click);
				buton.Dock = DockStyle.Left;
				buton.Text = "Ok";
				buton.Size = new Size(75, 23);
				buton.TabIndex = 0;

				panel.Height = 23;
				panel.Controls.Add(buton);
				panel.Dock = DockStyle.Fill;
				panel.TabIndex = 1;

				form.ControlBox = false;
				form.Text = "Enter password";
				form.ClientSize = new Size(100, 43);
				form.Controls.Add(panel);
				form.Controls.Add(textBox);
				form.AcceptButton = buton;

				textBox.ResumeLayout();
				buton.ResumeLayout();
				panel.ResumeLayout();
				form.ResumeLayout();

				textBox.Focus();
				form.ShowDialog();

				return textBox.Text;
			}
		}

		static void buton_Click(object sender, EventArgs e)
		{
			Form form = SwishFunctions.GetForm(sender);
			if (form != null)
			{
				form.Close();
			}
		}

		public static void Display(string inputFileName)
		{
			int exitCode;
			string output;
			string arguments = string.Join(" ", ArgumentFunctions.OperationArgument, DisplayClientOperation, ArgumentFunctions.InputArgument, inputFileName);
			SwishFunctions.RunProcess(Application.ExecutablePath, arguments, Environment.CurrentDirectory, true, TimeSpan.Zero, out exitCode, out output);
		}

		public static void DisplayClient(string inputFileName)
		{
			string extension = Path.GetExtension(inputFileName);
			string useFileName;

			try
			{
				if (extension.ToLower() == ".csv")
				{
					useFileName = inputFileName;
				} else
				{
					useFileName = FileFunctions.TempoaryOutputFileName(".csv");
					SaveFile(inputFileName, useFileName);
				}

				string[] lines = File.ReadAllLines(useFileName);
				for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
				{
					lines[lineIndex] = lines[lineIndex].Replace(',', '\t');
				}

				SwishFunctions.MessageTextBox("File: " + inputFileName + Environment.NewLine, lines, false);
			} catch (Exception error)
			{
				SwishFunctions.MessageTextBox("failed to display " + inputFileName + Environment.NewLine + ExceptionFunctions.Write(error, true), false);
			}

		}

		public static void RemoveColumns(string inputFileName, string outputFileName, List<string> variableNames)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			string line = "drop " + StataScriptFunctions.VariableList(variableNames);
			lines.Add(line);
			line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}
		}

		public static void Transpose(string inputFileName, string outputFileName)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			lines.Add("xpose, clear");
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);

			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}
		}

		public static void Select(string inputFileName, string outputFileName, string expression)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new Exception("Expression missing");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			lines.Add("keep if " + expression);
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}
		}

		public static void SelectColumns(string inputFileName, string outputFileName, List<string> variableNames)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			string line = "keep " + StataScriptFunctions.VariableList(variableNames);
			lines.Add(line);
			line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}
		}

		public static void StataCommand(string inputFileName, string outputFileName, string command)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			/// create the do file
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			lines.Add(command);

			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);


			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}
		}

		public static void Merge(string input1FileName, string input2FileName,
			List<string> variableNames, string outputFileName, bool keepMergeColumn, List<MergeRecordResult> keep)
		{
			if (!FileFunctions.FileExists(input1FileName))
			{
				throw new Exception("cannot find file \"" + input1FileName + "\"");
			}

			if (!FileFunctions.FileExists(input2FileName))
			{
				throw new Exception("cannot find file \"" + input2FileName + "\"");
			}

			if (
			Path.GetFullPath(input1FileName) == Path.GetFullPath(outputFileName)
			|| Path.GetFullPath(input2FileName) == Path.GetFullPath(outputFileName)
			)
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			if (input1FileName == input2FileName)
			{
				throw new Exception("Cannot merge the same tables");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			string doOutputFileName = FileFunctions.TempoaryOutputFileName(Path.GetExtension(outputFileName));
			if (FileFunctions.FileExists(doOutputFileName))
			{
				// Stata does not overwrite files
				File.Delete(doOutputFileName);
			}

			string intermediateFileName = FileFunctions.TempoaryOutputFileName(".dta");
			if (FileFunctions.FileExists(intermediateFileName))
			{
				File.Delete(intermediateFileName);
			}

			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, input2FileName);

			string line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);
			line = StataScriptFunctions.SaveFileCommand(intermediateFileName);
			lines.Add(line);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, input1FileName);

			line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);

			line = "merge 1:1 " + StataScriptFunctions.VariableList(variableNames) + " using \"" + intermediateFileName + "\", force ";
			if (keep != null && keep.Count > 0)
			{
				line += "keep(";

				if (keep.Contains(MergeRecordResult.Match))
				{
					line += "match ";
				}
				if (keep.Contains(MergeRecordResult.MatchConflict))
				{
					line += "match_conflict ";
				}
				if (keep.Contains(MergeRecordResult.MatchUpdate))
				{
					line += "match_update ";
				}
				if (keep.Contains(MergeRecordResult.Using))
				{
					line += "using ";
				}

				line += ") ";
			}
			lines.Add(line);
			if (!keepMergeColumn)
			{
				lines.Add("drop " + StataScriptFunctions.MergeColumnName);
			}
			line = StataScriptFunctions.SaveFileCommand(doOutputFileName);
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(doOutputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			/// move the output file
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			File.Move(doOutputFileName, outputFileName);

			/// delete all the files not needed
			File.Delete(intermediateFileName);
		}

		public static string Append(string input1FileName, string input2FileName, string outputFileName)
		{
			if (!FileFunctions.FileExists(input1FileName))
			{
				throw new Exception("cannot find file \"" + input1FileName + "\"");
			}

			if (!FileFunctions.FileExists(input2FileName))
			{
				throw new Exception("cannot find file \"" + input2FileName + "\"");
			}

			if (
			Path.GetFullPath(input1FileName) == Path.GetFullPath(outputFileName)
			|| Path.GetFullPath(input2FileName) == Path.GetFullPath(outputFileName)
			)
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();

			StataScriptFunctions.WriteHeadder(lines);

			string intermediateFileName = StataScriptFunctions.ConvertToStataFormat(lines, input2FileName);
			StataScriptFunctions.LoadFileCommand(lines, input1FileName);
			lines.Add("append using \"" + intermediateFileName + "\"");
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			/// delete all the files not needed
			File.Delete(intermediateFileName);
			return outputFileName;
		}

		public static double Collapse(string inputFileName, string variable, CollapseOpperation operation)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			string doOutputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(doOutputFileName))
			{
				// Stata does not overwrite files
				File.Delete(doOutputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			// collapse (mean) mean=head4 (median) medium=head4, by(head6)

			lines.Add("collapse " + "(" + StataScriptFunctions.Write(operation) + ") " + variable + "_" + StataScriptFunctions.Write(operation) + "=" + variable);

			string line = StataScriptFunctions.SaveFileCommand(doOutputFileName);
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(doOutputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			string[] resultLines = File.ReadAllLines(doOutputFileName);
			if (resultLines.Length < 2)
			{
				throw new Exception("Unexpected format");
			}

			double result = double.Parse(resultLines[1]);

			/// delete all the files not needed
			File.Delete(doOutputFileName);
			return result;
		}

		public static string Sort(string inputFileName, List<string> variableNames, string outputFileName)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			/// create the do file
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			/// sort varlist, stable
			/// add variables names
			string line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);

			line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}
			return outputFileName;
		}

		public static void Replace(string inputFileName, string outputFileName, string condition, string value)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(condition))
			{
				throw new Exception("condition missing");
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new Exception("value missing");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			//replace oldvar =exp [if] [in] [, nopromote]

			lines.Add("replace " + value + " if " + condition);
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}
		}

		public static void SaveFile(string inputFileName, string outputFileName)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			string inputFileExtension = Path.GetExtension(inputFileName);
			string outputFileExtension = Path.GetExtension(outputFileName);

			if (inputFileExtension.ToLower() == outputFileExtension.ToLower())
			{
				File.Copy(inputFileName, outputFileName);
				return;
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}
		}


	}
}
