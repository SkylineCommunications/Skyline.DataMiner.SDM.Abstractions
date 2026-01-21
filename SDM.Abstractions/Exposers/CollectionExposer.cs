namespace Skyline.DataMiner.SDM.Exposers
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Represents an exposer for collection-type properties.
	/// </summary>
	/// <typeparam name="TFilter">The type of the object containing the collection property.</typeparam>
	/// <typeparam name="TField">The element type of the collection.</typeparam>
	public class CollectionExposer<TFilter, TField> : Exposer<TFilter, IEnumerable<TField>>
		where TFilter : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionExposer{TFilter, TField}"/> class.
		/// </summary>
		/// <param name="accessFunc">The function to access the collection property.</param>
		/// <param name="name">The name of the collection property.</param>
		public CollectionExposer(Func<TFilter, IEnumerable<TField>> accessFunc, string name)
			: base(accessFunc, name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionExposer{TFilter, TField}"/> class from an existing exposer.
		/// </summary>
		/// <param name="exposer">The exposer to copy from.</param>
		public CollectionExposer(Exposer<TFilter, IEnumerable<TField>> exposer)
			: base(exposer.internalFunc, exposer.fieldName)
		{
		}

		/// <summary>
		/// Gets the element type of the collection.
		/// </summary>
		/// <value>
		/// The <see cref="Type"/> of <typeparamref name="TField"/>.
		/// </value>
		public Type ElementType => typeof(TField);
	}
}
