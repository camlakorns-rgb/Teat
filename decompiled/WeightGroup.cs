using System;
using System.Collections.Generic;
using System.Linq;

public class WeightGroup<T> : Dictionary<T, double> where T : class
{
	public T GetItem(int seed)
	{
		double num = 0.0;
		foreach (double value in base.Values)
		{
			double num2 = Math.Abs(value);
			num += num2;
		}
		if (num <= 0.0)
		{
			return null;
		}
		double num3 = new Random(seed).NextDouble();
		double num4 = 0.0;
		foreach (KeyValuePair<T, double> item in from v in this.ToList()
			orderby v.Value descending
			select v)
		{
			num4 += item.Value / num;
			if (num3 <= num4)
			{
				return item.Key;
			}
		}
		return base.Keys.FirstOrDefault();
	}
}
