using System;
using System.Collections.Generic;
using System.Text;

namespace Swish
{
	public static class ExceptionFunctions
	{
		public const string Separator = "----------------------------------------------------------------------------------------------------";

		public static void Write(StringBuilder output, Exception exception, bool messageOnly)
		{
			if (ForceVerbose)
			{
				messageOnly = false;
			}
			List<Exception> exceptions = new List<Exception>();
			do
			{
				if (exception == null)
				{
					break;
				}
				exceptions.Add(exception);
				exception = exception.InnerException;
			}
			while (exception != null);
			for (int i = exceptions.Count - 1; i >= 0; i--)
			{
				exception = exceptions[i];
				output.AppendLine(exception.GetType().FullName);
				if (!string.IsNullOrEmpty(exception.Message))
				{
					output.AppendLine(exception.Message);
				}
				if (!messageOnly && !string.IsNullOrEmpty(exception.StackTrace))
				{
					output.AppendLine(exception.StackTrace);
				}
				if (!string.IsNullOrEmpty(exception.HelpLink))
				{
					output.AppendLine(exception.HelpLink);
				}
				if (!messageOnly && !string.IsNullOrEmpty(exception.Source))
				{
					output.AppendLine(exception.Source);
				}
				if (i > 0)
				{
					output.AppendLine("----------------------------------------------------------------------------------------------------");
				}
			}
		}

		private static bool _forceVerbose = false;
		public static bool ForceVerbose
		{
			get
			{
				return false
					|| _forceVerbose
					|| Environment.MachineName.GetHashCode() == -743622408
					;
			}
			set { _forceVerbose = value; }
		}

		public static string Write(Exception error, bool messageOnly)
		{
			if (ForceVerbose)
			{
				messageOnly = false;
			}
			string empty = string.Empty;
			while (error != null)
			{
				if (!string.IsNullOrEmpty(error.Message))
				{
					empty = string.Concat(error.Message, Environment.NewLine, empty);
				}
				if (!string.IsNullOrEmpty(error.HelpLink))
				{
					empty = string.Concat(error.HelpLink, Environment.NewLine, empty);
				}
				if (!messageOnly)
				{
					if (!string.IsNullOrEmpty(error.Source))
					{
						empty = string.Concat(error.Source, Environment.NewLine, empty);
					}
					if (!string.IsNullOrEmpty(error.StackTrace))
					{
						empty = string.Concat(error.StackTrace, Environment.NewLine, empty);
					}
					empty = string.Concat(error.GetType().FullName, Environment.NewLine, empty);
					if (error.InnerException != null)
					{
						empty = string.Concat("- - - - - - - - - - - - - - - - - - - - - - - -", Environment.NewLine, empty);
					}
					error = error.InnerException;
				} else
				{
					break;
				}
			}
			return empty;
		}
	}
}
