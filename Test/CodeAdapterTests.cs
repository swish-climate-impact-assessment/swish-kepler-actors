using Swish.Adapters;

namespace Swish.Tests
{
	class CodeAdapterTests
	{
		internal void MessageBox()
		{
			AdapterArguments arguments = new AdapterArguments(new Arguments(">Operation Code >Function System.Windows.Forms.DialogResult System.Windows.Forms.MessageBox.Show(System.String text) >Text value"));
			CodeAdapter adapter = new CodeAdapter();
			string outFileName = adapter.Run(arguments);
		}
	}
}
