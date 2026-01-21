namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM.Types;
	using Skyline.DataMiner.SDM.Types.Shapes;

	/// <summary>
	/// Factory class for creating filter elements based on exposers and comparers.
	/// </summary>
	public static class FilterElementFactory
	{
		public static FilterElement<T> Create<T>(FieldExposer exposer, Comparer comparer, object value)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var shape = FieldTypeShape.Analyze(exposer.FieldType);
			var handler = ShapeLocator.Locate(exposer, shape);
			if (handler is null)
			{
				throw new NotSupportedException($"Field type of the exposer is not supported. Unsupported type: {shape.ElementType}");
			}

			if (!handler.SupportsComparer(shape, comparer))
			{
				throw new NotSupportedException($"Comparer '{comparer}' is not valid for field '{exposer.fieldName}' ({shape.ElementType}).");
			}

			var convertedValue = handler.Convert(value, shape);
			var filter = handler.BuildFilter<T>(exposer, comparer, convertedValue, shape);

			return filter;
		}

		/////// <summary>
		/////// Creates a <see cref="FilterElement{T}"/> for a list exposer using the specified comparer and value.
		/////// </summary>
		/////// <typeparam name="T">The type of the object to filter.</typeparam>
		/////// <typeparam name="TValue">The type of the value to compare.</typeparam>
		/////// <param name="exposer">The exposer used to extract the list of values from the object.</param>
		/////// <param name="comparer">The comparer to use for filtering.</param>
		/////// <param name="value">The value to compare against.</param>
		/////// <returns>A filter element for the specified parameters.</returns>
		////public static FilterElement<T> Create<T, TValue>(Exposer<T, List<TValue>> exposer, Comparer comparer, TValue value)
		////{
		////	return new ListManagedFilter<T, TValue>(exposer, comparer, value);
		////}

		/////// <summary>
		/////// Creates a <see cref="FilterElement{T}"/> using the specified exposer, comparer, and value.
		/////// </summary>
		/////// <typeparam name="T">The type of the object to filter.</typeparam>
		/////// <typeparam name="TValue">The type of the value to compare.</typeparam>
		/////// <param name="exposer">The exposer used to extract the value from the object.</param>
		/////// <param name="comparer">The comparer to use for filtering.</param>
		/////// <param name="value">The value to compare against.</param>
		/////// <returns>A filter element for the specified parameters.</returns>
		/////// <exception cref="ArgumentNullException">Thrown when <paramref name="exposer"/> is <c>null</c>.</exception>
		/////// <exception cref="NotSupportedException">Thrown when the specified comparer is not supported.</exception>
		////public static FilterElement<T> Create<T, TValue>(Exposer<T, TValue> exposer, Comparer comparer, TValue value)
		////{
		////	if (exposer is null)
		////	{
		////		throw new ArgumentNullException(nameof(exposer));
		////	}

		////	if (exposer is Exposer<T, string> stringExposer)
		////	{
		////		switch (comparer)
		////		{
		////			case Comparer.Contains:
		////				return stringExposer.Contains(value?.ToString());
		////			case Comparer.NotContains:
		////				return stringExposer.NotContains(value?.ToString());
		////			case Comparer.Regex:
		////				return stringExposer.Matches(value?.ToString());
		////			case Comparer.NotRegex:
		////				return stringExposer.NotMatches(value?.ToString());
		////		}
		////	}

		////	switch (comparer)
		////	{
		////		case Comparer.Equals:
		////		{
		////			return new ManagedFilter<T, TValue>(
		////				exposer,
		////				comparer,
		////				value,
		////				(T a) => UniversalComparer.Equals(exposer.internalFunc(a), value));
		////		}

		////		case Comparer.NotEquals:
		////		{
		////			return new ManagedFilter<T, TValue>(
		////				exposer,
		////				comparer,
		////				value,
		////				(T a) => !UniversalComparer.Equals(exposer.internalFunc(a), value));
		////		}

		////		case Comparer.GT:
		////		{
		////			return new ManagedFilter<T, TValue>(
		////				exposer,
		////				comparer,
		////				value,
		////				(T a) => UniversalComparer.Compare(exposer.internalFunc(a), value) > 0);
		////		}

		////		case Comparer.GTE:
		////		{
		////			return new ManagedFilter<T, TValue>(
		////				exposer,
		////				comparer,
		////				value,
		////				(T a) => UniversalComparer.Compare(exposer.internalFunc(a), value) > 0);
		////		}

		////		case Comparer.LT:
		////		{
		////			return new ManagedFilter<T, TValue>(
		////				exposer,
		////				comparer,
		////				value,
		////				(T a) => UniversalComparer.Compare(exposer.internalFunc(a), value) < 0);
		////		}

		////		case Comparer.LTE:
		////		{
		////			return new ManagedFilter<T, TValue>(
		////				exposer,
		////				comparer,
		////				value,
		////				(T a) => UniversalComparer.Compare(exposer.internalFunc(a), value) <= 0);
		////		}

		////		default:
		////			throw new NotSupportedException("This comparer option is not supported");
		////	}
		////}

		/////// <summary>
		/////// Creates a <see cref="FilterElement{T}"/> for a dynamic list exposer using the specified comparer and value.
		/////// </summary>
		/////// <typeparam name="T">The type of the object to filter.</typeparam>
		/////// <typeparam name="TValue">The type of the value to compare.</typeparam>
		/////// <param name="exposer">The dynamic list exposer used to extract the value from the object.</param>
		/////// <param name="comparer">The comparer to use for filtering.</param>
		/////// <param name="value">The value to compare against.</param>
		/////// <returns>A filter element for the specified parameters.</returns>
		/////// <exception cref="ArgumentNullException">Thrown when <paramref name="exposer"/> is <c>null</c>.</exception>
		/////// <exception cref="NotSupportedException">Thrown when the specified comparer is not supported.</exception>
		////public static FilterElement<T> Create<T, TValue>(DynamicListExposer<T, TValue> exposer, Comparer comparer, TValue value)
		////{
		////	if (exposer is null)
		////	{
		////		throw new ArgumentNullException(nameof(exposer));
		////	}

		////	switch (comparer)
		////	{
		////		case Comparer.Equals:
		////			return exposer.Equal(value);
		////		case Comparer.NotEquals:
		////			return exposer.NotEqual(value);
		////		case Comparer.GT:
		////			return exposer.GreaterThan(value);
		////		case Comparer.GTE:
		////			return exposer.GreaterThanOrEqual(value);
		////		case Comparer.LT:
		////			return exposer.LessThan(value);
		////		case Comparer.LTE:
		////			return exposer.LessThanOrEqual(value);
		////		case Comparer.Contains:
		////			return exposer.Contains(value);
		////		case Comparer.NotContains:
		////			return exposer.NotContains(value);
		////		default:
		////			throw new NotSupportedException("This comparer option is not supported");
		////	}
		////}
	}

	/// <summary>
	/// Provides universal comparison and equality operations for generic types.
	/// </summary>
	internal static class UniversalComparer
	{
		/// <summary>
		/// Compares two values of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of the values to compare.</typeparam>
		/// <param name="x">The first value.</param>
		/// <param name="y">The second value.</param>
		/// <returns>
		/// Less than zero if <paramref name="x"/> is less than <paramref name="y"/>.
		/// Zero if <paramref name="x"/> equals <paramref name="y"/>.
		/// Greater than zero if <paramref name="x"/> is greater than <paramref name="y"/>.
		/// </returns>
		/// <exception cref="ArgumentException">Thrown when the type does not implement <see cref="IComparable"/> or <see cref="IComparable{T}"/>.</exception>
		internal static int Compare<T>(T x, T y)
		{
			if (object.Equals(x, y))
			{
				return 0;
			}

			if (x == null)
			{
				return -1;
			}

			if (y == null)
			{
				return 1;
			}

			Type type = typeof(T);
			Type underlying = Nullable.GetUnderlyingType(type);

			if (underlying != null)
			{
				// Nullable value type like int?
				object xValue = Convert.ChangeType(x, underlying);
				object yValue = Convert.ChangeType(y, underlying);

				return ((IComparable)xValue).CompareTo(yValue);
			}

			var comparableT = x as IComparable<T>;
			if (comparableT != null)
			{
				return comparableT.CompareTo(y);
			}

			var comparable = x as IComparable;
			if (comparable != null)
			{
				return comparable.CompareTo(y);
			}

			throw new ArgumentException("Type " + type.FullName + " does not implement IComparable or IComparable<T>");
		}

		/// <summary>
		/// Determines whether two values of type <typeparamref name="T"/> are equal.
		/// </summary>
		/// <typeparam name="T">The type of the values to compare.</typeparam>
		/// <param name="x">The first value.</param>
		/// <param name="y">The second value.</param>
		/// <returns><c>true</c> if the values are equal; otherwise, <c>false</c>.</returns>
		internal static bool Equals<T>(T x, T y)
		{
			if (object.Equals(x, y))
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			var equatableT = x as IEquatable<T>;
			if (equatableT != null)
			{
				return equatableT.Equals(y);
			}

			// Fall back to Object.Equals if no better option is available
			return x.Equals(y);
		}
	}
}
