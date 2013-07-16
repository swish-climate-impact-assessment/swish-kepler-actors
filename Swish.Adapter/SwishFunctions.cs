using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Swish.IO;
using Swish.Stata;

namespace Swish
{
	public static class SwishFunctions
	{
		public const string DataFileExtension = ".dta";
		public const string CsvFileExtension = ".csv";
		public const string DoFileExtension = ".do";
		public const string AscGridFileExtension = ".asc";

		public static void MessageTextBox(string message, bool returnImediatly)
		{
			MessageTextBox("Message", message.Split('\r', '\n'), returnImediatly);
		}

		public static void MessageTextBox(string title, IList<string> lines, bool returnImediatly)
		{
			TextBox textBox = new TextBox();
			Form form = new Form();

			textBox.Multiline = true;
			int limit = 1000;
			if (lines.Count <= limit)
			{
				textBox.Lines = new List<string>(lines).ToArray();
			} else
			{
				string[] shortLines = new string[limit + 2];
				for (int lineIndex = 0; lineIndex < limit; lineIndex++)
				{
					shortLines[lineIndex] = lines[lineIndex];
				}
				shortLines[limit] = string.Empty;
				shortLines[limit + 1] = "Note, text truncated at 1000 lines.";

				textBox.Lines = shortLines;
			}
			textBox.SelectionStart = 0;
			textBox.SelectionLength = 0;
			textBox.ScrollBars = ScrollBars.Both;
			textBox.Size = new Size(400, 400);
			textBox.Dock = DockStyle.Fill;
			textBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			Size size = textBox.Size;
			int controlHeight = size.Height;

			form.ControlBox = true;
			form.KeyPreview = true;
			form.KeyUp += form_KeyUp;
			form.Text = title;
			size = textBox.Size;
			form.ClientSize = new Size(size.Width, controlHeight);
			form.Controls.Add(textBox);
			textBox.BringToFront();

			if (returnImediatly)
			{
				form.Show();
				Application.DoEvents();
				Thread.Sleep(100);
			} else
			{
				using (textBox)
				using (form)
				{
					try
					{
						form.ShowDialog();
					} finally
					{
						bool flag = form == null;
						if (!flag)
						{
							form.Dispose();
						}
					}
				}
			}
		}

		static void form_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				Form form = GetForm(sender);
				if (form != null)
				{
					form.DialogResult = DialogResult.OK;
					form.Close();
				}
			}
		}

		public static Form GetForm(object sender)
		{
			Control parent = (Control)sender;
			while (true)
			{
				if (parent == null)
				{
					return null;
				}
				if (parent is Form)
				{
					return parent as Form;
				}
				parent = parent.Parent;
			}
		}

		public static string GeneratePasswordFileName(string prompt, Process process)
		{
			int encodeLength = 8;
			byte[] encodedBytes = new byte[encodeLength];

			byte[] promptBytes = ASCIIEncoding.ASCII.GetBytes(prompt);
			byte[] idBytes = BitConverter.GetBytes((short)process.Id);

			for (int byteIndex = 0; byteIndex < promptBytes.Length; byteIndex++)
			{
				byte value = (byte)(promptBytes[byteIndex] ^ idBytes[byteIndex % idBytes.Length] ^ encodedBytes[byteIndex % encodeLength]);
				encodedBytes[byteIndex % encodeLength] = value;
			}

			string encodedFileName;
			using (MemoryStream _stream = new MemoryStream())
			{
				using (TextCodedStream stream = new TextCodedStream(_stream, true, true))
				{
					stream.Write(encodedBytes, 0, encodeLength);
					stream.Flush();
				}
				_stream.Position = 0;
				encodedFileName = StreamFunctions.ToString(_stream);
			}

			return encodedFileName;
		}

		public static string EncodePassword(string password, Process process)
		{
			byte[] passwordBytes = ASCIIEncoding.ASCII.GetBytes(password);
			byte[] encodedBytes = MangleBytes(passwordBytes, process);

			string encodedPassword;
			using (MemoryStream _stream = new MemoryStream())
			{
				using (TextCodedStream stream = new TextCodedStream(_stream, true, true))
				{
					stream.Write(encodedBytes, 0, encodedBytes.Length);
					stream.Flush();
				}
				_stream.Position = 0;
				encodedPassword = StreamFunctions.ToString(_stream);
			}

			return encodedPassword;
		}

		public static byte[] MangleBytes(byte[] bytes, Process process)
		{
			byte[] nameBytes = ASCIIEncoding.ASCII.GetBytes(process.ProcessName);
			byte[] timeBytes = BitConverter.GetBytes(process.StartTime.Ticks);

			byte[] mangledBytes = new byte[bytes.Length];

			for (int byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
			{
				byte value = (byte)(bytes[byteIndex] ^ nameBytes[byteIndex % nameBytes.Length] ^ timeBytes[byteIndex % timeBytes.Length]);
				mangledBytes[byteIndex] = value;
			}

			return mangledBytes;
		}

		public static string DecodePassword(string encodedPassword, Process process)
		{
			byte[] encodedBytes;
			using (TextAsStream _stream = new TextAsStream(encodedPassword))
			using (TextCodedStream stream = new TextCodedStream(_stream, false, false))
			{
				List<byte> _encodedBytes = new List<byte>();

				int bufferSize = 1;
				byte[] buffer = new byte[bufferSize];
				while (true)
				{
					int countRead = stream.Read(buffer, 0, bufferSize);
					if (countRead != bufferSize)
					{
						break;
					}
					_encodedBytes.Add(buffer[0]);
				}

				encodedBytes = _encodedBytes.ToArray();
			}
			byte[] passwordBytes = MangleBytes(encodedBytes, process);

			string password = ASCIIEncoding.ASCII.GetString(passwordBytes);
			return password;
		}

		public static List<string> ConvertLines(List<string> lines, List<Tuple<string, string>> symbols, ReportProgressFunction ReportMessage, bool optionalNameExtension, bool escapeStrings, bool reportChanges)
		{
			List<string> newPending = new List<string>();
			for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
			{
				string _line = lines[lineIndex];
				string line = ResloveSymbols(_line, symbols, escapeStrings, optionalNameExtension);

				if (reportChanges && line != _line)
				{
					_ReportMessage(ReportMessage, -1, "\"" + _line + "\"" + " changed to " + "\"" + line + "\"");
				}
				newPending.Add(line);
			}
			return newPending;
		}

		public static string ResloveSymbols(string line, List<Tuple<string, string>> symbols, bool escapeStrings, bool optionalNameExtension)
		{
			for (int symbolIndex = 0; symbolIndex < symbols.Count; symbolIndex++)
			{
				string symbol = symbols[symbolIndex].Item1;
				string value;
				if (!escapeStrings)
				{
					value = symbols[symbolIndex].Item2;
				} else
				{
					value = StringIO.Escape(symbols[symbolIndex].Item2);
				}
				line = line.Replace(symbol, value);
				if (optionalNameExtension)
				{
					if (symbol.ToLower().EndsWith("name%"))
					{
						string testSymbol = symbol.Substring(0, symbol.Length - "name%".Length) + "%";
						line = line.Replace(testSymbol, value);
					} else if (symbol.ToLower().EndsWith("names%"))
					{
						string testSymbol = symbol.Substring(0, symbol.Length - "names%".Length) + "%";
						line = line.Replace(testSymbol, value);
					} else
					{
						string testSymbol = symbol.Substring(0, symbol.Length - "%".Length) + "Name%";
						line = line.Replace(testSymbol, value);
						testSymbol = symbol.Substring(0, symbol.Length - "%".Length) + "Names%";
						line = line.Replace(testSymbol, value);
						testSymbol = symbol.Substring(0, symbol.Length - "%".Length) + "names%";
						line = line.Replace(testSymbol, value);
						testSymbol = symbol.Substring(0, symbol.Length - "%".Length) + "name%";
						line = line.Replace(testSymbol, value);
					}
				}
			}

			return line;
		}

		public static void _ReportMessage(ReportProgressFunction ReportMessage, int progress, string message)
		{
			if (ReportMessage != null)
			{
				ReportMessage(progress, message);
			} else if (ExceptionFunctions.ForceVerbose)
			{
				Console.WriteLine(message);
			} else if (progress >= 0)
			{
				//Console.WriteLine(progress + "%" + message);
			}
		}

		private static int _temporaryVariableId = Math.Abs((int)DateTime.Now.Ticks);
		public static int TemporaryVariableId()
		{
			int value = _temporaryVariableId;
			_temporaryVariableId++;
			return value;
		}

		public static void SetTemporaryVariableId(int value)
		{
			_temporaryVariableId = value;
		}


		public static string ConvertToCsvFile(string fileName)
		{
			string useInput;

			string extension = Path.GetExtension(fileName).ToLower();
			if (extension == SwishFunctions.CsvFileExtension.ToLower())
			{
				useInput = fileName;
			} else
			{
				useInput = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
				Save(fileName, useInput);
			}
			return useInput;
		}

		public static void Save(string inputFileName, string outputFileName)
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
			StataScriptFunctions.SaveFileCommand(lines, outputFileName);

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
			if (OperationFunctions.RecordDoScript) { record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines))); }
			record.ComputerName = Environment.MachineName;
			record.Operation = "Save";
			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);
		}

		public static void ConvertOutput(string destination, string source)
		{
			string sourceExtension = Path.GetExtension(source).ToLower();
			string destinationExtension = Path.GetExtension(destination).ToLower();

			if (FileFunctions.FileExists(destination))
			{
				File.Delete(destination);
			}

			if (sourceExtension == destinationExtension)
			{
				File.Move(source, destination);
				return;
			}

			Save(source, destination);
		}

		public static string ConvertInput(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLower();
			if (extension == SwishFunctions.DataFileExtension)
			{
				return fileName;
			}
			string usedFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			Save(fileName, usedFileName);
			return usedFileName;
		}

	}
}