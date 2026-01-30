namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	/// <summary>
	/// Represents a repository that supports paged reading operations.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
	public interface IPageableRepository<T> : IReadableRepository<T>
		where T : class
	{
		/// <summary>
		/// Reads entities in pages using the specified filter.
		/// </summary>
		/// <param name="filter">The filter to apply when reading entities.</param>
		/// <returns>An enumerable of paged results containing the filtered entities.</returns>
		IEnumerable<IPagedResult<T>> ReadPaged(FilterElement<T> filter);

		/// <summary>
		/// Reads entities in pages using the specified query.
		/// </summary>
		/// <param name="query">The query to apply when reading entities.</param>
		/// <returns>An enumerable of paged results containing the queried entities.</returns>
		IEnumerable<IPagedResult<T>> ReadPaged(IQuery<T> query);

		/// <summary>
		/// Reads entities in pages using the specified filter and page size.
		/// </summary>
		/// <param name="filter">The filter to apply when reading entities.</param>
		/// <param name="pageSize">The number of entities per page.</param>
		/// <returns>An enumerable of paged results containing the filtered entities.</returns>
		IEnumerable<IPagedResult<T>> ReadPaged(FilterElement<T> filter, int pageSize);

		/// <summary>
		/// Reads entities in pages using the specified query and page size.
		/// </summary>
		/// <param name="query">The query to apply when reading entities.</param>
		/// <param name="pageSize">The number of entities per page.</param>
		/// <returns>An enumerable of paged results containing the queried entities.</returns>
		IEnumerable<IPagedResult<T>> ReadPaged(IQuery<T> query, int pageSize);
	}

	/// <summary>
	/// Represents middleware that intercepts paged reading operations.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the middleware.</typeparam>
	public interface IPageableMiddleware<T> : IReadableMiddleware<T>
		where T : class
	{
		/// <summary>
		/// Intercepts paged read operations using the specified filter.
		/// </summary>
		/// <param name="filter">The filter to apply when reading entities.</param>
		/// <param name="next">The next middleware or repository method in the chain.</param>
		/// <returns>An enumerable of paged results containing the filtered entities.</returns>
		IEnumerable<IPagedResult<T>> OnReadPaged(FilterElement<T> filter, Func<FilterElement<T>, IEnumerable<IPagedResult<T>>> next);

		/// <summary>
		/// Intercepts paged read operations using the specified query.
		/// </summary>
		/// <param name="query">The query to apply when reading entities.</param>
		/// <param name="next">The next middleware or repository method in the chain.</param>
		/// <returns>An enumerable of paged results containing the queried entities.</returns>
		IEnumerable<IPagedResult<T>> OnReadPaged(IQuery<T> query, Func<IQuery<T>, IEnumerable<IPagedResult<T>>> next);

		/// <summary>
		/// Intercepts paged read operations using the specified filter and page size.
		/// </summary>
		/// <param name="filter">The filter to apply when reading entities.</param>
		/// <param name="pageSize">The number of entities per page.</param>
		/// <param name="next">The next middleware or repository method in the chain.</param>
		/// <returns>An enumerable of paged results containing the filtered entities.</returns>
		IEnumerable<IPagedResult<T>> OnReadPaged(FilterElement<T> filter, int pageSize, Func<FilterElement<T>, int, IEnumerable<IPagedResult<T>>> next);

		/// <summary>
		/// Intercepts paged read operations using the specified query and page size.
		/// </summary>
		/// <param name="query">The query to apply when reading entities.</param>
		/// <param name="pageSize">The number of entities per page.</param>
		/// <param name="next">The next middleware or repository method in the chain.</param>
		/// <returns>An enumerable of paged results containing the queried entities.</returns>
		IEnumerable<IPagedResult<T>> OnReadPaged(IQuery<T> query, int pageSize, Func<IQuery<T>, int, IEnumerable<IPagedResult<T>>> next);
	}
}