namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	public interface IPartialReadableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		IEnumerable<PartialObject<T>> ReadPartial(IPartialQuery<T> query);
	}

	public interface IPartialReadableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		IEnumerable<PartialObject<T>> OnReadPartial(
			IPartialQuery<T> query,
			Func<IPartialQuery<T>, IEnumerable<PartialObject<T>>> next);
	}

	public interface IPartialPageableRepository<T> : IPartialReadableRepository<T>
		where T : class
	{
		IEnumerable<IPagedResult<PartialObject<T>>> ReadPartialPaged(IPartialQuery<T> query);

		IEnumerable<IPagedResult<PartialObject<T>>> ReadPartialPaged(IPartialQuery<T> query, int pageSize);
	}

	public interface IPartialPageableMiddleware<T> : IPartialReadableMiddleware<T>
		where T : class
	{
		IEnumerable<IPagedResult<PartialObject<T>>> OnReadPartialPaged(
			IPartialQuery<T> query,
			Func<IPartialQuery<T>, IEnumerable<IPagedResult<PartialObject<T>>>> next);

		IEnumerable<IPagedResult<PartialObject<T>>> OnReadPartialPaged(
			IPartialQuery<T> query,
			int pageSize,
			Func<IPartialQuery<T>, int, IEnumerable<IPagedResult<PartialObject<T>>>> next);
	}
}
