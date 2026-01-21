namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM.Exposers;

	/// <summary>
	/// Provides extension methods for creating filters on exposers and collection exposers.
	/// </summary>
	public static class FilterExtensions
	{
		/// <summary>
		/// Creates a filter that checks if the exposed field equals the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <typeparam name="TField">The type of the field being compared. Must be an Enum.</typeparam>
		/// <param name="exposer">The exposer that identifies the field to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, TField}"/> configured for equality comparison.</returns>
		public static ManagedFilter<TFilter, TField> Equal<TFilter, TField>(this Exposer<TFilter, TField> exposer, TField value)
			where TField : Enum
		{
			return exposer.UncheckedEqual(value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains an element equal to the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for equality comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> Equal<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.Equals, value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains an element equal to the specified value using the specified string comparison.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <param name="stringComparison">The string comparison method to use.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for equality comparison with the specified string comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> Equal<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value, StringComparison stringComparison)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.Equals, value)
			{
				StringComparison = stringComparison,
			};
		}

		/// <summary>
		/// Creates a filter that checks if the exposed field does not equal the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <typeparam name="TField">The type of the field being compared. Must be an Enum.</typeparam>
		/// <param name="exposer">The exposer that identifies the field to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, TField}"/> configured for inequality comparison.</returns>
		public static ManagedFilter<TFilter, TField> NotEqual<TFilter, TField>(this Exposer<TFilter, TField> exposer, TField value)
			where TField : Enum
		{
			return exposer.UncheckedNotEqual(value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains an element not equal to the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for inequality comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> NotEqual<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.NotEquals, value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains an element not equal to the specified value using the specified string comparison.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <param name="stringComparison">The string comparison method to use.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for inequality comparison with the specified string comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> NotEqual<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value, StringComparison stringComparison)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.NotEquals, value)
			{
				StringComparison = stringComparison,
			};
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to search for in the collection.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for contains comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> Contains<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.Contains, value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains the specified value using the specified string comparison.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to search for in the collection.</param>
		/// <param name="stringComparison">The string comparison method to use.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for contains comparison with the specified string comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> Contains<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value, StringComparison stringComparison)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.Contains, value)
			{
				StringComparison = stringComparison,
			};
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection does not contain the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to search for in the collection.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for not-contains comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> NotContains<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.NotContains, value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection does not contain the specified value using the specified string comparison.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to search for in the collection.</param>
		/// <param name="stringComparison">The string comparison method to use.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for not-contains comparison with the specified string comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> NotContains<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value, StringComparison stringComparison)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.NotContains, value)
			{
				StringComparison = stringComparison,
			};
		}

		/// <summary>
		/// Creates a filter that checks if the exposed field is less than the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <typeparam name="TField">The type of the field being compared. Must be an Enum.</typeparam>
		/// <param name="exposer">The exposer that identifies the field to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, TField}"/> configured for less-than comparison.</returns>
		public static ManagedFilter<TFilter, TField> LessThan<TFilter, TField>(this Exposer<TFilter, TField> exposer, TField value)
			where TField : Enum
		{
			return new ManagedFilter<TFilter, TField>(
				exposer,
				Comparer.GTE,
				value,
				(obj) => Convert.ToInt32(exposer.internalFunc(obj)).CompareTo(Convert.ToInt32(value)) < 0);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains an element less than the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for less-than comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> LessThan<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.LT, value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed field is less than or equal to the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <typeparam name="TField">The type of the field being compared. Must be an Enum.</typeparam>
		/// <param name="exposer">The exposer that identifies the field to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, TField}"/> configured for less-than-or-equal comparison.</returns>
		public static ManagedFilter<TFilter, TField> LessThanOrEqual<TFilter, TField>(this Exposer<TFilter, TField> exposer, TField value)
			where TField : Enum
		{
			return new ManagedFilter<TFilter, TField>(
				exposer,
				Comparer.GTE,
				value,
				(obj) => Convert.ToInt32(exposer.internalFunc(obj)).CompareTo(Convert.ToInt32(value)) <= 0);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains an element less than or equal to the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for less-than-or-equal comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> LessThanOrEqual<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.LTE, value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed field is greater than the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <typeparam name="TField">The type of the field being compared. Must be an Enum.</typeparam>
		/// <param name="exposer">The exposer that identifies the field to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, TField}"/> configured for greater-than comparison.</returns>
		public static ManagedFilter<TFilter, TField> GreaterThan<TFilter, TField>(this Exposer<TFilter, TField> exposer, TField value)
			where TField : Enum
		{
			return new ManagedFilter<TFilter, TField>(
				exposer,
				Comparer.GTE,
				value,
				(obj) => Convert.ToInt32(exposer.internalFunc(obj)).CompareTo(Convert.ToInt32(value)) > 0);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains an element greater than the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for greater-than comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> GreaterThan<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.GT, value);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed field is greater than or equal to the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <typeparam name="TField">The type of the field being compared. Must be an Enum.</typeparam>
		/// <param name="exposer">The exposer that identifies the field to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, TField}"/> configured for greater-than-or-equal comparison.</returns>
		public static ManagedFilter<TFilter, TField> GreaterThanOrEqual<TFilter, TField>(this Exposer<TFilter, TField> exposer, TField value)
			where TField : Enum
		{
			return new ManagedFilter<TFilter, TField>(
				exposer,
				Comparer.GTE,
				value,
				(obj) => Convert.ToInt32(exposer.internalFunc(obj)).CompareTo(Convert.ToInt32(value)) >= 0);
		}

		/// <summary>
		/// Creates a filter that checks if the exposed collection contains an element greater than or equal to the specified value.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter. Must be a reference type.</typeparam>
		/// <typeparam name="TField">The type of elements in the collection.</typeparam>
		/// <param name="exposer">The collection exposer that identifies the collection to filter on.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A <see cref="ManagedFilter{TFilter, IEnumerable}"/> configured for greater-than-or-equal comparison.</returns>
		public static ManagedFilter<TFilter, IEnumerable<TField>> GreaterThanOrEqual<TFilter, TField>(this CollectionExposer<TFilter, TField> exposer, TField value)
			where TFilter : class
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, Comparer.GTE, value);
		}
	}
}