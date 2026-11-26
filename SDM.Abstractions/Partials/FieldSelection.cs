namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Represents a mutable collection of fields selected for a partial query.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	public class FieldSelection<T> : IFieldSelection<T>
		where T : class
	{
		private readonly List<IFieldProjection<T>> projections = new List<IFieldProjection<T>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldSelection{T}"/> class.
		/// </summary>
		public FieldSelection()
		{
		}

		internal FieldSelection(IReadOnlyFieldSelection<T> selection)
		{
			if (selection is null)
			{
				throw new ArgumentNullException(nameof(selection));
			}

			projections.AddRange(selection);
		}

		/// <inheritdoc />
		public int Count => projections.Count;

		/// <summary>
		/// Adds a field to the selection when it is not already contained.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The exposer to add.</param>
		/// <returns><see langword="true"/> when the exposer was added; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">The exposer is null.</exception>
		public bool Add<TField>(Exposer<T, TField> exposer)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var fieldExposer = (FieldExposer)exposer;
			foreach (var projection in projections)
			{
				if (projection.Exposer.Equals(fieldExposer))
				{
					return false;
				}
			}

			projections.Add(new FieldProjection<T, TField>(exposer));
			return true;
		}

		/// <inheritdoc />
		public IEnumerator<IFieldProjection<T>> GetEnumerator()
		{
			return projections.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
