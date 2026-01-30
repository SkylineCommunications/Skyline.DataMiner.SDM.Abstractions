namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a repository that supports creating entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
	public interface ICreatableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		/// <summary>
		/// Creates a new entity in the repository.
		/// </summary>
		/// <param name="oToCreate">The entity to create.</param>
		/// <returns>The created entity.</returns>
		T Create(T oToCreate);
	}

	/// <summary>
	/// Represents a repository that supports bulk creation of entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
	public interface IBulkCreatableRepository<T> : ICreatableRepository<T>
		where T : class
	{
		/// <summary>
		/// Creates multiple entities in the repository.
		/// </summary>
		/// <param name="oToCreate">The collection of entities to create.</param>
		/// <returns>A read-only collection of the created entities.</returns>
		IReadOnlyCollection<T> Create(IEnumerable<T> oToCreate);
	}

	/// <summary>
	/// Represents middleware that can intercept and process entity creation operations.
	/// </summary>
	/// <typeparam name="T">The type of entity being created.</typeparam>
	public interface ICreatableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		/// <summary>
		/// Intercepts the creation of an entity, allowing preprocessing or postprocessing.
		/// </summary>
		/// <param name="oToCreate">The entity being created.</param>
		/// <param name="next">The next function in the middleware pipeline to invoke.</param>
		/// <returns>The created entity after processing through the middleware pipeline.</returns>
		T OnCreate(T oToCreate, Func<T, T> next);
	}

	/// <summary>
	/// Represents middleware that can intercept and process bulk entity creation operations.
	/// </summary>
	/// <typeparam name="T">The type of entity being created.</typeparam>
	public interface IBulkCreatableMiddleware<T> : ICreatableMiddleware<T>
		where T : class
	{
		/// <summary>
		/// Intercepts the bulk creation of entities, allowing preprocessing or postprocessing.
		/// </summary>
		/// <param name="oToCreate">The collection of entities being created.</param>
		/// <param name="next">The next function in the middleware pipeline to invoke.</param>
		/// <returns>A read-only collection of the created entities after processing through the middleware pipeline.</returns>
		IReadOnlyCollection<T> OnCreate(IEnumerable<T> oToCreate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next);
	}
}