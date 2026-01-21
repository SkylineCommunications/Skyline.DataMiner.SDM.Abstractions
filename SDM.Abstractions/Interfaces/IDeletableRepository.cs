namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	public interface IDeletableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		void Delete(T oToDelete);
	}

	public interface IBulkDeletableRepository<T> : IDeletableRepository<T>
		where T : class
	{
		void Delete(IEnumerable<T> oToDelete);
	}

	public interface IDeletableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		void OnDelete(T oToDelete, Action<T> next);
	}

	public interface IBulkDeletableMiddleware<T> : IDeletableMiddleware<T>
		where T : class
	{
		void OnDelete(IEnumerable<T> oToDelete, Action<IEnumerable<T>> next);
	}
}