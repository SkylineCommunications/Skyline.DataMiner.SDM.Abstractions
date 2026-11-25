namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Provides read-only access to a collection of unique exposers for an object.
	/// </summary>
	/// <typeparam name="T">The type of the object.</typeparam>
	public interface IReadOnlyExposerCollection<T> : IReadOnlyCollection<FieldExposer>
		where T : class
	{
		/// <summary>
		/// Determines whether a typed exposer is contained.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The exposer to find.</param>
		/// <returns>True when the exposer is contained.</returns>
		bool Contains<TField>(Exposer<T, TField> exposer);
	}

	/// <summary>
	/// Provides mutation access to a collection of unique exposers for an object.
	/// </summary>
	/// <typeparam name="T">The type of the object.</typeparam>
	public interface IExposerCollection<T> : IReadOnlyExposerCollection<T>
		where T : class
	{
		/// <summary>
		/// Adds a typed exposer when it is not already contained.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The exposer to add.</param>
		/// <returns>True when the exposer was added; otherwise false.</returns>
		bool Add<TField>(Exposer<T, TField> exposer);
	}

	/// <summary>
	/// Represents an ordered collection of unique exposers for an object.
	/// </summary>
	/// <typeparam name="T">The type of the object.</typeparam>
	public sealed class ExposerCollection<T> : IExposerCollection<T>
		where T : class
	{
		private readonly List<FieldExposer> exposers = new List<FieldExposer>();

		public ExposerCollection()
		{
		}

		internal ExposerCollection(IEnumerable<FieldExposer> fields)
		{
			if (fields is null)
			{
				throw new ArgumentNullException(nameof(fields));
			}

			foreach (var field in fields)
			{
				if (field is null)
				{
					throw new ArgumentNullException(nameof(fields));
				}

				if (field.FilterType != typeof(T))
				{
					throw new ArgumentException($"The exposer must belong to {typeof(T)}.", nameof(fields));
				}

				if (!Contains(field))
				{
					exposers.Add(field);
				}
			}
		}

		/// <inheritdoc />
		public int Count => exposers.Count;

		/// <summary>
		/// Adds a typed exposer when it is not already contained.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The exposer to add.</param>
		/// <returns>True when the exposer was added; otherwise false.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		public bool Add<TField>(Exposer<T, TField> exposer)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var fieldExposer = (FieldExposer)exposer;
			if (Contains(fieldExposer))
			{
				return false;
			}

			exposers.Add(fieldExposer);
			return true;
		}

		/// <summary>
		/// Determines whether a typed exposer is contained.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The exposer to find.</param>
		/// <returns>True when the exposer is contained.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		public bool Contains<TField>(Exposer<T, TField> exposer)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			return Contains((FieldExposer)exposer);
		}

		/// <summary>
		/// Determines whether an exposer is contained.
		/// </summary>
		/// <param name="exposer">The exposer to find.</param>
		/// <returns>True when the exposer is contained.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		public bool Contains(FieldExposer exposer)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			for (var i = 0; i < exposers.Count; i++)
			{
				if (exposers[i].Equals(exposer))
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc />
		public IEnumerator<FieldExposer> GetEnumerator()
		{
			return exposers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
