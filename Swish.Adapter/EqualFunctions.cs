using System;
using System.Collections.Generic;
using System.IO;

namespace Swish
{
	public static class EqualFunctions
	{
		public delegate bool EqualFunction<t>(t left, t right);

		public static bool FilesEqual(string leftFileName, string rightFileName)
		{
			using (FileStream left = new FileStream(leftFileName, FileMode.Open, FileAccess.Read))
			using (FileStream right = new FileStream(rightFileName, FileMode.Open, FileAccess.Read))
			{

				if (left.Length != right.Length)
				{
					return false;
				}

				byte[] leftBuffer = new byte[8192];
				byte[] rightBuffer = new byte[8192];
				while (true)
				{

					int leftCount = left.Read(leftBuffer, 0, leftBuffer.Length);
					int rightCount = right.Read(rightBuffer, 0, rightBuffer.Length);

					if (leftCount != rightCount || !Equal(leftBuffer, rightBuffer))
					{
						return false;
					}

					if (leftCount != leftBuffer.Length)
					{
						break;
					}
				}
			}
			return true;
		}

		public static bool Equal(byte[] left, byte[] right)
		{
			if (left == null)
			{
				if (right != null)
				{
					return false;
				}
				return true;
			} else
			{
				if (right == null)
				{
					return false;
				}
			}

			if (left.Length != right.Length)
			{
				return false;
			}

			for (int byteIndex = 0; byteIndex < left.Length; byteIndex++)
			{
				if (left[byteIndex] != right[byteIndex])
				{
					return false;
				}
			}

			return true;
		}

		public static bool Equal(Type left, Type right)
		{
#if !MONO
			return left == right;
#else
			if ((left as object) == null)
			{
				if ((right as object) == null)
				{
					return true;
				}
				return false;
			}
			if ((right as object) == null)
			{
				return false;
			}

			return left.Name == right.Name;
#endif
		}

		public static bool Equal(byte[] left, byte[] right, int count)
		{
			if (left == null)
			{
				if (right == null)
				{
					return true;
				}
				return false;
			}
			if (right == null)
			{
				return false;
			}

			if (left.Length < count || right.Length < count)
			{
				throw new Exception();
			}

			for (int index = 0; index < count; index++)
			{
				if (left[index] != right[index])
				{
					return false;
				}
			}

			return true;
		}


		public static bool Equal<t>(List<t> left, List<t> right, EqualFunction<t> equalFunction)
		{
			if (left == null)
			{
				if (right != null)
				{
					return false;
				}
				return true;
			} else
			{
				if (right == null)
				{
					return false;
				}
			}

			if (left.Count != right.Count)
			{
				return false;
			}

			for (int itemIndex = 0; itemIndex < left.Count; itemIndex++)
			{
				if (!equalFunction(left[itemIndex], right[itemIndex]))
				{
					return false;
				}
			}

			return true;
		}

		public static bool Equal(List<string> left, List<string> right)
		{
			return Equal(left, right, Equal);
		}

		public static bool Equal(string left, string right)
		{
			//if (left != right)
			//{
			//    throw new Exception();
			//}
			return left == right;
		}

		public static bool Equal(Table left, Table right)
		{
			if (left == null)
			{
				if (right == null)
				{
					return true;
				}
				return false;
			}
			if (right == null)
			{
				return false;
			}
			if (left.Headers.Count != right.Headers.Count || left.Records.Count != right.Records.Count)
			{
				return false;
			}

			for (int headerIndex = 0; headerIndex < left.Headers.Count; headerIndex++)
			{
				string leftName = left.Headers[headerIndex];
				string rightName = right.Headers[headerIndex];
				if (leftName != rightName)
				{
					return false;
				}
			}

			for (int recordIndex = 0; recordIndex < left.Records.Count; recordIndex++)
			{
				List<string> leftRecord = left.Records[recordIndex];
				List<string> rightRecord = right.Records[recordIndex];
				for (int valueIndex = 0; valueIndex < leftRecord.Count; valueIndex++)
				{

					string leftItem = left.Headers[valueIndex];
					string rightItem = right.Headers[valueIndex];
					if (leftItem != rightItem)
					{
						return false;
					}
				}
			}

			return true;
		}


	}
}
