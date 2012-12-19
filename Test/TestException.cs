using System;

namespace Swish.Tests
{
	public class TestException: Exception
	{
		public TestException()
		{

		}
		public TestException(string message)
			: base(message)
		{
		}
	}
}