namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Querying;
	using SLDataGateway.API.Types.Querying;

	/// <summary>
	/// Provides a factory for creating <see cref="IOrderByElement"/> instances.
	/// </summary>
	public static class OrderByElementFactory
	{
		/// <summary>
		/// Creates a new <see cref="IOrderByElement"/> instance with the specified field exposer, sort order, and natural sort option.
		/// </summary>
		/// <param name="exposer">The field exposer to use for ordering.</param>
		/// <param name="sortOrder">The sort order to apply.</param>
		/// <param name="naturalSort">Indicates whether to use natural sorting. Default is <c>false</c>.</param>
		/// <returns>
		/// An <see cref="IOrderByElement"/> configured with the specified parameters.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="exposer"/> is <c>null</c>.
		/// </exception>
		public static IOrderByElement Create(FieldExposer exposer, SortOrder sortOrder, bool naturalSort = false)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			return OrderByElement.Default
				.WithFieldExposer(exposer)
				.WithSortOrder(sortOrder)
				.WithNaturalSort(naturalSort);
		}
	}
}
