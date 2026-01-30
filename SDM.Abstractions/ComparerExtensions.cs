namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Provides extension methods for the <see cref="Comparer"/> enumeration.
	/// </summary>
	public static class ComparerExtensions
	{
		/// <summary>
		/// Inverts the specified comparer to its logical opposite.
		/// </summary>
		/// <param name="comparer">The comparer to invert.</param>
		/// <returns>The inverted comparer.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the specified comparer cannot be inverted.</exception>
		/// <remarks>
		/// The following inversions are supported:
		/// <list type="bullet">
		/// <item><description><see cref="Comparer.Equals"/> ↔ <see cref="Comparer.NotEquals"/></description></item>
		/// <item><description><see cref="Comparer.GTE"/> ↔ <see cref="Comparer.LT"/></description></item>
		/// <item><description><see cref="Comparer.GT"/> ↔ <see cref="Comparer.LTE"/></description></item>
		/// <item><description><see cref="Comparer.Contains"/> ↔ <see cref="Comparer.NotContains"/></description></item>
		/// <item><description><see cref="Comparer.Regex"/> ↔ <see cref="Comparer.NotRegex"/></description></item>
		/// </list>
		/// </remarks>
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
