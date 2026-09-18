namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Provides extension methods for creating filters on exposers and collection exposers
	/// for fields that expose <see cref="Type"/> values.
	/// </summary>
	public static class TypeFilterExtensions
	{
		/// <summary>
		/// Creates a filter that checks whether the exposed <see cref="Type"/> value equals the specified type.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <typeparam name="TField">
		/// The exposed field type being compared. Must derive from <see cref="Type"/>.
		/// </typeparam>
		/// <param name="exposer">The exposer that identifies the type field to filter on.</param>
		/// <param name="value">The type value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, TField}"/> configured for equality comparison.</returns>
		public static ManagedFilter<TFilter, TField> Equal<TFilter, TField>(this Exposer<TFilter, TField> exposer, TField value)
			where TField : Type
			where TFilter : class
		{
			return exposer.UncheckedEqual(value);
		}

		/// <summary>
		/// Creates a filter that checks whether the exposed <see cref="Type"/> value does not equal the specified type.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <typeparam name="TField">
		/// The exposed field type being compared. Must derive from <see cref="Type"/>.
		/// </typeparam>
		/// <param name="exposer">The exposer that identifies the type field to filter on.</param>
		/// <param name="value">The type value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, TField}"/> configured for inequality comparison.</returns>
		public static ManagedFilter<TFilter, TField> NotEqual<TFilter, TField>(this Exposer<TFilter, TField> exposer, TField value)
			where TField : Type
			where TFilter : class
		{
			return exposer.UncheckedNotEqual(value);
		}
	}
}
