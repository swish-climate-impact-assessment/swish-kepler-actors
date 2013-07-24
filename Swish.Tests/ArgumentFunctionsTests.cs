using System.Collections.Generic;
using System;

namespace Swish.Tests
{
	public class ArgumentFunctionsTests
	{
		public void ArgumentInQuotes()
		{
			// this tests that the arguments can be in quates,
			// use by the command adapter to specify the command and escape characters

			string argument1Name = ArgumentParser.DefaultArgumentPrefix + "blah";
			string escapedSlash = "\\/";
			string escapedQuote = "\\\"";

			string expectedArgument1Value = argument1Name + " / \"value";

			string[] arguments = new string[]{
 				argument1Name , // argument name
 				"\""+argument1Name+" "+ escapedSlash,  // value part 1 of 2
				 escapedQuote + "value" + "\"", // value part 2 of 2
				ArgumentParser.DefaultArgumentPrefix+"other",
				"otherValue"
			};

			// when I get the 'blah' argument value I expect a single string, with no surronding ", and escaped characters converted
			Arguments splitArguments = ArgumentParser.Read(arguments);
			string argument1Value = splitArguments.String(argument1Name, true);
			if (argument1Value != expectedArgument1Value)
			{
				throw new TestException();
			}

			// test that other arguments are not affected
			if (splitArguments.String(ArgumentParser.DefaultArgumentPrefix + "other", true) != "otherValue")
			{
				throw new TestException();
			}
		}

		public void GetSwitch()
		{
			/// this is test verifies that gets switch returns true if command argument present
			/// and false if it is absent

			string arguments = ArgumentParser.DefaultArgumentPrefix + "flag " + ArgumentParser.DefaultArgumentPrefix + "setting";
			Arguments splitArguments = ArgumentParser.Read(arguments);
			if (!splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "flag"))
			{
				throw new TestException();
			}

			if (splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "fake"))
			{
				throw new TestException();
			}

			if (!splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "setting"))
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
			string arguments = ArgumentParser.DefaultArgumentPrefix + name + " " + TestFlags.One.ToString() + " " + TestFlags.Two.ToString().ToLower() + " ";
			Arguments splitArguments = ArgumentParser.Read(arguments);
			List<TestFlags> flags = splitArguments.EnumList<TestFlags>(ArgumentParser.DefaultArgumentPrefix + name, true, true);

			if (flags.Count != 2 || flags[0] != TestFlags.One || flags[1] != TestFlags.Two)
			{
				throw new TestException();
			}
		}

		internal void NoStray0x34s()
		{
			/// verifies that there all the "'s are removed from the various encodings of arguments
			/// this is important so that any encoded arguments are fully decoded, clean and ready for use
			/// 

			List<Tuple<string, string>> expectedArguments = new List<Tuple<string, string>>(new Tuple<string, string>[] { 
				new Tuple<string, string>("operation","RenameVariable"),
				new Tuple<string, string>("input","C:\\Swish\\SampleData\\Kaleen.csv"),
				new Tuple<string, string>("newVariableName","minimumAverageTemperature maximumAverageTemperature"),
				new Tuple<string, string>("variable","minave maxave"),
			});

			string[] arguments = new string[]{ 
				">operation", "RenameVariable", 
				">input\"C:\\Swish\\SampleData\\Kaleen.csv\"", 
				">newVariableName\"minimumAverageTemperature", "maximumAverageTemperature", "\"", 
				">variable\"<034minave", "maxave<034\""
			};

			Arguments splitArguments = ArgumentParser.Read(arguments);
			if (splitArguments.SplitArguments.Count != 4)
			{
				throw new Exception();
			}

			for (int argumentIndex = 0; argumentIndex < splitArguments.SplitArguments.Count; argumentIndex++)
			{
				Tuple<string, string> argument = splitArguments.SplitArguments[argumentIndex];
				if (argument.Item1.Contains("\"") || argument.Item2.Contains("\""))
				{
					throw new Exception();
				}

				Tuple<string, string> expectedArgument = expectedArguments[argumentIndex];
				if (argument.Item1 != expectedArgument.Item1.ToLower() )
					
									{
					throw new Exception();
				}

				if (argument.Item2 != expectedArgument.Item2)
				{
					throw new Exception();
				}
			}

			// more encoded arguments
			arguments = new string[]{ 
				"<034><034<034operation<034<034RenameVariable<034", 
				"\">\"\"input\"\"C:\\Swish\\SampleData\\Kaleen.csv\"", 
				"\"<034><034\"\"<034newVariableName<034\"\"<034minimumAverageTemperature", "maximumAverageTemperature<034\"", 
				">variable minave maxave"
			};

			splitArguments = ArgumentParser.Read(arguments);
			if (splitArguments.SplitArguments.Count != 4)
			{
				throw new Exception();
			}

			for (int argumentIndex = 0; argumentIndex < splitArguments.SplitArguments.Count; argumentIndex++)
			{
				Tuple<string, string> argument = splitArguments.SplitArguments[argumentIndex];
				if (argument.Item1.Contains("\"") || argument.Item2.Contains("\""))
				{
					throw new Exception();
				}

				Tuple<string, string> expectedArgument = expectedArguments[argumentIndex];
				if (argument.Item1 != expectedArgument.Item1.ToLower() || argument.Item2 != expectedArgument.Item2)
				{
					throw new Exception();
				}
			}
		}
	}
}
