using System;
using System.Collections.Generic;

namespace Swish
{
	public class MetadataRecord
	{
		private string _user = string.Empty;
		public string User
		{
			get { return _user; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_user = string.Empty;
					return;
				}
				_user = value;
			}
		}

		private string _pcName = string.Empty;
		public string ComputerName
		{
			get { return _pcName; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_pcName = string.Empty;
					return;
				}
				_pcName = value;
			}
		}

		private string _operation = string.Empty;
		public string Operation
		{
			get { return _operation; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_operation = string.Empty;
					return;
				}
				_operation = value;
			}
		}

		public DateTime Time { get; set; }

		private List<Tuple<string, string>> _arguments = new List<Tuple<string, string>>();
		public List<Tuple<string, string>> Arguments
		{
			get { return _arguments; }
			set
			{
				if (value == null)
				{
					_arguments = new List<Tuple<string, string>>();
					return;
				}
				_arguments = new List<Tuple<string, string>>(value);
			}
		}


	}
}
