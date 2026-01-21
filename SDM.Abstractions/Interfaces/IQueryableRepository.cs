namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Linq;

	public interface IQueryableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		IQueryable<T> Query();
	}

	public interface IQueryableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		IQueryable<T> OnQuery(Func<IQueryable<T>> next);
	}
}
