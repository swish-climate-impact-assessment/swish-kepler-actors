using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Swish.Tests
{
	public class ArgumentFunctionsTests
	{
		public void ArgumentInQuotes()
		{
			// this tests that the arguments can be in quates,
			// use by the command adapter to specify the command and escape characters

			string argument1Name = ArgumentFunctions.ArgumentCharacter + "blah";
			string escapedSlash = "\\/";
			string escapedQuote = "\\\"";

			string expectedArgument1Value = argument1Name + " / \"value";

			string[] arguments = new string[]{
 				argument1Name , // argument name
 				"\""+argument1Name+" "+ escapedSlash,  // value part 1 of 2
				 escapedQuote + "value" + "\"", // value part 2 of 2
				ArgumentFunctions.ArgumentCharacter+"other",
				"otherValue"
			};

			// when I get the 'blah' argument value I expect a single string, with no surronding ", and escaped characters converted
			List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
			string argument1Value = ArgumentFunctions.GetArgument(argument1Name, splitArguments, true);
			if (argument1Value != expectedArgument1Value)
			{
				throw new Exception();
			}

			// test that other arguments are not affected
			if (ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "other", splitArguments, true) != "otherValue")
			{
				throw new Exception();
			}
		}

		public void GetSwitch()
		{
			/// this is test verifies that gets switch returns true if command argument present
			/// and false if it is absent

			string arguments = ArgumentFunctions.ArgumentCharacter + "flag " + ArgumentFunctions.ArgumentCharacter + "setting";
			List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
			if (!ArgumentFunctions.GetSwitch(ArgumentFunctions.ArgumentCharacter + "flag", splitArguments))
			{
				throw new Exception("");
			}

			if (ArgumentFunctions.GetSwitch(ArgumentFunctions.ArgumentCharacter + "fake", splitArguments))
			{
				throw new Exception("");
			}

			if (!ArgumentFunctions.GetSwitch(ArgumentFunctions.ArgumentCharacter + "setting", splitArguments))
			{
				throw new Exception("");
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
			string arguments = ArgumentFunctions.ArgumentCharacter + name + " " + TestFlags.One.ToString() + " " + TestFlags.Two.ToString().ToLower() + " ";
			List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
			List<TestFlags> flags = ArgumentFunctions.GetArgumentFlags<TestFlags>(ArgumentFunctions.ArgumentCharacter + name, splitArguments, true, true);

			if (flags.Count != 2 || flags[0] != TestFlags.One || flags[1] != TestFlags.Two)
			{
				throw new Exception();
			}
		}
	}
}
