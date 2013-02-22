using System;
using System.Collections.Generic;

namespace Swish.StataOperations
{
	public class SataScriptOutput
	{
		private List<string> _lines = new List<string>();
		public List<string> Lines
		{
			get { return _lines; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Lines");
				}
				if (value.Count == 0)
				{
					_lines = new List<string>(); return;
				}
				_lines = new List<string>(value);

			}
		}
	}
}