namespace Skyline.DataMiner.SDM.Middleware
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Drawing.Printing;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Querying;
	using SLDataGateway.API.Types.Querying;

	/// <summary>
	/// A dynamic middleware repository that wraps any repository interface and applies middleware
	/// based on the interfaces implemented by the repository.
	/// </summary>
	/// <typeparam name="TValue">The entity type.</typeparam>
	/// <remarks>
	/// This class implements a chain-of-responsibility pattern where middleware components can intercept
	/// and modify repository operations. Each method checks if the underlying repository supports the
	/// requested operation and chains together registered middleware in reverse order before executing
	/// the final repository method.
	/// </remarks>
	public class MiddlewareRepository<TValue> : IBulkRepository<TValue>, IQueryableRepository<TValue>
		where TValue : class
	{
		private readonly IRepositoryMarker<TValue> _inner;
		private readonly IMiddlewareMarker<TValue> _middleware;

		/// <summary>
		/// Initializes a new instance of the <see cref="MiddlewareRepository{TValue}"/> class.
		/// </summary>
		/// <param name="inner">The underlying repository instance to wrap.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="middleware"/> or <paramref name="inner"/> is <see langword="null"/>.</exception>
		internal MiddlewareRepository(
			IRepositoryMarker<TValue> inner)
		{
			_inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MiddlewareRepository{TValue}"/> class.
		/// </summary>
		/// <param name="middleware">The middleware container that holds all registered middleware components.</param>
		/// <param name="inner">The underlying repository instance to wrap.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="middleware"/> or <paramref name="inner"/> is <see langword="null"/>.</exception>
		internal MiddlewareRepository(
			IRepositoryMarker<TValue> inner,
			IMiddlewareMarker<TValue> middleware)
		{
			_middleware = middleware;
			_inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Counts the number of entities matching the specified filter by applying registered middleware.
		/// </summary>
		/// <param name="filter">The filter criteria to apply when counting entities.</param>
		/// <returns>The number of entities that match the filter.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="ICountableRepository{TValue}"/>.</exception>
		public long Count(FilterElement<TValue> filter)
		{
			if (!(_inner is ICountableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support counting.");
			}

			if (_middleware is ICountableMiddleware<TValue> middleware)
			{
				return middleware.OnCount(filter, repository.Count);
			}
			else
			{
				return repository.Count(filter);
			}
		}

		/// <summary>
		/// Counts the number of entities matching the specified query by applying registered middleware.
		/// </summary>
		/// <param name="query">The query criteria to apply when counting entities.</param>
		/// <returns>The number of entities that match the query.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="ICountableRepository{TValue}"/>.</exception>
		public long Count(IQuery<TValue> query)
		{
			if (!(_inner is ICountableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support counting.");
			}

			if (_middleware is ICountableMiddleware<TValue> middleware)
			{
				return middleware.OnCount(query, repository.Count);
			}
			else
			{
				return repository.Count(query);
			}
		}

		/// <summary>
		/// Creates a new entity by applying registered middleware.
		/// </summary>
		/// <param name="oToCreate">The entity to create.</param>
		/// <returns>The created entity.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="ICreatableRepository{TValue}"/>.</exception>
		public TValue Create(TValue oToCreate)
		{
			if (!(_inner is ICreatableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support creation.");
			}

			if (_middleware is ICreatableMiddleware<TValue> middleware)
			{
				return middleware.OnCreate(oToCreate, repository.Create);
			}
			else
			{
				return repository.Create(oToCreate);
			}
		}

		/// <summary>
		/// Creates multiple entities in bulk by applying registered middleware.
		/// </summary>
		/// <param name="oToCreate">The collection of entities to create.</param>
		/// <returns>A collection of the created entities.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IBulkCreatableRepository{TValue}"/>.</exception>
		public IReadOnlyCollection<TValue> Create(IEnumerable<TValue> oToCreate)
		{
			if (!(_inner is IBulkCreatableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support bulk creation.");
			}

			if (_middleware is IBulkCreatableMiddleware<TValue> middleware)
			{
				return middleware.OnCreate(oToCreate, repository.Create);
			}
			else
			{
				return repository.Create(oToCreate);
			}
		}

		/// <summary>
		/// Creates or updates multiple entities in bulk by applying registered middleware.
		/// </summary>
		/// <param name="oToCreateOrUpdate">The collection of entities to create or update.</param>
		/// <returns>A collection of the created or updated entities.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IBulkRepository{TValue}"/>.</exception>
		public IReadOnlyCollection<TValue> CreateOrUpdate(IEnumerable<TValue> oToCreateOrUpdate)
		{
			if (!(_inner is IBulkRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support bulk create or update.");
			}

			if (_middleware is IBulkRepositoryMiddleware<TValue> middleware)
			{
				return middleware.OnCreateOrUpdate(oToCreateOrUpdate, repository.CreateOrUpdate);
			}
			else
			{
				return repository.CreateOrUpdate(oToCreateOrUpdate);
			}
		}

		/// <summary>
		/// Reads entities matching the specified filter by applying registered middleware.
		/// </summary>
		/// <param name="filter">The filter criteria to apply when reading entities.</param>
		/// <returns>An enumerable collection of entities that match the filter.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IReadableRepository{TValue}"/>.</exception>
		public IEnumerable<TValue> Read(FilterElement<TValue> filter)
		{
			if (!(_inner is IReadableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support reading.");
			}

			if (_middleware is IReadableMiddleware<TValue> middleware)
			{
				return middleware.OnRead(filter, repository.Read);
			}
			else
			{
				return repository.Read(filter);
			}
		}

		/// <summary>
		/// Reads entities matching the specified query by applying registered middleware.
		/// </summary>
		/// <param name="query">The query criteria to apply when reading entities.</param>
		/// <returns>An enumerable collection of entities that match the query.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IReadableRepository{TValue}"/>.</exception>
		public IEnumerable<TValue> Read(IQuery<TValue> query)
		{
			if (!(_inner is IReadableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support reading.");
			}

			if (_middleware is IReadableMiddleware<TValue> middleware)
			{
				return middleware.OnRead(query, repository.Read);
			}
			else
			{
				return repository.Read(query);
			}
		}

		/// <summary>
		/// Reads entities matching the specified filter in pages by applying registered middleware.
		/// </summary>
		/// <param name="filter">The filter criteria to apply when reading entities.</param>
		/// <returns>An enumerable collection of paged results containing entities that match the filter.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IPageableRepository{TValue}"/>.</exception>
		public IEnumerable<IPagedResult<TValue>> ReadPaged(FilterElement<TValue> filter)
		{
			if (!(_inner is IPageableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support paged reading.");
			}

			if (_middleware is IPageableMiddleware<TValue> middleware)
			{
				return middleware.OnReadPaged(filter, repository.ReadPaged);
			}
			else
			{
				return repository.ReadPaged(filter);
			}
		}

		/// <summary>
		/// Reads entities matching the specified query in pages by applying registered middleware.
		/// </summary>
		/// <param name="query">The query criteria to apply when reading entities.</param>
		/// <returns>An enumerable collection of paged results containing entities that match the query.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IPageableRepository{TValue}"/>.</exception>
		public IEnumerable<IPagedResult<TValue>> ReadPaged(IQuery<TValue> query)
		{
			if (!(_inner is IPageableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support paged reading.");
			}

			if (_middleware is IPageableMiddleware<TValue> middleware)
			{
				return middleware.OnReadPaged(query, repository.ReadPaged);
			}
			else
			{
				return repository.ReadPaged(query);
			}
		}

		/// <summary>
		/// Reads entities matching the specified filter in pages with a specific page size by applying registered middleware.
		/// </summary>
		/// <param name="filter">The filter criteria to apply when reading entities.</param>
		/// <param name="pageSize">The number of entities per page.</param>
		/// <returns>An enumerable collection of paged results containing entities that match the filter.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IPageableRepository{TValue}"/>.</exception>
		public IEnumerable<IPagedResult<TValue>> ReadPaged(FilterElement<TValue> filter, int pageSize)
		{
			if (!(_inner is IPageableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support paged reading.");
			}

			if (_middleware is IPageableMiddleware<TValue> middleware)
			{
				return middleware.OnReadPaged(filter, pageSize, repository.ReadPaged);
			}
			else
			{
				return repository.ReadPaged(filter, pageSize);
			}
		}

		/// <summary>
		/// Reads entities matching the specified query in pages with a specific page size by applying registered middleware.
		/// </summary>
		/// <param name="query">The query criteria to apply when reading entities.</param>
		/// <param name="pageSize">The number of entities per page.</param>
		/// <returns>An enumerable collection of paged results containing entities that match the query.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IPageableRepository{TValue}"/>.</exception>
		public IEnumerable<IPagedResult<TValue>> ReadPaged(IQuery<TValue> query, int pageSize)
		{
			if (!(_inner is IPageableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support paged reading.");
			}

			if (_middleware is IPageableMiddleware<TValue> middleware)
			{
				return middleware.OnReadPaged(query, pageSize, repository.ReadPaged);
			}
			else
			{
				return repository.ReadPaged(query, pageSize);
			}
		}

		/// <summary>
		/// Gets a queryable collection of entities by applying registered middleware.
		/// </summary>
		/// <returns>An <see cref="IQueryable{T}"/> that can be used to build LINQ queries.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IQueryableRepository{TValue}"/>.</exception>
		public IQueryable<TValue> Query()
		{
			if (!(_inner is IQueryableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support querying.");
			}

			if (_middleware is IQueryableMiddleware<TValue> middleware)
			{
				return middleware.OnQuery(repository.Query);
			}
			else
			{
				return repository.Query();
			}
		}

		/// <summary>
		/// Updates an existing entity by applying registered middleware.
		/// </summary>
		/// <param name="oToUpdate">The entity to update.</param>
		/// <returns>The updated entity.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IUpdatableRepository{TValue}"/>.</exception>
		public TValue Update(TValue oToUpdate)
		{
			if (!(_inner is IUpdatableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support updating.");
			}

			if (_middleware is IUpdatableMiddleware<TValue> middleware)
			{
				return middleware.OnUpdate(oToUpdate, repository.Update);
			}
			else
			{
				return repository.Update(oToUpdate);
			}
		}

		/// <summary>
		/// Updates multiple entities in bulk by applying registered middleware.
		/// </summary>
		/// <param name="oToUpdate">The collection of entities to update.</param>
		/// <returns>A collection of the updated entities.</returns>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IBulkUpdatableRepository{TValue}"/>.</exception>
		public IReadOnlyCollection<TValue> Update(IEnumerable<TValue> oToUpdate)
		{
			if (!(_inner is IBulkUpdatableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support bulk updating.");
			}

			if (_middleware is IBulkUpdatableMiddleware<TValue> middleware)
			{
				return middleware.OnUpdate(oToUpdate, repository.Update);
			}
			else
			{
				return repository.Update(oToUpdate);
			}
		}

		/// <summary>
		/// Deletes an entity by applying registered middleware.
		/// </summary>
		/// <param name="oToDelete">The entity to delete.</param>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IDeletableRepository{TValue}"/>.</exception>
		public void Delete(TValue oToDelete)
		{
			if (!(_inner is IDeletableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support deletion.");
			}

			if (_middleware is IDeletableMiddleware<TValue> middleware)
			{
				middleware.OnDelete(oToDelete, repository.Delete);
			}
			else
			{
				repository.Delete(oToDelete);
			}
		}

		/// <summary>
		/// Deletes multiple entities in bulk by applying registered middleware.
		/// </summary>
		/// <param name="oToDelete">The collection of entities to delete.</param>
		/// <exception cref="NotSupportedException">Thrown when the underlying repository does not implement <see cref="IBulkDeletableRepository{TValue}"/>.</exception>
		public void Delete(IEnumerable<TValue> oToDelete)
		{
			if (!(_inner is IBulkDeletableRepository<TValue> repository))
			{
				throw new NotSupportedException($"The repository of type '{_inner.GetType().FullName}' does not support bulk deletion.");
			}

			if (_middleware is IBulkDeletableMiddleware<TValue> middleware)
			{
				middleware.OnDelete(oToDelete, repository.Delete);
			}
			else
			{
				repository.Delete(oToDelete);
			}
		}
	}
}
