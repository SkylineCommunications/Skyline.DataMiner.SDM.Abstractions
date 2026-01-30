namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines a repository that supports updating entities.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
	public interface IUpdatableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <param name="oToUpdate">The entity to update.</param>
		/// <returns>The updated entity.</returns>
		T Update(T oToUpdate);
	}

	/// <summary>
	/// Defines a repository that supports bulk updating of entities.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
	public interface IBulkUpdatableRepository<T> : IUpdatableRepository<T>
		where T : class
	{
		/// <summary>
		/// Updates multiple entities in a single operation.
		/// </summary>
		/// <param name="oToUpdate">The collection of entities to update.</param>
		/// <returns>A read-only collection of the updated entities.</returns>
		IReadOnlyCollection<T> Update(IEnumerable<T> oToUpdate);
	}

	/// <summary>
	/// Defines middleware that can intercept and process update operations.
	/// </summary>
	/// <typeparam name="T">The type of entity being updated.</typeparam>
	public interface IUpdatableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		/// <summary>
		/// Intercepts an update operation, allowing pre- and post-processing.
		/// </summary>
		/// <param name="oToUpdate">The entity to update.</param>
		/// <param name="next">The next function in the middleware chain to execute.</param>
		/// <returns>The updated entity.</returns>
		T OnUpdate(T oToUpdate, Func<T, T> next);
	}

	/// <summary>
	/// Defines middleware that can intercept and process bulk update operations.
	/// </summary>
	/// <typeparam name="T">The type of entity being updated.</typeparam>
	public interface IBulkUpdatableMiddleware<T> : IUpdatableMiddleware<T>
		where T : class
	{
		/// <summary>
		/// Intercepts a bulk update operation, allowing pre- and post-processing.
		/// </summary>
		/// <param name="oToUpdate">The collection of entities to update.</param>
		/// <param name="next">The next function in the middleware chain to execute.</param>
		/// <returns>A read-only collection of the updated entities.</returns>
		IReadOnlyCollection<T> OnUpdate(IEnumerable<T> oToUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next);
	}
}