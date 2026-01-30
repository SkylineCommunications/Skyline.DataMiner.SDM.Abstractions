namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a generic repository that provides comprehensive data access operations.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository. Must be a reference type.</typeparam>
	public interface IRepository<T> :
		ICreatableRepository<T>,
		IPageableRepository<T>,
		IUpdatableRepository<T>,
		IDeletableRepository<T>,
		ICountableRepository<T>
		where T : class
	{
	}

	/// <summary>
	/// Represents a repository that supports bulk operations in addition to standard repository operations.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository. Must be a reference type.</typeparam>
	public interface IBulkRepository<T> :
		IRepository<T>,
		IBulkCreatableRepository<T>,
		IBulkUpdatableRepository<T>,
		IBulkDeletableRepository<T>
		where T : class
	{
		/// <summary>
		/// Creates or updates a collection of entities in a single operation.
		/// </summary>
		/// <param name="oToCreateOrUpdate">The collection of entities to create or update.</param>
		/// <returns>A read-only collection of the created or updated entities.</returns>
		IReadOnlyCollection<T> CreateOrUpdate(IEnumerable<T> oToCreateOrUpdate);
	}

	/// <summary>
	/// Represents middleware that intercepts repository operations for comprehensive data access.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the middleware. Must be a reference type.</typeparam>
	public interface IRepositoryMiddleware<T> :
		ICreatableMiddleware<T>,
		IPageableMiddleware<T>,
		IUpdatableMiddleware<T>,
		IDeletableMiddleware<T>,
		ICountableMiddleware<T>
		where T : class
	{
	}

	/// <summary>
	/// Represents middleware that intercepts bulk repository operations in addition to standard operations.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the middleware. Must be a reference type.</typeparam>
	public interface IBulkRepositoryMiddleware<T> :
		IRepositoryMiddleware<T>,
		IBulkCreatableMiddleware<T>,
		IBulkUpdatableMiddleware<T>,
		IBulkDeletableMiddleware<T>
		where T : class
	{
		/// <summary>
		/// Intercepts the create or update operation for a collection of entities.
		/// </summary>
		/// <param name="oToCreateOrUpdate">The collection of entities to create or update.</param>
		/// <param name="next">The next middleware or repository operation in the chain.</param>
		/// <returns>A read-only collection of the created or updated entities.</returns>
		IReadOnlyCollection<T> OnCreateOrUpdate(IEnumerable<T> oToCreateOrUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next);
	}
}