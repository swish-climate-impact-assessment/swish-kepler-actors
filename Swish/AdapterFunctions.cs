using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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

		public static void RunOperation(string operation, Arguments splitArguments)
		{
			switch (operation)
			{
			case FormatOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					string outputFileName = splitArguments.OutputFileName();
					List<string> variableNames = splitArguments.StringList(Arguments.DefaultArgumentPrefix + "variables", true, true);
					string format = splitArguments.String(Arguments.DefaultArgumentPrefix + "format", true);
					Format(inputFileName, outputFileName, variableNames, format);
					Console.Write(outputFileName);
				}
				break;

			case CompressOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					string outputFileName = splitArguments.OutputFileName();
					Compress(inputFileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case GenerateOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					string outputFileName = splitArguments.OutputFileName();
					string variableName = splitArguments.String(Arguments.DefaultArgumentPrefix + "variable", true);
					string expression = splitArguments.String(Arguments.DefaultArgumentPrefix + "expression", true);
					string type = splitArguments.String(Arguments.DefaultArgumentPrefix + "type", false);
					Generate(inputFileName, outputFileName, variableName, type, expression);
					Console.Write(outputFileName);
				}
				break;

			case PasswordOperation:
				{
					string prompt = splitArguments.String(Arguments.DefaultArgumentPrefix + "prompt", false);
					bool requireEntry = splitArguments.Bool(Arguments.DefaultArgumentPrefix + "ignoreCache", false);
					string password = Password(prompt, requireEntry);
					Console.Write(password);
				}
				break;

			case TransposeOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					string outputFileName = splitArguments.OutputFileName();
					Transpose(inputFileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case SortOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					List<string> variableNames = splitArguments.StringList(Arguments.DefaultArgumentPrefix + "variables", true, true);
					string outputFileName = splitArguments.OutputFileName();
					outputFileName = Sort(inputFileName, variableNames, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case SelectCloumnsOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					List<string> variableNames = splitArguments.StringList(Arguments.DefaultArgumentPrefix + "variables", true, true);
					string outputFileName = splitArguments.OutputFileName();
					SelectColumns(inputFileName, outputFileName, variableNames);
					Console.Write(outputFileName);
				}
				break;

			case SelectRecordsOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					string expression = splitArguments.String(Arguments.DefaultArgumentPrefix + "expression", true);
					string outputFileName = splitArguments.OutputFileName();
					Select(inputFileName, outputFileName, expression);
					Console.Write(outputFileName);
				}
				break;

			case SaveOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					string outputFileName = splitArguments.OutputFileName();
					SaveFile(inputFileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case MergeOperation:
				{
					string input1FileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument + "1", true));
					string input2FileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument + "2", true));
					List<string> variableNames = splitArguments.StringList(Arguments.DefaultArgumentPrefix + "variables", true, true);
					string outputFileName = splitArguments.OutputFileName();
					List<MergeRecordResult> keep = splitArguments.EnumList<MergeRecordResult>(Arguments.DefaultArgumentPrefix + "keep", false, false);

				string keepMergeString = FileFunctions.AdjustFileName(splitArguments.String(Arguments.DefaultArgumentPrefix + "keepMerge", false));
					bool keepMerge;
					if (!string.IsNullOrWhiteSpace(keepMergeString))
					{
						keepMerge = bool.Parse(keepMergeString.ToLower());
					} else
					{
						keepMerge = false;
					}

					Merge(input1FileName, input2FileName, variableNames, outputFileName, keepMerge, keep);
					Console.Write(outputFileName);
				}
				break;

			case DoScriptOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.DefaultArgumentPrefix + "filename", true));
					string log = StataFunctions.RunScript(inputFileName, false);
					Console.Write(log);
				}
				break;

			case CommandOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					string command = splitArguments.String(Arguments.DefaultArgumentPrefix + "command", true);
					string outputFileName = splitArguments.OutputFileName();
					StataCommand(inputFileName, outputFileName, command);
					Console.Write(outputFileName);
				}
				break;

			case AppendOperation:
				{
					string input1FileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument + "1", true));
					string input2FileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument + "2", true));
					string outputFileName = splitArguments.OutputFileName();
					outputFileName = Append(input1FileName, input2FileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case RemoveColumnsOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					List<string> variableNames = splitArguments.StringList(Arguments.DefaultArgumentPrefix + "variables", true, true);
					string outputFileName = splitArguments.OutputFileName();
					RemoveColumns(inputFileName, outputFileName, variableNames);
					Console.Write(outputFileName);
				}
				break;

			case DisplayOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					Display(inputFileName);
				}
				break;

			case DisplayClientOperation:
				{
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					DisplayClient(inputFileName);
				}
				break;

			case TestOperation:
				{
					bool silent = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "silent");

					List<string> lines = new List<string>();
					lines.Add("Arguments: " + splitArguments.ArgumentString);
					lines.Add("Startup path: " + Application.StartupPath);
					lines.Add("Working directory: " + Environment.CurrentDirectory);

					if (ProcessFunctions.KeplerProcess != null)
					{
						lines.Add("Keper process:");
						lines.Add(ProcessFunctions.WriteProcessInformation(ProcessFunctions.KeplerProcess));
					} else
					{
						lines.Add("Keper process: Not found");
					}

					lines.Add("Current process heritage: ");

					lines.AddRange(ProcessFunctions.WriteProcessHeritage().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
					lines.AddRange(ProcessFunctions.WriteSystemVariables().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
					if (!silent)
					{
						SwishFunctions.MessageTextBox("Test display", lines, false);
					}
					Console.Write(string.Join(Environment.NewLine, lines));
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
					string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
					string outputFileName = splitArguments.OutputFileName();
					string condition = splitArguments.String(Arguments.DefaultArgumentPrefix + "condition", true);
					string value = splitArguments.String(Arguments.DefaultArgumentPrefix + "value", true);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("VariableNames", string.Join(" ", variableNames)));
			record.Arguments.Add(new Tuple<string, string>("format", format));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));

			record.ComputerName = Environment.MachineName;
			record.Operation = "Format";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Compress";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("Variable", variableName));
			record.Arguments.Add(new Tuple<string, string>("Type", type));
			record.Arguments.Add(new Tuple<string, string>("Expression", expression));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Generate";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
		}

		/// <summary>
		/// force means ignore any cache and require the user to enter a password
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="force"></param>
		/// <returns></returns>
		public static string Password(string prompt, bool requireEntry)
		{
			string passwordFileName;
			if (!string.IsNullOrWhiteSpace(prompt))
			{
				if (ProcessFunctions.KeplerProcess != null)
				{
					passwordFileName = SwishFunctions.GeneratePasswordFileName(prompt, ProcessFunctions.KeplerProcess);
					passwordFileName = Path.Combine(Path.GetTempPath(), passwordFileName);

					if (!requireEntry && FileFunctions.FileExists(passwordFileName))
					{
						string _encodedPassword = File.ReadAllText(passwordFileName);
						string _password = SwishFunctions.DecodePassword(_encodedPassword, ProcessFunctions.KeplerProcess);
						return _password;
					}
				} else
				{
					passwordFileName = string.Empty;
				}
			} else
			{
				prompt = "Please enter password";
				passwordFileName = string.Empty;
			}

			string password;

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
				textBox.Size = new Size(300, textBox.Height);
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
				form.Text = prompt;
				form.ClientSize = new Size(300, 43);
				form.Controls.Add(panel);
				form.Controls.Add(textBox);
				form.AcceptButton = buton;

				textBox.ResumeLayout();
				buton.ResumeLayout();
				panel.ResumeLayout();
				form.ResumeLayout();

				textBox.Focus();
				form.ShowDialog();

				password = textBox.Text;
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				return string.Empty;
			}

			if (string.IsNullOrWhiteSpace(passwordFileName))
			{
				return password;
			}

			string encodedPassword = SwishFunctions.EncodePassword(password, ProcessFunctions.KeplerProcess);
			if (File.Exists(passwordFileName))
			{
				FileFunctions.DeleteFile(passwordFileName, null);
			}
			File.WriteAllText(passwordFileName, encodedPassword);

			return password;
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
			string arguments = string.Join(" ", Arguments.OperationArgument, DisplayClientOperation, Arguments.InputArgument, inputFileName);
			ProcessFunctions.Run(Application.ExecutablePath, arguments, Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, true);
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
				string message = "failed to display " + inputFileName + Environment.NewLine + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose);
				//if (ExceptionFunctions.ForceVerbose)
				//{
				//    message += ProcessFunctions.WriteProcessHeritage();
				//    message += ProcessFunctions.WriteSystemVariables();
				//}

				SwishFunctions.MessageTextBox(message, false);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("VariableNames", string.Join(" ", variableNames)));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "RemoveColumns";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Transpose";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("Expression", expression));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Select";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("VariableNames", string.Join(" ", variableNames)));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "SelectColumns";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("Command", command));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "StataCommand";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
		}

		public static void Merge(string input1FileName, string input2FileName, List<string> variableNames, string outputFileName, bool keepMergeColumn, List<MergeRecordResult> keep)
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(input1FileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(input1FileName);
				metadata.AddRange(inputMetadata);
			}

			if (MetadataFunctions.Exists(input2FileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(input2FileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", input1FileName));
			record.Arguments.Add(new Tuple<string, string>("InputFileName", input2FileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("VariableNames", string.Join(" ", variableNames)));
			record.Arguments.Add(new Tuple<string, string>("KeepMergeColumn", keepMergeColumn.ToString()));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			if (keep != null)
			{
				string keepString = string.Join(" ", keep);
				record.Arguments.Add(new Tuple<string, string>("Keep", keepString));
			}
			record.ComputerName = Environment.MachineName;
			record.Operation = "Merge";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
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

			List<MetadataRecord> metadata = new List<MetadataRecord>();

			if (MetadataFunctions.Exists(input1FileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(input1FileName);
				metadata.AddRange(inputMetadata);
			}

			if (MetadataFunctions.Exists(input2FileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(input2FileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", input1FileName));
			record.Arguments.Add(new Tuple<string, string>("InputFileName", input2FileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Append";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);

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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("Variable", variable));
			record.Arguments.Add(new Tuple<string, string>("Operation", operation.ToString()));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Collapse";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(inputFileName + "out", metadata);

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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("VariableNames", string.Join(" ", variableNames)));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Sort";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);

			return outputFileName;
		}

		public static void Replace(string inputFileName, string outputFileName, string condition, string value)
		{
			//SwishFunctions.MessageTextBox(""
			//+ "inputFileName: " + inputFileName + Environment.NewLine
			//+ "outputFileName: " + outputFileName + Environment.NewLine
			//+ "condition: " + condition + Environment.NewLine
			//+ "value: " + value + Environment.NewLine, false);

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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("Condition", condition));
			record.Arguments.Add(new Tuple<string, string>("Value", value));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Replace";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);

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

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			if (MetadataFunctions.Exists(inputFileName))
			{
				List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
				metadata.AddRange(inputMetadata);
			}

			MetadataRecord record = new MetadataRecord();
			record.Arguments.Add(new Tuple<string, string>("InputFileName", inputFileName));
			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			record.ComputerName = Environment.MachineName;
			record.Operation = "Save";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
		}
	}
}

