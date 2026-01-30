namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	/// <summary>
	/// Defines a repository that supports read operations for entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by this repository. Must be a reference type.</typeparam>
	public interface IReadableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		/// <summary>
		/// Reads entities that match the specified filter.
		/// </summary>
		/// <param name="filter">The filter criteria to apply when reading entities.</param>
		/// <returns>An enumerable collection of entities matching the filter criteria.</returns>
		IEnumerable<T> Read(FilterElement<T> filter);

		/// <summary>
		/// Reads entities using the specified query.
		/// </summary>
		/// <param name="query">The query to execute for reading entities.</param>
		/// <returns>An enumerable collection of entities returned by the query.</returns>
		IEnumerable<T> Read(IQuery<T> query);
	}

	/// <summary>
	/// Defines middleware that can intercept and modify read operations for entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by this middleware. Must be a reference type.</typeparam>
	public interface IReadableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		/// <summary>
		/// Intercepts a read operation that uses a filter, allowing for preprocessing or postprocessing of the operation.
		/// </summary>
		/// <param name="filter">The filter criteria for the read operation.</param>
		/// <param name="next">A function that executes the next middleware or the final read operation.</param>
		/// <returns>An enumerable collection of entities after middleware processing.</returns>
		IEnumerable<T> OnRead(FilterElement<T> filter, Func<FilterElement<T>, IEnumerable<T>> next);

		/// <summary>
		/// Intercepts a read operation that uses a query, allowing for preprocessing or postprocessing of the operation.
		/// </summary>
		/// <param name="query">The query for the read operation.</param>
		/// <param name="next">A function that executes the next middleware or the final read operation.</param>
		/// <returns>An enumerable collection of entities after middleware processing.</returns>
		IEnumerable<T> OnRead(IQuery<T> query, Func<IQuery<T>, IEnumerable<T>> next);
	}
}