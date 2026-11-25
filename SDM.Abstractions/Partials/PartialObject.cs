namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM.Exposers;

	/// <summary>
	/// Provides access to the explicitly selected fields of an object.
	/// </summary>
	/// <typeparam name="T">The type of the object.</typeparam>
	public class PartialObject<T>
		where T : class
	{
		private readonly T _instance;
		private readonly FieldExposer[] _fields;

		/// <summary>
		/// Initializes a new instance of the <see cref="PartialObject{T}"/> class with no selected fields.
		/// </summary>
		public PartialObject()
		{
			_fields = Array.Empty<FieldExposer>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PartialObject{T}"/> class.
		/// </summary>
		/// <param name="object">The underlying object instance that this partial object wraps.</param>
		/// <param name="fields">The collection of fields that were retrieved for the partial object.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="object"/> or <paramref name="fields"/> is <see langword="null"/>.</exception>
		public PartialObject(T @object, IReadOnlyExposerCollection<T> fields)
		{
			_instance = @object ?? throw new ArgumentNullException(nameof(@object));
			_fields = fields is null
				? throw new ArgumentNullException(nameof(fields))
				: fields.ToArray();
		}

		private PartialObject(T @object, FieldExposer[] fields)
		{
			_instance = @object ?? throw new ArgumentNullException(nameof(@object));
			_fields = fields ?? throw new ArgumentNullException(nameof(fields));
		}

		/// <summary>
		/// Creates a builder for a partial SDM object.
		/// </summary>
		/// <param name="object">The underlying object.</param>
		/// <returns>A builder for adding the fields that were retrieved.</returns>
		public static Builder CreateBuilder(T @object)
		{
			return new Builder(@object);
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

			return _fields.Any(field => field.Equals(exposer));
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

		private bool TryGetValueCore<TField>(Exposer<T, TField> exposer, out TField value)
		{
			if (!_fields.Any(field => field.Equals(exposer)))
			{
				value = default;
				return false;
			}

			value = exposer.internalFunc(_instance);
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
			private readonly T instance;
			private readonly List<FieldExposer> fields = new List<FieldExposer>();

			/// <summary>
			/// Initializes a new instance of the <see cref="Builder"/> class.
			/// </summary>
			/// <param name="object">The underlying object.</param>
			internal Builder(T @object)
			{
				instance = @object ?? throw new ArgumentNullException(nameof(@object));
			}

			/// <summary>
			/// Adds a field that was retrieved.
			/// </summary>
			/// <typeparam name="TField">The field type.</typeparam>
			/// <param name="exposer">The field exposer.</param>
			/// <returns>The current builder.</returns>
			/// <exception cref="ArgumentNullException">The exposer is null.</exception>
			public Builder Include<TField>(Exposer<T, TField> exposer)
			{
				if (exposer is null)
				{
					throw new ArgumentNullException(nameof(exposer));
				}

				fields.Add(exposer);
				return this;
			}

			/// <summary>
			/// Builds the partial SDM object.
			/// </summary>
			/// <returns>The constructed partial SDM object.</returns>
			public PartialObject<T> Build()
			{
				return new PartialObject<T>(instance, fields.ToArray());
			}
		}
	}
}
