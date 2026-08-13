using System;
using System.Collections.Generic;
using System.Linq;

namespace ODUtils.Helpers;

public static class MathHelpers
{
	public static bool DoubleBetween(double min, double max, double value)
	{
		return value > min && value < max;
	}

	public static bool DoubleBetweenMinEquals(double min, double max, double value)
	{
		return value >= min && value < max;
	}

	public static Tuple<long, long> GetMinMaxSumOfValues(List<long> values, int sumCount)
	{
		long item;
		long item2;
		if (sumCount == 1)
		{
			item = values.Min();
			item2 = values.Max();
			return Tuple.Create(item, item2);
		}
		values.Sort();
		item = values.Take(sumCount).Sum();
		values.Reverse();
		item2 = values.Take(sumCount).Sum();
		return Tuple.Create(item, item2);
	}

	public static long GetMinSumOfValues(List<long> values, int sumCount)
	{
		if (values.Count == 0)
		{
			return 0L;
		}
		if (sumCount == 1)
		{
			return values.Min();
		}
		values.Sort();
		return values.Take(sumCount).Sum();
	}

	public static long GetMaxSumOfValues(List<long> values, int sumCount)
	{
		if (values.Count == 0)
		{
			return 0L;
		}
		if (sumCount == 1)
		{
			return values.Max();
		}
		return values.OrderByDescending((long x) => x).Take(sumCount).Sum();
	}
}
