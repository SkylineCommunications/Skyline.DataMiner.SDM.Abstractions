namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Linq;

	/// <summary>
	/// Defines a repository that supports querying entities.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
	public interface IQueryableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		/// <summary>
		/// Gets a queryable collection of entities.
		/// </summary>
		/// <returns>An <see cref="IQueryable{T}"/> that can be used to query entities.</returns>
		IQueryable<T> Query();
	}

	/// <summary>
	/// Defines middleware that can intercept and modify query operations.
	/// </summary>
	/// <typeparam name="T">The type of entity being queried.</typeparam>
	public interface IQueryableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		/// <summary>
		/// Intercepts a query operation and optionally modifies the queryable result.
		/// </summary>
		/// <param name="next">A function that retrieves the next queryable in the middleware chain.</param>
		/// <returns>An <see cref="IQueryable{T}"/> representing the modified or original query.</returns>
		IQueryable<T> OnQuery(Func<IQueryable<T>> next);
	}
}
