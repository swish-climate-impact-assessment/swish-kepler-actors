using System.Collections.Generic;

namespace Swish.ScriptGenerators
{
	public interface IScriptGenerator
	{
		string Name { get; }

		List<string> GenerateScript();
	}
}
