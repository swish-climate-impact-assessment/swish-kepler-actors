using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Swish.IO;

namespace Swish.Stata
{
	public class ScriptSymbol
	{
		public ScriptSymbol(SymbolType type, bool optional, string name, string defaultValue)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException("name");
			}
			Type = type;
			Optional = optional;
			_name = name;
			DefaultValue = defaultValue;
		}

		private string _name = string.Empty;
		public string Name
		{
			get { return _name; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_name = string.Empty;
					return;
				}
				_name = value;
			}
		}

		public bool Optional { get; set; }

		private string _defaultValue = string.Empty;
		public string DefaultValue
		{
			get { return _defaultValue; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_defaultValue = string.Empty;
					return;
				}
				_defaultValue = value;
			}
		}

		public SymbolType Type { get; set; }
	}
}
