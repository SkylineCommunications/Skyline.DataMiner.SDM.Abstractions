namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	public interface IRepository<T> :
		ICreatableRepository<T>,
		IPageableRepository<T>,
		IUpdatableRepository<T>,
		IDeletableRepository<T>,
		ICountableRepository<T>
		where T : class
	{
	}

	public interface IBulkRepository<T> :
		IRepository<T>,
		IBulkCreatableRepository<T>,
		IBulkUpdatableRepository<T>,
		IBulkDeletableRepository<T>
		where T : class
	{
		IReadOnlyCollection<T> CreateOrUpdate(IEnumerable<T> oToCreateOrUpdate);
	}

	public interface IRepositoryMiddleware<T> :
		ICreatableMiddleware<T>,
		IPageableMiddleware<T>,
		IUpdatableMiddleware<T>,
		IDeletableMiddleware<T>,
		ICountableMiddleware<T>
		where T : class
	{
	}

	public interface IBulkRepositoryMiddleware<T> :
		IRepositoryMiddleware<T>,
		IBulkCreatableMiddleware<T>,
		IBulkUpdatableMiddleware<T>,
		IBulkDeletableMiddleware<T>
		where T : class
	{
		IReadOnlyCollection<T> OnCreateOrUpdate(IEnumerable<T> oToCreateOrUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next);
	}
}