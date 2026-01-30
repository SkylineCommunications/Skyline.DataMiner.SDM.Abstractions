namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	/// <summary>
	/// Represents a repository that supports counting operations for entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
	public interface ICountableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		/// <summary>
		/// Counts the number of entities that match the specified filter.
		/// </summary>
		/// <param name="filter">The filter to apply when counting entities.</param>
		/// <returns>The number of entities that match the filter.</returns>
		long Count(FilterElement<T> filter);

		/// <summary>
		/// Counts the number of entities that match the specified query.
		/// </summary>
		/// <param name="query">The query to apply when counting entities.</param>
		/// <returns>The number of entities that match the query.</returns>
		long Count(IQuery<T> query);
	}

	/// <summary>
	/// Represents middleware that can intercept and process count operations for entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the middleware.</typeparam>
	public interface ICountableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		/// <summary>
		/// Intercepts a count operation using a filter element.
		/// </summary>
		/// <param name="filter">The filter to apply when counting entities.</param>
		/// <param name="next">The next middleware or repository method in the pipeline.</param>
		/// <returns>The number of entities that match the filter.</returns>
		long OnCount(FilterElement<T> filter, Func<FilterElement<T>, long> next);

		/// <summary>
		/// Intercepts a count operation using a query.
		/// </summary>
		/// <param name="query">The query to apply when counting entities.</param>
		/// <param name="next">The next middleware or repository method in the pipeline.</param>
		/// <returns>The number of entities that match the query.</returns>
		long OnCount(IQuery<T> query, Func<IQuery<T>, long> next);
	}
}
