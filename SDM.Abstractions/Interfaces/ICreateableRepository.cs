namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	public interface ICreatableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		T Create(T oToCreate);
	}

	public interface IBulkCreatableRepository<T> : ICreatableRepository<T>
		where T : class
	{
		IReadOnlyCollection<T> Create(IEnumerable<T> oToCreate);
	}

	public interface ICreatableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		T OnCreate(T oToCreate, Func<T, T> next);
	}

	public interface IBulkCreatableMiddleware<T> : ICreatableMiddleware<T>
		where T : class
	{
		IReadOnlyCollection<T> OnCreate(IEnumerable<T> oToCreate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next);
	}
}