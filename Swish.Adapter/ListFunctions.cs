using System;
using System.Collections.Generic;

namespace Swish
{
	public static class ListFunctions
	{
		public delegate int CompareFunction<Record>(Record left, Record right);

		public static void QuickSort<Record>(Record[] list, CompareFunction<Record> Compare)
		{
			QuickSort(list, 0, list.Length, Compare);
		}

		public static void QuickSort<Record>(Record[] list, int startIndex, int endIndex, CompareFunction<Record> Compare)
		{
			int start = startIndex;
			int end = endIndex - 1;

			Record pivotValue = list[start];

			Record value = default(Record);
			while (true)
			{
				while (start < end)
				{
					value = list[end];
					int result = Compare(value, pivotValue);
					if (result < 0)
					{
						break;
					}
					//else if (result == 0)
					//{
					//	throw new Exception();
					//}
					end--;
				}

				if (start == end)
				{
					break;
				}
				list[start] = value;
				start++;

				while (start < end)
				{
					value = list[start];
					int result = Compare(value, pivotValue);
					if (result > 0)
					{
						break;
					}
					//else if (result == 0)
					//{
					//	throw new Exception();
					//}

					start++;
				}

				if (start == end)
				{
					break;
				}
				list[end] = list[start];
				end--;
			}
			list[start] = pivotValue;
			start++;

			if (end - startIndex > 1)
			{
				QuickSort(list, startIndex, end, Compare);
			}
			if (endIndex - start > 1)
			{
				QuickSort(list, start, endIndex, Compare);
			}
		}

		public static int IndexOf<Record>(Record record, IList<Record> list, CompareFunction<Record> Compare)
		{
			int start = 0;
			int end = list.Count - 1;
			int index;
			int result;

			if (list.Count == 0)
			{
				return -1;
			}

			result = Compare(record, list[start]);
			if (result < 0)
			{
				return -(start + 1);
			}
			if (result == 0)
			{
				return start;
			}

			result = Compare(record, list[end]);
			if (result > 0)
			{
				return -(end + 2);
			}
			if (result == 0)
			{
				return end;
			}

			while ((index = end - start) > 1)
			{
				index = index / 2 + start;

				result = Compare(record, list[index]);
				if (result < 0)
				{
					end = index;
				} else if (result > 0)
				{
					start = index;
				} else
				{
					return index;
				}
			}

			return -(start + 2);
		}

		public static bool TryAddSorted<Record>(Record record, List<Record> list, CompareFunction<Record> Compare)
		{
			int listIndex = IndexOf(record, list, Compare);
			if (listIndex >= 0)
			{
				return false;
			}
			listIndex = -(listIndex + 1);
			list.Insert(listIndex, record);
			return true;
		}

	}
}
