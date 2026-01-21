namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	public interface IUpdatableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		T Update(T oToUpdate);
	}

	public interface IBulkUpdatableRepository<T> : IUpdatableRepository<T>
		where T : class
	{
		IReadOnlyCollection<T> Update(IEnumerable<T> oToUpdate);
	}

	public interface IUpdatableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		T OnUpdate(T oToUpdate, Func<T, T> next);
	}

	public interface IBulkUpdatableMiddleware<T> : IUpdatableMiddleware<T>
		where T : class
	{
		IReadOnlyCollection<T> OnUpdate(IEnumerable<T> oToUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next);
	}
}