using System;

namespace Swish.Stata
{
	public enum SymbolType
	{
		Unknown = 0,
		String,
		Input,
		VariableNameList,
		VariableName,
		Token,
		Bool,
		Double,
		Date,
		TemporaryFile,
		Output,

	}
}
