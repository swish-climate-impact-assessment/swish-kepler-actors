using System;

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
	}
}
