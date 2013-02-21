using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Swish.Adapters
{
	public interface IAdapter
	{
		string Name { get; }
		void Run(AdapterArguments splitArguments);
	}
}
