namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	public static class ComparerExtensions
	{
		public static Comparer Invert(this Comparer comparer)
		{
			if (comparer == Comparer.Equals)
			{
				return Comparer.NotEquals;
			}

			if (comparer == Comparer.NotEquals)
			{
				return Comparer.Equals;
			}

			if (comparer == Comparer.GTE)
			{
				return Comparer.LT;
			}

			if (comparer == Comparer.GT)
			{
				return Comparer.LTE;
			}

			if (comparer == Comparer.LTE)
			{
				return Comparer.GT;
			}

			if (comparer == Comparer.LT)
			{
				return Comparer.GTE;
			}

			if (comparer == Comparer.Contains)
			{
				return Comparer.NotContains;
			}

			if (comparer == Comparer.NotContains)
			{
				return Comparer.Contains;
			}

			if (comparer == Comparer.Regex)
			{
				return Comparer.NotRegex;
			}

			if (comparer == Comparer.NotRegex)
			{
				return Comparer.Regex;
			}

			throw new InvalidOperationException($"Could not invert '{comparer}'");
		}
	}
}
