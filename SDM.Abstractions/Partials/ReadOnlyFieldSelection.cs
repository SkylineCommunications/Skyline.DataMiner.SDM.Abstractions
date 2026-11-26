namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Provides a read-only snapshot of a field selection.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	public class ReadOnlyFieldSelection<T> : IReadOnlyFieldSelection<T>
		where T : class
	{
		private readonly IReadOnlyFieldSelection<T> selection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyFieldSelection{T}"/> class with no selected fields.
		/// </summary>
		public ReadOnlyFieldSelection()
		{
			selection = new FieldSelection<T>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyFieldSelection{T}"/> class from the specified selection.
		/// </summary>
		/// <param name="selection">The field selection to wrap.</param>
		/// <exception cref="ArgumentNullException"><paramref name="selection"/> is <see langword="null"/>.</exception>
		public ReadOnlyFieldSelection(IReadOnlyFieldSelection<T> selection)
		{
			this.selection = selection ?? throw new ArgumentNullException(nameof(selection));
		}

		/// <inheritdoc />
		public int Count => selection.Count;

		/// <inheritdoc />
		public IEnumerator<IFieldProjection<T>> GetEnumerator()
		{
			return selection.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
