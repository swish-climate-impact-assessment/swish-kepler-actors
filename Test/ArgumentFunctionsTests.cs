using System.Collections.Generic;

namespace Swish.Tests
{
	public class ArgumentFunctionsTests
	{
		public void ArgumentInQuotes()
		{
			// this tests that the arguments can be in quates,
			// use by the command adapter to specify the command and escape characters

			string argument1Name = Arguments.DefaultArgumentPrefix + "blah";
			string escapedSlash = "\\/";
			string escapedQuote = "\\\"";

			string expectedArgument1Value = argument1Name + " / \"value";

			string[] arguments = new string[]{
 				argument1Name , // argument name
 				"\""+argument1Name+" "+ escapedSlash,  // value part 1 of 2
				 escapedQuote + "value" + "\"", // value part 2 of 2
				Arguments.DefaultArgumentPrefix+"other",
				"otherValue"
			};

			// when I get the 'blah' argument value I expect a single string, with no surronding ", and escaped characters converted
			Arguments splitArguments = new Arguments(arguments);
			string argument1Value = splitArguments.String(argument1Name, true);
			if (argument1Value != expectedArgument1Value)
			{
				throw new TestException();
			}

			// test that other arguments are not affected
			if (splitArguments.String(Arguments.DefaultArgumentPrefix + "other", true) != "otherValue")
			{
				throw new TestException();
			}
		}

		public void GetSwitch()
		{
			/// this is test verifies that gets switch returns true if command argument present
			/// and false if it is absent

			string arguments = Arguments.DefaultArgumentPrefix + "flag " + Arguments.DefaultArgumentPrefix + "setting";
			Arguments splitArguments = new Arguments(arguments);
			if (!splitArguments.Exists(Arguments.DefaultArgumentPrefix + "flag"))
			{
				throw new TestException();
			}

			if (splitArguments.Exists(Arguments.DefaultArgumentPrefix + "fake"))
			{
				throw new TestException();
			}

			if (!splitArguments.Exists(Arguments.DefaultArgumentPrefix + "setting"))
			{
				throw new TestException();
			}

		}

		enum TestFlags
		{
			Unknown = 0,
			One,
			Two,
			Three,
		}
		internal void GetFlags()
		{
			string name = "flag";
			string arguments = Arguments.DefaultArgumentPrefix + name + " " + TestFlags.One.ToString() + " " + TestFlags.Two.ToString().ToLower() + " ";
			Arguments splitArguments = new Arguments(arguments);
			List<TestFlags> flags = splitArguments.EnumList<TestFlags>(Arguments.DefaultArgumentPrefix + name, true, true);

			if (flags.Count != 2 || flags[0] != TestFlags.One || flags[1] != TestFlags.Two)
			{
				throw new TestException();
			}
		}
	}
}
