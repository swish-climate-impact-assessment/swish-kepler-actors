using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class PostGreSqlPasswordFileEditAdapter: IOperation
	{
		public const string OperationName = "PostGreSqlPasswordFile";
		public string Name { get { return OperationName; } }

		public string Run(OperationArguments splitArguments)
		{
			Swish.PostGreSqlPasswordFileEditor.Program.Run();
			return string.Empty;
		}
	}
}
