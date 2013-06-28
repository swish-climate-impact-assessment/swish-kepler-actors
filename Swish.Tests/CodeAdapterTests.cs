using Swish.Adapters;

namespace Swish.Tests
{
	class CodeAdapterTests
	{
		internal void MessageBox()
		{
			OperationArguments arguments = new OperationArguments(new Arguments(">Operation Code >Function System.Windows.Forms.DialogResult System.Windows.Forms.MessageBox.Show(System.String text) >Text value"));
			CodeAdapter adapter = new CodeAdapter();
			string outFileName = adapter.Run(arguments);
		}
	}
}
