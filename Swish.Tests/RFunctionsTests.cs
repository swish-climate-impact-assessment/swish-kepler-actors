using System;

namespace Swish.Tests
{
	class RFunctionsTests
	{
		internal void RVersion()
		{
			if (!RFunctions.Installed)
			{
				throw new Exception();
			}
		}
	}
}
