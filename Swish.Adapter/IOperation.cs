using System;

namespace Swish
{
	public interface IOperation
	{
		string Name { get; }
		string Run(OperationArguments splitArguments);
	}
}
