namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines a repository that supports deleting entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by this repository.</typeparam>
	public interface IDeletableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		/// <summary>
		/// Deletes the specified entity from the repository.
		/// </summary>
		/// <param name="oToDelete">The entity to delete.</param>
		void Delete(T oToDelete);
	}

	/// <summary>
	/// Defines a repository that supports deleting multiple entities of type <typeparamref name="T"/> in bulk operations.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by this repository.</typeparam>
	public interface IBulkDeletableRepository<T> : IDeletableRepository<T>
		where T : class
	{
		/// <summary>
		/// Deletes multiple entities from the repository in a bulk operation.
		/// </summary>
		/// <param name="oToDelete">The collection of entities to delete.</param>
		void Delete(IEnumerable<T> oToDelete);
	}

	/// <summary>
	/// Defines middleware that can intercept and process delete operations for entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity being deleted.</typeparam>
	public interface IDeletableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		/// <summary>
		/// Intercepts a delete operation for the specified entity.
		/// </summary>
		/// <param name="oToDelete">The entity being deleted.</param>
		/// <param name="next">The next action in the middleware pipeline to invoke with the entity.</param>
		void OnDelete(T oToDelete, Action<T> next);
	}

	/// <summary>
	/// Defines middleware that can intercept and process bulk delete operations for entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity being deleted.</typeparam>
	public interface IBulkDeletableMiddleware<T> : IDeletableMiddleware<T>
		where T : class
	{
		/// <summary>
		/// Intercepts a bulk delete operation for the specified entities.
		/// </summary>
		/// <param name="oToDelete">The collection of entities being deleted.</param>
		/// <param name="next">The next action in the middleware pipeline to invoke with the entities.</param>
		void OnDelete(IEnumerable<T> oToDelete, Action<IEnumerable<T>> next);
	}
}