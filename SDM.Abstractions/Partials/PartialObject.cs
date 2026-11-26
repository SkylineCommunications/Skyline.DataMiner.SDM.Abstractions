#pragma warning disable SA1201 // Elements should appear in the correct order
namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM.Exposers;

	/// <summary>
	/// Provides access to the explicitly selected fields of an object.
	/// </summary>
	/// <typeparam name="T">The type of the object.</typeparam>
	public class PartialObject<T>
		where T : class
	{
		private readonly Dictionary<FieldExposer, IFieldValue> _values;

		/// <summary>
		/// Initializes a new instance of the <see cref="PartialObject{T}"/> class with no selected fields.
		/// </summary>
		public PartialObject()
		{
			_values = new Dictionary<FieldExposer, IFieldValue>();
		}

		private PartialObject(Dictionary<FieldExposer, IFieldValue> values)
		{
			_values = values ?? throw new ArgumentNullException(nameof(values));
		}

		/// <summary>
		/// Creates a builder for a partial SDM object.
		/// </summary>
		/// <returns>A builder for adding the fields that were retrieved.</returns>
		public static Builder CreateBuilder()
		{
			return new Builder();
		}

		/// <summary>
		/// Determines whether a field was retrieved.
		/// </summary>
		/// <param name="exposer">The field exposer.</param>
		/// <returns>True when the exposer was included in this partial object.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		public bool HasField(FieldExposer exposer)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			return _values.ContainsKey(exposer);
		}

		/// <summary>
		/// Determines whether a typed field was retrieved.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The field exposer.</param>
		/// <returns>True when the exposer was included in this partial object.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		public bool HasField<TField>(Exposer<T, TField> exposer)
		{
			return HasField((FieldExposer)exposer);
		}

		/// <summary>
		/// Retrieves a selected field from the underlying object.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The same exposer instance used to select the field.</param>
		/// <param name="value">The field value, or the default value when the field is not selected.</param>
		/// <returns>True when the field was selected, including when its value is null or the default value.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		/// <exception cref="InvalidOperationException">The exposer represents a collection.</exception>
		public bool TryGetValue<TField>(Exposer<T, TField> exposer, out TField value)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			if (IsCollectionExposer(exposer))
			{
				throw new InvalidOperationException("Collection exposers must be retrieved with TryGetValues.");
			}

			return TryGetValueCore(exposer, out value);
		}

		/// <summary>
		/// Retrieves a selected field, throwing when the field was not retrieved.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The same exposer used to select the field.</param>
		/// <returns>The field value.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		/// <exception cref="InvalidOperationException">The field was not retrieved or represents a collection.</exception>
		public TField GetValue<TField>(Exposer<T, TField> exposer)
		{
			if (!TryGetValue(exposer, out var value))
			{
				throw new InvalidOperationException("The requested field was not included in this partial object.");
			}

			return value;
		}

		/// <summary>
		/// Retrieves a selected collection field from the underlying object.
		/// </summary>
		/// <typeparam name="TField">The element type of the collection.</typeparam>
		/// <param name="exposer">The collection exposer used to select the field.</param>
		/// <param name="value">The collection value, or the default value when the field is not selected.</param>
		/// <returns>True when the field was selected, including when its value is null or empty.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		public bool TryGetValues<TField>(CollectionExposer<T, TField> exposer, out IEnumerable<TField> value)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			return TryGetValueCore(exposer, out value);
		}

		/// <summary>
		/// Retrieves a selected dynamic list field from the underlying object.
		/// </summary>
		/// <typeparam name="TField">The element type of the dynamic list.</typeparam>
		/// <param name="exposer">The dynamic list exposer used to select the field.</param>
		/// <param name="value">The list value, or the default value when the field is not selected.</param>
		/// <returns>True when the field was selected, including when its value is null or empty.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		public bool TryGetValues<TField>(DynamicListExposer<T, TField> exposer, out IEnumerable<TField> value)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			if (!_values.TryGetValue(exposer, out var fieldValue) ||
				!fieldValue.TryGetValue(out value))
			{
				value = default;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Retrieves a selected collection field, throwing when the field was not retrieved.
		/// </summary>
		/// <typeparam name="TField">The element type of the collection.</typeparam>
		/// <param name="exposer">The collection exposer used to select the field.</param>
		/// <returns>The collection value.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		/// <exception cref="InvalidOperationException">The field was not retrieved.</exception>
		public IEnumerable<TField> GetValues<TField>(CollectionExposer<T, TField> exposer)
		{
			if (!TryGetValues(exposer, out var value))
			{
				throw new InvalidOperationException("The requested field was not included in this partial object.");
			}

			return value;
		}

		/// <summary>
		/// Retrieves a selected dynamic list field, throwing when the field was not retrieved.
		/// </summary>
		/// <typeparam name="TField">The element type of the dynamic list.</typeparam>
		/// <param name="exposer">The dynamic list exposer used to select the field.</param>
		/// <returns>The list value.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		/// <exception cref="InvalidOperationException">The field was not retrieved.</exception>
		public IEnumerable<TField> GetValues<TField>(DynamicListExposer<T, TField> exposer)
		{
			if (!TryGetValues(exposer, out var value))
			{
				throw new InvalidOperationException("The requested field was not included in this partial object.");
			}

			return value;
		}

		private bool TryGetValueCore<TField>(Exposer<T, TField> exposer, out TField value)
		{
			if (!_values.TryGetValue(exposer, out var fieldValue) ||
				!fieldValue.TryGetValue(out value))
			{
				value = default;
				return false;
			}

			return true;
		}

		private static bool IsCollectionExposer(FieldExposer exposer)
		{
			for (var type = exposer.GetType(); type != null; type = type.BaseType)
			{
				if (type.IsGenericType &&
					type.GetGenericTypeDefinition() == typeof(CollectionExposer<,>))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Builds a partial SDM object by adding fields that were retrieved.
		/// </summary>
		public sealed class Builder
		{
			private readonly Dictionary<FieldExposer, IFieldValue> fields = new Dictionary<FieldExposer, IFieldValue>();

			/// <summary>
			/// Initializes a new instance of the <see cref="Builder"/> class.
			/// </summary>
			internal Builder()
			{
			}

			/// <summary>
			/// Adds a field that was retrieved.
			/// </summary>
			/// <typeparam name="TField">The field type.</typeparam>
			/// <param name="exposer">The field exposer.</param>
			/// <param name="value">The field's value.</param>
			/// <returns>The current builder.</returns>
			/// <exception cref="ArgumentNullException">The exposer is null.</exception>
			public Builder Include<TField>(Exposer<T, TField> exposer, TField value)
			{
				if (exposer is null)
				{
					throw new ArgumentNullException(nameof(exposer));
				}

				fields[exposer] = new FieldValue<TField>(value);
				return this;
			}

			/// <summary>
			/// Adds a dynamic list field that was retrieved.
			/// </summary>
			/// <typeparam name="TField">The element type of the dynamic list.</typeparam>
			/// <param name="exposer">The dynamic list exposer.</param>
			/// <param name="value">The list's value.</param>
			/// <returns>The current builder.</returns>
			/// <exception cref="ArgumentNullException">The exposer is null.</exception>
			public Builder Include<TField>(DynamicListExposer<T, TField> exposer, IEnumerable<TField> value)
			{
				if (exposer is null)
				{
					throw new ArgumentNullException(nameof(exposer));
				}

				fields[exposer] = new FieldValue<IEnumerable<TField>>(value);
				return this;
			}

			/// <summary>
			/// Builds the partial SDM object.
			/// </summary>
			/// <returns>The constructed partial SDM object.</returns>
			public PartialObject<T> Build()
			{
				return new PartialObject<T>(new Dictionary<FieldExposer, IFieldValue>(fields));
			}
		}

		private interface IFieldValue
		{
			bool TryGetValue<TField>(out TField value);
		}

		private sealed class FieldValue<TField> : IFieldValue
		{
			public FieldValue(TField value)
			{
				Value = value;
			}

			public TField Value { get; }

			public bool TryGetValue<TRequestedField>(out TRequestedField value)
			{
#pragma warning disable S3060 // Intentional generic type discrimination for typed value retrieval
				// Intentional type discrimination:
				// values with different generic types are stored behind the common IFieldValue interface.
				// FieldValue<TField> must verify that the requested generic type matches the stored closed generic type before returning it.
				// Replacing this with polymorphism would require object casts, dynamic dispatch, or a more complex visitor pattern,
				// so this check is required for type-safe retrieval.
				if (this is FieldValue<TRequestedField> typedValue)
				{
					value = typedValue.Value;
					return true;
				}
#pragma warning restore S3060

				value = default;
				return false;
			}
		}
	}
}

#pragma warning restore SA1201 // Elements should appear in the correct order