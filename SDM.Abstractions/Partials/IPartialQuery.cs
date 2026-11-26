namespace Skyline.DataMiner.SDM
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	/// <summary>
	/// Represents a query that only retrieves a partial set of fields for a given type.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	public interface IPartialQuery<T>
		where T : class
	{
		/// <summary>
		/// Gets the underlying query.
		/// </summary>
		IQuery<T> Query { get; }

		/// <summary>
		/// Gets the collection of fields that will be retrieved by this query.
		/// </summary>
		IReadOnlyFieldSelection<T> Fields { get; }

		/// <summary>
		/// Returns a new partial query with the specified fields.
		/// </summary>
		/// <param name="fields">The fields to retrieve.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> instance with the specified fields.</returns>
		IPartialQuery<T> WithFields(IReadOnlyFieldSelection<T> fields);

		/// <summary>
		/// Returns a new partial query with the specified underlying query.
		/// </summary>
		/// <param name="query">The query to use.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> instance with the specified query.</returns>
		IPartialQuery<T> WithQuery(IQuery<T> query);

		/// <summary>
		/// Returns a new partial query with the specified filter applied.
		/// </summary>
		/// <param name="filter">The filter to apply.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> instance with the specified filter.</returns>
		IPartialQuery<T> WithFilter(FilterElement<T> filter);

		/// <summary>
		/// Returns a new partial query with the specified ordering applied.
		/// </summary>
		/// <param name="order">The ordering to apply.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> instance with the specified ordering.</returns>
		IPartialQuery<T> WithOrder(IOrderBy order);

		/// <summary>
		/// Returns a new partial query with the specified limit applied.
		/// </summary>
		/// <param name="limit">The limit to apply.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> instance with the specified limit.</returns>
		IPartialQuery<T> WithLimit(ILimitBy limit);

		/// <summary>
		/// Executes the partial query against the specified in-memory collection of items.
		/// </summary>
		/// <param name="items">The collection of items to execute the query against.</param>
		/// <returns>A collection of <see cref="PartialObject{T}"/> instances containing the selected fields for each matching item.</returns>
		IEnumerable<PartialObject<T>> ExecuteInMemory(IEnumerable<T> items);
	}
}
