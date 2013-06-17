using System;
using System.Diagnostics;

namespace Swish
{
	public class ProcessResult
	{
		public int ExitCode { get; set; }

		private string _output = string.Empty;
		public string Output
		{
			get { return _output; }
			set
			{
				if (string.IsNullOrWhiteSpace(_output))
				{
					_output = string.Empty;
					return;
				}
				_output = value;
			}
		}

		private string _error = string.Empty;
		public string Error
		{
			get { return _error; }
			set
			{
				if (string.IsNullOrWhiteSpace(_error))
				{
					_error = string.Empty;
					return;
				}
				_error = value;
			}
		}

		internal void ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(e.Data))
			{
				_error += e.Data;
			}
		}

		internal void OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(e.Data))
			{
				_output += e.Data;
			}
		}

	}
}
